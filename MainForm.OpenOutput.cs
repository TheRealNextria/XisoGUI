using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace XisoGUI
{
    public partial class MainForm : Form
    {
        // Adds the method referenced by btnOpenOutput_Click
        private void OpenOutputFolder()
        {
            try
            {
                var path = (txtOutput.Text ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(path))
                {
                    MessageBox.Show(this, "No output folder is set.", "Open output folder",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (!Directory.Exists(path))
                {
                    MessageBox.Show(this, "The output folder does not exist yet.", "Open output folder",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                Process.Start(new ProcessStartInfo("explorer.exe", path) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Could not open output folder: " + ex.Message, "Open output folder",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
