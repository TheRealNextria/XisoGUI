using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace XisoGUI
{
    public partial class MainForm : Form
    {
        private void btnAddFolder_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog
            {
                Description = "" // requested: no text
            })
            {
                if (fbd.ShowDialog(this) == DialogResult.OK)
                {
                    var dir = fbd.SelectedPath;
                    IEnumerable<string> isoFiles = Enumerable.Empty<string>();
                    try
                    {
                        isoFiles = Directory.EnumerateFiles(dir, "*.*", SearchOption.AllDirectories)
                                            .Where(p => p.EndsWith(".iso", StringComparison.OrdinalIgnoreCase)
                                                     || p.EndsWith(".xiso", StringComparison.OrdinalIgnoreCase));
                    }
                    catch (Exception ex)
                    {
                        AppendLog("Folder scan error: " + ex.Message);
                    }

                    var list = isoFiles?.ToList() ?? new System.Collections.Generic.List<string>();
                    if (list.Count == 0)
                    {
                        MessageBox.Show(this, "No ISO/XISO files found in the selected folder.", "Nothing to add",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    AddPaths(list);
                    AppendLog($"> Added {list.Count} ISO(s) from: " + dir);
                }
            }
        }
    }
}
