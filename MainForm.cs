//
// MainForm.cs — v1.6.1 (Disc Grouping + Overwrite/Skip/Cancel Prompt)
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XisoGUI
{
    public partial class MainForm : Form
    {
        private readonly BindingList<WorkItem> _queue = new BindingList<WorkItem>();
        private readonly HashSet<string> _seenPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private Process? _proc;
        private bool _isRunning;

        private const int ExitCodeUserCanceled = -999;
        private const int ExitCodeSkipped      = -2;
		
		
		public MainForm()
        {
            InitializeComponent();
            InitializeQueue();
            InitializeOptions();
            AutoDetectExtractXiso();
            UpdateUi();
        }

        private void InitializeQueue()
        {
            lvQueue.FullRowSelect = true;
            lvQueue.AllowDrop = true;
            lvQueue.CheckBoxes = true;

            lvQueue.DragEnter += LvQueue_DragEnter;
            lvQueue.DragDrop += LvQueue_DragDrop;
            lvQueue.ItemChecked += LvQueue_ItemChecked;

            txtExe.TextChanged += (_, __) => UpdateUi();
            txtOutput.Text = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        }

        private void InitializeOptions()
        {
            gridOptions.Rows.Clear();
            gridOptions.Rows.Add(false, "-v", "Verbose output");
            gridOptions.Rows.Add(true,  "-s", "Skip $SystemUpdate folder");
            gridOptions.Rows.Add(true,  "ui:group-discs", "Group multi-disc ISOs (Disc N subfolders)");
        }

        private void LvQueue_ItemChecked(object? sender, ItemCheckedEventArgs e)
        {
            lblCount.Text = $"{lvQueue.Items.Count} ISO(s), {lvQueue.CheckedItems.Count} selected";
            UpdateUi();
        }

        private void LvQueue_DragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void LvQueue_DragDrop(object? sender, DragEventArgs e)
        {
            if (e.Data == null) return;
            var files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files != null) AddPaths(files);
        }

        private static string NormalizePath(string p)
        {
            try { return Path.GetFullPath(p.Trim()); }
            catch { return p.Trim(); }
        }

        private void AddPaths(IEnumerable<string> paths)
        {
            int added = 0, skipped = 0;
            foreach (var raw in paths)
            {
                var p = NormalizePath(raw);
                if (File.Exists(p))
                {
                    var ext = Path.GetExtension(p).ToLowerInvariant();
                    if (ext == ".iso" || ext == ".xiso")
                    {
                        if (_seenPaths.Contains(p)) { skipped++; continue; }
                        _queue.Add(new WorkItem { Path = p, Type = WorkType.Xiso });
                        _seenPaths.Add(p);
                        added++;
                    }
                }
            }
            RefreshQueueView();
            if (skipped > 0) AppendLog($"> Skipped {skipped} duplicate(s).");
            if (added > 0) AppendLog($"> Added {added} new ISO(s).");
        }

        private void RefreshQueueView()
        {
            lvQueue.Items.Clear();
            foreach (var it in _queue)
            {
                var lvi = new ListViewItem(new[] { it.Path, it.Type.ToString(), it.Status })
                {
                    Tag = it,
                    Checked = true
                };
                lvQueue.Items.Add(lvi);
            }
            lblCount.Text = $"{_queue.Count} ISO(s), {lvQueue.CheckedItems.Count} selected";
            UpdateUi();
        }

        private void btnBrowseInput_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Title = "Add Xbox/Xbox 360 ISO(s)",
                Filter = "ISO images (*.iso;*.xiso)|*.iso;*.xiso|All files|*.*",
                Multiselect = true
            };
            if (ofd.ShowDialog(this) == DialogResult.OK)
                AddPaths(ofd.FileNames);
        }

        private void btnAddFiles_Click(object sender, EventArgs e) => btnBrowseInput_Click(sender, e);

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lvQueue.SelectedItems.Count == 0) return;
            foreach (ListViewItem sel in lvQueue.SelectedItems)
            {
                if (sel.Tag is WorkItem tag)
                {
                    _queue.Remove(tag);
                    _seenPaths.Remove(NormalizePath(tag.Path));
                }
            }
            RefreshQueueView();
        }

        
        private void btnCheckAll_Click(object? sender, EventArgs e)
        {
            foreach (ListViewItem lvi in lvQueue.Items)
                lvi.Checked = true;
            lblCount.Text = $"{lvQueue.Items.Count} ISO(s), {lvQueue.CheckedItems.Count} selected";
            UpdateUi();
        }

        private void btnUncheckAll_Click(object? sender, EventArgs e)
        {
            foreach (ListViewItem lvi in lvQueue.Items)
                lvi.Checked = false;
            lblCount.Text = $"{lvQueue.Items.Count} ISO(s), {lvQueue.CheckedItems.Count} selected";
            UpdateUi();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            _queue.Clear();
            _seenPaths.Clear();
            RefreshQueueView();
        }

        private void btnBrowseExe_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Title = "Locate extract-xiso.exe",
                Filter = "extract-xiso.exe|extract-xiso.exe|All files|*.*",
                Multiselect = false
            };
            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                txtExe.Text = ofd.FileName;
                UpdateUi();
            }
        }

        private void btnBrowseOutput_Click(object sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog { Description = string.Empty };
            if (fbd.ShowDialog(this) == DialogResult.OK)
                txtOutput.Text = fbd.SelectedPath;
        }

        private async void btnRun_Click(object sender, EventArgs e) => await RunQueueAsync();

        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                if (_proc != null && !_proc.HasExited)
                {
                    try { _proc.Kill(entireProcessTree: true); }
                    catch { _proc.Kill(); }
                }
            }
            catch { }
        }

        private async Task RunQueueAsync()
        {
            if (_isRunning) return;

            var itemsToRun = new List<WorkItem>();
            foreach (ListViewItem lvi in lvQueue.CheckedItems)
                if (lvi.Tag is WorkItem wi) itemsToRun.Add(wi);

            if (itemsToRun.Count == 0)
            {
                MessageBox.Show(this, "No items are checked. Check the ISO(s) you want to extract, then click Run.",
                    "Nothing selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtExe.Text) || !File.Exists(txtExe.Text))
                AutoDetectExtractXiso();

            if (string.IsNullOrWhiteSpace(txtExe.Text) || !File.Exists(txtExe.Text))
            {
                var res = MessageBox.Show(this,
                    "extract-xiso.exe was not found. Do you want me to download the latest Windows build and place it next to this app?",
                    "extract-xiso missing", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res == DialogResult.Yes)
                {
                    AppendLog("> Fetching latest extract-xiso release from GitHub...");
                    var path = await TryDownloadExtractXisoAsync();
                    if (!string.IsNullOrEmpty(path) && File.Exists(path))
                    {
                        txtExe.Text = path;
                        AppendLog($"> Downloaded and placed at: {path}");
                        UpdateUi();
                    }
                    else
                    {
                        MessageBox.Show(this, "Could not download extract-xiso automatically. Please download it manually.",
                            "Download failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        UpdateUi();
                        return;
                    }
                }
                else
                {
                    UpdateUi();
                    return;
                }
            }

            _isRunning = true;
            UpdateUi();
            txtLog.Clear();

            try
            {
                foreach (var item in itemsToRun)
                {
                    item.Status = "Extracting...";
                    UpdateItem(item);

                    var exit = await ExtractOneAsync(item);

                    if (exit == ExitCodeUserCanceled)
                    {
                        item.Status = "Canceled by user";
                        UpdateItem(item);
						break;
}
                    if (exit == ExitCodeSkipped)
                    {
                        // Keep "Skipped (exists)" status set earlier and continue to next
                        UpdateItem(item);
                        continue;
                    }


                    item.Status = exit == 0 ? "Done" : $"Failed ({exit})";
                    UpdateItem(item);

                    if (exit != 0 && !chkContinueOnError.Checked)
                        break;
                }
            }
            finally
            {
                _isRunning = false;
                UpdateUi();
            }
        }

        private async Task<int> ExtractOneAsync(WorkItem item)
        {
            var (args, ensureDir, workdir) = BuildExtractArgs(item);

            // Prompt: Yes = Overwrite, No = Skip, Cancel = stop whole run
            if (!string.IsNullOrEmpty(ensureDir) && Directory.Exists(ensureDir))
            {
                var resp = MessageBox.Show(
                    this,
                    "The destination already exists:\n\n" + ensureDir +
                    "\n\nYes = Overwrite (delete and recreate)\nNo = Skip this game\nCancel = Stop the entire run",
                    "Destination exists",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2
                );

                if (resp == DialogResult.Cancel)
                    return ExitCodeUserCanceled;

                if (resp == DialogResult.No)
                {
                    AppendLog($"> Skipped (destination exists): {ensureDir}");
                    item.Status = "Skipped (exists)";
                    UpdateItem(item);
                    return ExitCodeSkipped;
                }

                // Yes → overwrite
                try { Directory.Delete(ensureDir, true); } catch { }
            }

            if (!string.IsNullOrEmpty(ensureDir) && !Directory.Exists(ensureDir))
                Directory.CreateDirectory(ensureDir);

            AppendLog($"> CWD: {workdir}");
            AppendLog($"> {txtExe.Text} {string.Join(" ", args)}");

            var tcs = new TaskCompletionSource<int>();
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo(txtExe.Text)
                {
                    WorkingDirectory = workdir,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                },
                EnableRaisingEvents = true
            };
            foreach (var a in args) proc.StartInfo.ArgumentList.Add(a);

            proc.OutputDataReceived += (s, e) => { if (e.Data != null) AppendLog(e.Data); };
            proc.ErrorDataReceived  += (s, e) => { if (e.Data != null) AppendLog(e.Data); };
            proc.Exited += (s, e) => tcs.TrySetResult(proc.ExitCode);

            _proc = proc;
            proc.Start();
            proc.BeginOutputReadLine();
            proc.BeginErrorReadLine();
            var code = await tcs.Task;
            proc.Dispose();
            _proc = null;

            return code;
        }

        private (List<string> args, string ensureDir, string workdir) BuildExtractArgs(WorkItem item)
        {
            var args = new List<string>();
            string ensureDir = string.Empty;
            string workdir = Path.GetDirectoryName(txtExe.Text) ?? Environment.CurrentDirectory;

            bool groupDiscs = false;
            foreach (DataGridViewRow row in gridOptions.Rows)
            {
                if (row.IsNewRow) continue;
                var on = row.Cells[0].Value as bool? ?? false;
                var key = Convert.ToString(row.Cells[1].Value) ?? string.Empty;

                if (key.Equals("ui:group-discs", StringComparison.OrdinalIgnoreCase))
                {
                    groupDiscs = on;
                    continue;
                }

                if (on && !string.IsNullOrWhiteSpace(key))
                {
                    args.Add(key.Trim());
                }
            }

            var outDir = (txtOutput.Text ?? string.Empty).Trim();
            var input  = item.Path;

            if (!string.IsNullOrEmpty(outDir))
            {
                string target;
                if (groupDiscs)
                {
                    var baseName = Path.GetFileNameWithoutExtension(item.Path);
                    var parsed = ParseDiscInfo(baseName);
                    if (parsed.IsDisc)
                    {
                        var parent = Path.Combine(outDir, parsed.SeriesKey);
                        target = Path.Combine(parent, $"Disc {parsed.DiscNumber}");
                    }
                    else
                    {
                        target = Path.Combine(outDir, baseName);
                    }
                }
                else
                {
                    target = Path.Combine(outDir, Path.GetFileNameWithoutExtension(item.Path));
                }

                ensureDir = target;
                workdir = target;
                args.Add("-d");
                args.Add(target);
            }

            args.Add(input);
            return (args, ensureDir, workdir);
        }

        private static readonly Regex DiscRegex = new Regex(
            @"(?ix)
            (?:^|[\s._\-\(\)\[\]])
            (?:disc|disk|dvd|cd)\s*[-_ ]?\s*
            (?<num>\d{1,2})
            (?:$|[\s._\-\)\]\(])
            "
        );

        private static readonly Regex BracketedDiscRegex = new Regex(
            @"(?ix)[\(\[\{]\s*(?:disc|disk|dvd|cd)\s*\d+\s*[\)\]\}]"
        );

        private static readonly Regex ReleaseTagRegex = new Regex(
            @"(?ix)\s*[\(\[\{](?:usa|eur|pal|ntsc|multi\d*|eng|en|fr|de|es|it|jp|usa\s*ntsc|proper|repack|rip|rf)\s*[\)\]\}]"
        );

        private static DiscInfo ParseDiscInfo(string baseName)
        {
            var m = DiscRegex.Match(baseName);
            if (!m.Success) return DiscInfo.NotDisc(baseName);

            int disc = 0;
            int.TryParse(m.Groups["num"].Value, out disc);
            disc = Math.Max(1, disc);

            var series = BracketedDiscRegex.Replace(baseName, "");
            series = DiscRegex.Replace(series, "");
            series = ReleaseTagRegex.Replace(series, "");
            series = series.Replace("__", "_").Replace("..", ".").Replace("--", "-");
            series = series.Trim().Trim('_', '-', '.', ' ');
            if (string.IsNullOrWhiteSpace(series)) series = baseName;

            return new DiscInfo(true, disc, series);
        }

        private readonly struct DiscInfo
        {
            public bool IsDisc { get; }
            public int DiscNumber { get; }
            public string SeriesKey { get; }
            public DiscInfo(bool isDisc, int num, string series)
            {
                IsDisc = isDisc; DiscNumber = num; SeriesKey = series;
            }
            public static DiscInfo NotDisc(string baseName) => new DiscInfo(false, 0, baseName);
        }

        private void AppendLog(string line)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new Action<string>(AppendLog), line);
                return;
            }
            txtLog.AppendText(line + Environment.NewLine);
        }

        private void UpdateItem(WorkItem item)
        {
            foreach (ListViewItem lvi in lvQueue.Items)
            {
                if (ReferenceEquals(lvi.Tag, item))
                {
                    lvi.SubItems[2].Text = item.Status;
                    break;
                }
            }
        }

        private void UpdateUi()
        {
            bool idle = !_isRunning;
            btnRun.Enabled         = idle && lvQueue.CheckedItems.Count > 0;
            btnStop.Enabled        = !idle;
            btnAddFiles.Enabled    = idle;
            btnBrowseInput.Enabled = idle;
            btnRemove.Enabled      = idle;
            btnClear.Enabled       = idle;
            if (gridOptions != null) gridOptions.Enabled = idle;
        }
    }

    public enum WorkType { Xiso, Directory, File }

    public class WorkItem
    {
        public string Path { get; set; } = string.Empty;
        public WorkType Type { get; set; } = WorkType.Xiso;
        public string Status { get; set; } = "Queued";
    }
}
