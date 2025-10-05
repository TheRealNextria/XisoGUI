using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace XisoGUI
{
    public partial class MainForm : Form
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            TryApplyAppIcon();
        }

        private void TryApplyAppIcon()
        {
            try
            {
                this.ShowInTaskbar = true; // make sure it's visible

                string baseDir = AppContext.BaseDirectory;
                string[] candidates = new string[]
                {
                    Path.Combine(baseDir, "Assets", "XisoGUI.ico"),
                    Path.Combine(baseDir, "XisoGUI.ico"),
                };

                foreach (var icoPath in candidates)
                {
                    if (File.Exists(icoPath))
                    {
                        using (var fs = new FileStream(icoPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            this.Icon = new Icon(fs);
                        }
                        return;
                    }
                }

                // Fallback: use the EXE's own associated icon (from <ApplicationIcon>)
                try
                {
                    var exeIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
                    if (exeIcon != null)
                    {
                        this.Icon = exeIcon;
                        return;
                    }
                }
                catch { /* ignore */ }
            }
            catch
            {
                // swallow icon issues quietly
            }
        }
    }
}
