using SpellWork.Forms;
using SpellWork.Properties;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellWork
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (!EnsureSetup())
                return;

            RunMainForm();
        }

        public static bool EnsureSetup()
        {
            var dbcPath = Path.Combine(Settings.Default.DbcPath, Settings.Default.Locale);
            var needsSetup = string.IsNullOrEmpty(Settings.Default.GameVersion) || !Directory.Exists(dbcPath);

            if (needsSetup)
            {
                var setup = new FormSetup();
                if (setup.ShowDialog() != DialogResult.OK)
                    return false;
            }

            return true;
        }

        public static void RunMainForm()
        {
            try
            {
                var mainForm = new FormMain();
                Task.Run(async () =>
                {
                    try
                    {
                        await DBC.DBC.Load(progress =>
                        {
                            if (mainForm.InvokeRequired)
                                mainForm.Invoke(new Action(() => mainForm.SetLoadingProgress(progress)));
                        });
                    }
                    catch (Exception ex)
                    {
                        var inner = ex;
                        while (inner.InnerException != null) inner = inner.InnerException;
                        MessageBox.Show($"Error while loading DBC:\n{ex.Message}\n\nCause: {inner.GetType().Name}: {inner.Message}");
                    }
                    finally
                    {
                        if (mainForm.InvokeRequired)
                            mainForm.Invoke(new Action(() => mainForm.Unblock()));
                        else
                            mainForm.Unblock();
                    }
                });
                Application.Run(mainForm);
            }
            catch (DirectoryNotFoundException dnfe)
            {
                MessageBox.Show(dnfe.Message, @"Missing required DBC file!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ArgumentException ae)
            {
                MessageBox.Show(ae.Message, @"DBC file has wrong structure!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"SpellWork Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
