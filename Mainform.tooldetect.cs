using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XisoGUI
{
    public partial class MainForm : Form
    {
        // Try to fill txtExe with a discovered extract-xiso.exe
        private void AutoDetectExtractXiso()
        {
            if (!string.IsNullOrWhiteSpace(txtExe.Text) && File.Exists(txtExe.Text)) return;

            var found = FindExtractXiso();
            if (found != null)
            {
                txtExe.Text = found;
                AppendLog($"> Found extract-xiso at: {found}");
                UpdateUi();
            }
        }

        // Search next to the app and on PATH
        private static string? FindExtractXiso()
        {
            // 1) Next to this app
            var appDir = Path.GetDirectoryName(Application.ExecutablePath) ?? Environment.CurrentDirectory;
            var candidate = Path.Combine(appDir, "extract-xiso.exe");
            if (File.Exists(candidate)) return candidate;

            // 2) PATH
            var pathEnv = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
            foreach (var dir in pathEnv.Split(Path.PathSeparator).Where(p => !string.IsNullOrWhiteSpace(p)))
            {
                var cand = Path.Combine(dir.Trim(), "extract-xiso.exe");
                if (File.Exists(cand)) return cand;
            }
            return null;
        }

        // Download latest Windows build from GitHub releases and place next to app
        private async Task<string?> TryDownloadExtractXisoAsync()
        {
            try
            {
                using var http = new HttpClient();
                http.DefaultRequestHeaders.UserAgent.ParseAdd("XisoGUI/1.0 (+https://github.com/XboxDev/extract-xiso)");

                // Latest release JSON
                var latestJson = await http.GetStringAsync("https://api.github.com/repos/XboxDev/extract-xiso/releases/latest");
                using var doc = JsonDocument.Parse(latestJson);
                if (!doc.RootElement.TryGetProperty("assets", out var assets) || assets.ValueKind != JsonValueKind.Array)
                    return null;

                // Prefer Win64_Release .zip, then Win32_Release, then any .zip
                string? assetUrl = assets.EnumerateArray()
                    .Select(a => new {
                        name = a.GetProperty("name").GetString() ?? "",
                        url  = a.GetProperty("browser_download_url").GetString() ?? ""
                    })
                    .OrderBy(a => a.name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase) ? 0 : 1)
                    .ThenBy(a =>
                        a.name.Contains("Win64_Release", StringComparison.OrdinalIgnoreCase) ? 0 :
                        a.name.Contains("Win32_Release", StringComparison.OrdinalIgnoreCase) ? 1 : 2)
                    .Select(a => a.url)
                    .FirstOrDefault(u => !string.IsNullOrEmpty(u));
                if (assetUrl is null) return null;

                // Download → temp
                var tmpZip = Path.Combine(Path.GetTempPath(), "extract-xiso_" + Guid.NewGuid().ToString("N") + ".zip");
                var bytes = await http.GetByteArrayAsync(assetUrl);
                await File.WriteAllBytesAsync(tmpZip, bytes);

                // Unpack → find exe
                var appDir = Path.GetDirectoryName(Application.ExecutablePath) ?? Environment.CurrentDirectory;
                var tempExtractDir = Path.Combine(Path.GetTempPath(), "extract-xiso_unpack_" + Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(tempExtractDir);
                ZipFile.ExtractToDirectory(tmpZip, tempExtractDir, true);

                string? exeFound = Directory.EnumerateFiles(tempExtractDir, "extract-xiso*.exe", SearchOption.AllDirectories)
                                            .OrderBy(p => Path.GetFileName(p).Equals("extract-xiso.exe", StringComparison.OrdinalIgnoreCase) ? 0 : 1)
                                            .ThenByDescending(p => new FileInfo(p).Length)
                                            .FirstOrDefault();
                if (exeFound is null) return null;

                var finalPath = Path.Combine(appDir, "extract-xiso.exe");
                File.Copy(exeFound, finalPath, true);

                try { File.Delete(tmpZip); } catch { }
                try { Directory.Delete(tempExtractDir, true); } catch { }

                return finalPath;
            }
            catch (Exception ex)
            {
                AppendLog("Download error: " + ex.Message);
                return null;
            }
        }
    }
}
