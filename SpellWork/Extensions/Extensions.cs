using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace SpellWork.Extensions
{
    public static class Extensions
    {
        public static StringBuilder AppendFormatIfNotNull(this StringBuilder builder, string format, params object[] arg)
        {
            return arg[0].ToUInt32() != 0 ? builder.AppendFormat(format, arg) : builder;
        }

        // Append Format Line
        public static StringBuilder AppendFormatLine(this StringBuilder builder, string format, params object[] arg0)
        {
            return builder.AppendFormat(format, arg0).AppendLine();
        }

        public static StringBuilder AppendFormatLineIfNotNull(this StringBuilder builder, string format, int arg)
        {
            return arg != 0 ? builder.AppendFormat(format, arg).AppendLine() : builder;
        }

        public static StringBuilder AppendFormatLineIfNotNull(this StringBuilder builder, string format, uint arg)
        {
            return arg != 0 ? builder.AppendFormat(format, arg).AppendLine() : builder;
        }

        public static uint ToUInt32(this object val)
        {
            if (val == null)
                return 0;
            var valStr = val.ToString();

            uint num;
            if (valStr.StartsWith("0x"))
                uint.TryParse(valStr.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num);
            else
                uint.TryParse(valStr, out num);
            return num;
        }

        public static int ToInt32(this object val)
        {
            if (val == null)
                return 0;
            var valStr = val.ToString();

            int num;
            if (valStr.StartsWith("0x"))
                int.TryParse(valStr.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num);
            else
                int.TryParse(valStr, out num);
            return num;
        }

        public static float ToFloat(this object val)
        {
            if (val == null)
                return 0.0f;

            float num;
            float.TryParse(val.ToString().Replace(',', '.'), out num);
            return num;
        }

        public static ulong ToUlong(this object val)
        {
            if (val == null)
                return 0U;
            var valStr = val.ToString();

            ulong num;
            if (valStr.StartsWith("0x"))
                ulong.TryParse(valStr.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num);
            else
                ulong.TryParse(valStr, out num);
            return num;
        }

        public static long ToLong(this object val)
        {
            if (val == null)
                return 0;
            var valStr = val.ToString();

            long num;
            if (valStr.StartsWith("0x"))
                long.TryParse(valStr.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num);
            else
                long.TryParse(valStr, out num);
            return num;
        }

        public static string NormalizeString(this string text, string remove = null)
        {
            var str = string.Empty;
            if (!string.IsNullOrEmpty(remove))
                text = text.Replace(remove, string.Empty);

            foreach (var s in text.Split('_'))
            {
                var i = 0;
                foreach (var c in s.ToCharArray())
                {
                    str += i == 0 ? c.ToString().ToUpper() : c.ToString().ToLower();
                    i++;
                }
                str += " ";
            }

            return str.Remove(str.Length - 1);
        }

        public static void SetEnumValues<T>(this ComboBox cb, string noValue)
        {
            var dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("NAME");

            dt.Rows.Add(-1, noValue);

            foreach (var str in Enum.GetValues(typeof(T)))
                dt.Rows.Add((int)str, "(" + ((int)str).ToString("000") + ") " + str);

            cb.DataSource = dt;
            cb.DisplayMember = "NAME";
            cb.ValueMember = "ID";

            cb.MakeSearchable();
        }

        public static void SetEnumValuesDirect<T>(this ComboBox cb, bool setFirstValue)
        {
            cb.BeginUpdate();

            cb.Items.Clear();
            foreach (var value in Enum.GetValues(typeof(T)))
                cb.Items.Add(((Enum)value).GetFullName());

            if (setFirstValue && cb.Items.Count > 0)
                cb.SelectedIndex = 0;

            cb.EndUpdate();
        }

        public static void SetStructFields<T>(this ComboBox cb)
        {
            cb.Items.Clear();

            var dt = new DataTable();
            dt.Columns.Add("ID", typeof(MemberInfo));
            dt.Columns.Add("NAME", typeof(string));

            var type = typeof(T).GetMembers();
            var i = 0;
            foreach (var str in type)
            {
                if (!(str is FieldInfo) && !(str is PropertyInfo))
                    continue;

                if (str.GetCustomAttribute<IgnoreAutopopulatedFilterValueAttribute>() != null)
                    continue;

                var dr = dt.NewRow();
                dr["ID"] = str;
                dr["NAME"] = $"({i:000}) {str.Name}";
                dt.Rows.Add(dr);
                ++i;
            }

            cb.DataSource    = dt;
            cb.DisplayMember = "NAME";
            cb.ValueMember   = "ID";

            cb.MakeSearchable();
        }

        /// <summary>
        /// Makes a data-bound ComboBox editable so typing filters the drop-down list
        /// (contains-match on the display text) instead of only allowing selection.
        /// </summary>
        public static void MakeSearchable(this ComboBox cb)
        {
            cb.DropDownStyle = ComboBoxStyle.DropDown;
            cb.AutoCompleteMode = AutoCompleteMode.None;

            var lastIndex = cb.SelectedIndex >= 0 ? cb.SelectedIndex : 0;
            var openingForFilter = false;

            cb.SelectedIndexChanged += (s, e) =>
            {
                if (cb.SelectedIndex >= 0)
                    lastIndex = cb.SelectedIndex;
            };

            cb.TextUpdate += (s, e) =>
            {
                var table = cb.DataSource as DataTable;
                if (table == null)
                    return;

                var text = cb.Text;
                table.DefaultView.RowFilter = string.IsNullOrEmpty(text)
                    ? string.Empty
                    : $"NAME LIKE '%{EscapeLikePattern(text)}%'";

                // filtering can drop the current selection; keep the typed text
                if (cb.Text != text)
                    cb.Text = text;

                openingForFilter = true;
                cb.DroppedDown = true;
                openingForFilter = false;

                cb.SelectionStart = cb.Text.Length;
                cb.SelectionLength = 0;
            };

            cb.DropDown += (s, e) =>
            {
                // reopened after a committed selection: show the full list again
                if (openingForFilter || cb.SelectedIndex < 0 || cb.Text != cb.GetItemText(cb.SelectedItem))
                    return;

                var table = cb.DataSource as DataTable;
                if (table != null)
                    table.DefaultView.RowFilter = string.Empty;
            };

            cb.Leave += (s, e) =>
            {
                var table = cb.DataSource as DataTable;
                if (table != null)
                    table.DefaultView.RowFilter = string.Empty;

                if (cb.SelectedIndex >= 0)
                    return;

                var index = cb.FindStringExact(cb.Text);
                if (index >= 0)
                    cb.SelectedIndex = index;
                else if (string.IsNullOrWhiteSpace(cb.Text))
                    cb.SelectedIndex = 0;
                else
                    cb.SelectedIndex = Math.Min(lastIndex, cb.Items.Count - 1);
            };
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

        /// <summary>
        /// Compares the text on the partial occurrence of a string and ignore case
        /// </summary>
        /// <param name="text">The original text, which will search entry</param>
        /// <param name="compareText">String which will be matched with the original text</param>
        /// <returns>Boolean(true or false)</returns>
        public static bool ContainsText(this string text, string compareText)
        {
            return text.ToUpper().IndexOf(compareText.ToUpper(), StringComparison.CurrentCultureIgnoreCase) != -1;
        }

        /// <summary>
        /// Compares the text on the partial occurrence of a string and ignore case
        /// </summary>
        /// <param name="text">The original text, which will search entry</param>
        /// <param name="compareText">Array strings which will be matched with the original text</param>
        /// <returns>Boolean(true or false)</returns>
        public static bool ContainsText(this string text, string[] compareText)
        {
            return compareText.Any(str => text.IndexOf(str, StringComparison.CurrentCultureIgnoreCase) != -1);
        }

        public static bool ContainsElement(this uint[] array, uint[] value)
        {
            return array.Length == value.Length && array.Where((t, i) => (t & value[i]) != 0).Any();
        }

        /// <summary>
        /// Checks if the specified value in a given array
        /// </summary>
        /// <param name="array">Array in which to search</param>
        /// <param name="value">Meaning Search</param>
        /// <returns>true or false</returns>
        public static bool ContainsElement<T>(this T[] array, T value) where T : IComparable
        {
            return array.Any(i => i.Equals(value));
        }

        public static TValue GetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) where TValue : class
        {
            return dictionary.TryGetValue(key, out TValue value) ? value : null;
        }

        public static string GetFullName(this Enum @enum)
        {
            var field = @enum.GetType().GetField(@enum.ToString());
            var attrs = (FullNameAttribute[])field?.GetCustomAttributes(typeof(FullNameAttribute), false);

            return attrs?.Length > 0 ? attrs[0].FullName : @enum.ToString();
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class FullNameAttribute : Attribute
    {
        public string FullName { get; }

        public FullNameAttribute(string fullName)
        {
            FullName = fullName;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class IgnoreAutopopulatedFilterValueAttribute : Attribute
    {
        public string Reason { get; }

        public IgnoreAutopopulatedFilterValueAttribute(string reason = "")
        {
            Reason = reason;
        }
    }
}
