using System;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace SpellWork.Controls
{
    /// <summary>
    /// Editable ComboBox: typing filters the drop-down list (contains-match on the
    /// display text) while the list stays open. Expects a DataTable DataSource with
    /// a "NAME" column, as populated by the SetEnumValues/SetStructFields extensions.
    /// </summary>
    public class SearchableComboBox : ComboBox
    {
        private int _lastIndex;
        private bool _openingForFilter;

        public SearchableComboBox()
        {
            AutoCompleteMode = AutoCompleteMode.None;
        }

        // WinForms quirk: while the bound view is filtered the native control can
        // still report a stale selection index, which makes the base getter throw
        // ArgumentOutOfRangeException from Items[SelectedIndex].
        public override string Text
        {
            get
            {
                try
                {
                    return base.Text;
                }
                catch (ArgumentOutOfRangeException)
                {
                    return string.Empty;
                }
            }
            set => base.Text = value;
        }

        protected override void WndProc(ref Message m)
        {
            try
            {
                base.WndProc(ref m);
            }
            catch (ArgumentOutOfRangeException)
            {
                // same stale-index quirk, thrown from internal message handlers
                // (e.g. CBN_SELCHANGE reading SelectedItem/Text mid-refilter)
            }
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            if (SelectedIndex >= 0 && SelectedIndex < Items.Count)
                _lastIndex = SelectedIndex;

            base.OnSelectedIndexChanged(e);
        }

        protected override void OnTextUpdate(EventArgs e)
        {
            if (DataSource is DataTable table)
            {
                var text = Text;
                table.DefaultView.RowFilter = string.IsNullOrEmpty(text)
                    ? string.Empty
                    : $"NAME LIKE '%{EscapeLikePattern(text)}%'";

                // filtering can drop the current selection; keep the typed text
                if (Text != text)
                    Text = text;

                _openingForFilter = true;
                DroppedDown = true;
                _openingForFilter = false;

                SelectionStart = Text.Length;
                SelectionLength = 0;
            }

            base.OnTextUpdate(e);
        }

        protected override void OnDropDown(EventArgs e)
        {
            // reopened after a committed selection: show the full list again
            if (!_openingForFilter && SelectedIndex >= 0 && SelectedIndex < Items.Count
                && Text == GetItemText(Items[SelectedIndex])
                && DataSource is DataTable table)
            {
                table.DefaultView.RowFilter = string.Empty;
            }

            base.OnDropDown(e);
        }

        protected override void OnLeave(EventArgs e)
        {
            if (DataSource is DataTable table)
                table.DefaultView.RowFilter = string.Empty;

            if (SelectedIndex < 0 || SelectedIndex >= Items.Count)
            {
                var index = FindStringExact(Text);
                if (index >= 0)
                    SelectedIndex = index;
                else if (string.IsNullOrWhiteSpace(Text))
                    SelectedIndex = 0;
                else
                    SelectedIndex = Math.Min(_lastIndex, Items.Count - 1);
            }

            base.OnLeave(e);
        }

        private static string EscapeLikePattern(string text)
        {
            var sb = new StringBuilder(text.Length + 8);
            foreach (var c in text)
            {
                switch (c)
                {
                    case '*':
                    case '%':
                    case '[':
                    case ']':
                        sb.Append('[').Append(c).Append(']');
                        break;
                    case '\'':
                        sb.Append("''");
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }
            return sb.ToString();
        }
    }
}
