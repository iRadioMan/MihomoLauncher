using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO.Compression;

namespace MihomoLauncher
{
    public partial class MainForm: Form
    {

        private const string RegistryPath = @"Software\MihomoLauncher";
        private const string StartupPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string AppName = "MihomoLauncher";
        private static readonly HttpClient HttpClient = CreateHttpClient();
        private string ExeName = "mihomo-windows-amd64-v{cpu}.exe";
        private string Url = "https://github.com/MetaCubeX/mihomo/releases/download/{version}/mihomo-windows-amd64-v{cpu}-{version}.zip";

        private Process _mihomoProcess;
        private DateTime _startTime;
        private Timer _statusTimer;
        private NotifyIcon _trayIcon;
        private string _status;
        private bool _isRunning;

        private Dictionary<string, bool> _versions = new Dictionary<string, bool>();
        private string latestVersion;
        private string newLatestVersion;
        private string AppDirectory => Application.StartupPath;

        private static HttpClient CreateHttpClient()
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            var client = new HttpClient(handler);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("MihomoLauncher-App");
            return client;
        }

        public MainForm()
        {
            CheckSystemRequirements();
            InitializeComponent();
            SetupTrayIcon();
            SetupCustomCombo();
            InitializeFolders();

            _statusTimer = new Timer { Interval = 1000 };
            _statusTimer.Tick += (s, e) => UpdateStatus();
        }

        private void CheckSystemRequirements()
        {
            if (Environment.OSVersion.Version.Major < 10)
            {
                MessageBox.Show("Windows versions older that 10 are not supported and may not work properly!");
            }

            string _sCpuFeatureLevel = CpuChecker.GetCpuLevel().ToString();
            ExeName = ExeName.Replace("{cpu}", _sCpuFeatureLevel);
            Url = Url.Replace("{cpu}", _sCpuFeatureLevel);
        }

        private void InitializeFolders()
        {
            Directory.CreateDirectory(Path.Combine(AppDirectory, "cores"));
            Directory.CreateDirectory(Path.Combine(AppDirectory, "configs"));
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            await RefreshVersions();
            RefreshConfigs();
            LoadSettings();

            if (chkAutoStart.Checked)
            {
                btnStart_Click(null, null);
                this.Close();
            }

            _statusTimer.Start();
        }

        #region Registry & Settings
        private void RefreshConfigs()
        {
            foreach (var file in Directory.GetFiles(Path.Combine(AppDirectory, "configs")))
            {
                if (string.Equals(Path.GetExtension(file), ".yaml", StringComparison.OrdinalIgnoreCase))
                {
                    cmbConfigs.Items.Add(Path.GetFileName(file));
                }
            }

            if (cmbConfigs.Items.Count > 0)
            {
                cmbConfigs.Enabled = true;
                cmbConfigs.SelectedIndex = 0;
            }
            else
            {
                Log($"No configs found! Please copy YAML files to configs dir");
            }
        }

        private void SaveSettings()
        {
            using (var key = Registry.CurrentUser.CreateSubKey(RegistryPath))
            {
                key.SetValue("LastCore", cmbCores.SelectedItem?.ToString() ?? "");
                key.SetValue("LastConfig", cmbConfigs.SelectedItem?.ToString() ?? "");
                key.SetValue("AutoStart", chkAutoStart.Checked ? 1 : 0);
                key.SetValue("LatestVersion", latestVersion ?? "");
            }

            using (var key = Registry.CurrentUser.CreateSubKey(StartupPath))
            {
                if (chkAutoStart.Checked)
                {
                    key.SetValue(AppName, Application.ExecutablePath);
                }
                else
                {
                    try { key.DeleteValue(AppName); } catch { }
                }
            }
        }

        private void LoadSettings()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(RegistryPath))
            {
                if (key != null)
                {
                    string lastCore = key.GetValue("LastCore")?.ToString();
                    string lastConfig = key.GetValue("LastConfig")?.ToString();
                    latestVersion = (key.GetValue("LatestVersion")?.ToString() ?? latestVersion);
                    int autoStart;
                    int.TryParse(key.GetValue("AutoStart")?.ToString(), out autoStart);
                    chkAutoStart.Checked = autoStart == 1;

                    if (!string.IsNullOrEmpty(lastCore)) cmbCores.SelectedItem = lastCore;
                    if (!string.IsNullOrEmpty(lastConfig)) cmbConfigs.SelectedItem = lastConfig;
                }
                else // For first program start (no data)
                {
                    latestVersion = newLatestVersion;
                    cmbCores.SelectedIndex = cmbCores.Items.Count > 0 ? 0 : -1;
                }
            }
            
            if (newLatestVersion != null && newLatestVersion != latestVersion)
            {
                latestVersion = newLatestVersion;

                Log($"NEW VERSION AVAILABLE: {latestVersion}");
            }
        }
        #endregion

        #region GitHub API & Downloading
        private async Task RefreshVersions()
        {
            this.Text = $"Mihomo Launcher v{Application.ProductVersion} | 🌐 Fetching versions...";
            newLatestVersion = null;
            try
            {
                // Usually one page provides 15 stable releases plus the Alpha entry.
                // Fetch another page only if Alpha entries leave the first page short.
                const int pageSize = 16;
                var stableReleases = new List<JObject>();
                for (int page = 1; stableReleases.Count < 15; page++)
                {
                    string url = $"https://api.github.com/repos/MetaCubeX/mihomo/releases?per_page={pageSize}&page={page}";
                    string response;
                    using (var timeout = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(15)))
                    using (var apiResponse = await HttpClient.GetAsync(url, timeout.Token))
                    {
                        apiResponse.EnsureSuccessStatusCode();
                        response = await apiResponse.Content.ReadAsStringAsync();
                    }
                    var pageReleases = JArray.Parse(response).OfType<JObject>().ToList();
                    if (pageReleases.Count == 0) break;

                    foreach (var release in pageReleases)
                    {
                        if (!IsAlphaRelease(release) &&
                            !string.IsNullOrWhiteSpace(release["tag_name"]?.ToString()))
                        {
                            stableReleases.Add(release);
                            if (stableReleases.Count == 15) break;
                        }
                    }

                    if (pageReleases.Count < pageSize) break;
                }

                if (stableReleases.Count == 0)
                    throw new InvalidDataException("GitHub returned no stable releases.");

                // GitHub's release order can change. Filter Alpha before choosing latest.
                _versions.Clear();
                foreach (var release in stableReleases)
                {
                    string ver = release["tag_name"].ToString();
                    var path = Path.Combine(AppDirectory, "cores", ver, ExeName);
                    _versions[ver] = File.Exists(path);
                }

                newLatestVersion = stableReleases.FirstOrDefault()?["tag_name"]?.ToString();
            }
            catch (Exception ex) 
            { 
                Log($"Error fetching versions from GitHub: {ex.Message}");
                Log($"Checking locally available versions...");

                _versions.Clear();
                foreach (var dir in Directory.GetDirectories(Path.Combine(AppDirectory, "cores")))
                {
                    var version = Path.GetFileName(dir);

                    if (File.Exists(Path.Combine(AppDirectory, "cores", version, ExeName)))
                    {
                        Log($"Local version found: {version}");
                        _versions[version] = true;
                    }
                }
            }

            cmbCores.DataSource = _versions.Keys.ToList();
            cmbCores.Enabled = true;

            this.Text = $"Mihomo Launcher v{Application.ProductVersion}";
        }

        private static bool IsAlphaRelease(JObject release)
        {
            string tag = release["tag_name"]?.ToString() ?? string.Empty;
            string name = release["name"]?.ToString() ?? string.Empty;
            return tag.IndexOf("Prerelease-Alpha", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   name.IndexOf("Prerelease-Alpha", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private async void btnDownload_Click(object sender, EventArgs e)
        {
            SaveSettings();
            _statusTimer.Stop();

            string version = cmbCores.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(version))
            {
                _statusTimer.Start();
                return;
            }
            string url = Url.Replace("{version}", version);

            btnDownload.Enabled = false;
            cmbCores.Enabled = false;
            try
            {
                if (await DownloadCore(url, version))
                {
                    _versions[version] = true;
                    cmbCores.Refresh();
                    UpdateButtons();
                }
            }
            finally
            {
                _statusTimer.Start();
                cmbCores.Enabled = true;
                UpdateButtons();
            }
        }

        private async Task<bool> DownloadCore(string url, string version)
        {
            Log($"Starting download {version}...");

            var destinationPath = Path.Combine(AppDirectory, "cores", version);
            var file = Path.Combine(destinationPath, "mihomo.zip");

            try
            {
                Directory.CreateDirectory(destinationPath);
                using (var downloadStream = await HttpClient.GetStreamAsync(url))
                using (var fileStream = new FileStream(file, FileMode.Create, FileAccess.Write))
                    await downloadStream.CopyToAsync(fileStream);

                ZipFile.ExtractToDirectory(file, destinationPath);
                File.Delete(file);

                if (!File.Exists(Path.Combine(destinationPath, ExeName)))
                    throw new InvalidDataException("The downloaded archive does not contain the expected core executable.");

                Log($"Download successful");
                return true;
            }
            catch (Exception ex)
            {
                try { if (File.Exists(file)) File.Delete(file); } catch { }
                Log($"Error downloading version: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region Core Management
        private void StartCore()
        {
            if (!btnStart.Enabled) return; // Double check if button is enabled (ex: for tray controls)

            if (_mihomoProcess != null && !_mihomoProcess.HasExited)
            {
                StopCore();
                return;
            }

            if (cmbCores.SelectedItem == null || cmbConfigs.SelectedItem == null)
            {
                Log("Select a core and a configuration before starting.");
                return;
            }

            string corePath = Path.Combine(AppDirectory, "cores", cmbCores.SelectedItem.ToString(), ExeName);
            string configPath = Path.Combine(AppDirectory, "configs", cmbConfigs.SelectedItem.ToString());

            if (!File.Exists(corePath)) { Log($"Critical error: file {corePath} not found!"); return; }
            if (!File.Exists(configPath)) { Log($"Critical error: config {configPath} not found!"); return; }

            cmbCores.Enabled = false;
            cmbConfigs.Enabled = false;
            btnClearCache.Enabled = false;
            btnClearGeo.Enabled = false;
            SaveSettings();

            KillZombies();

            try
            {
                _mihomoProcess = new Process();
                _mihomoProcess.StartInfo = new ProcessStartInfo
                {
                    FileName = corePath,
                    Arguments = $"-f \"{configPath}\" -d \"{AppDirectory}\"",
                    WorkingDirectory = AppDirectory,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                };

                _mihomoProcess.OutputDataReceived += (s, args) => Log(args.Data);
                _mihomoProcess.ErrorDataReceived += (s, args) => Log(args.Data);

                _mihomoProcess.Start();
                _mihomoProcess.BeginOutputReadLine();
                _mihomoProcess.BeginErrorReadLine();

                _startTime = DateTime.Now;
                btnStart.Text = "STOP";
                _trayIcon.Icon = Properties.Resources.Meta_active;
                Log("Core started.");
            }
            catch (Exception ex)
            {
                _mihomoProcess?.Dispose();
                _mihomoProcess = null;
                Log($"Cannot start core: {ex.Message}");
                UpdateStatus();
            }
        }

        private void StopCore()
        {
            if (_mihomoProcess != null)
            {
                try 
                { 
                    _mihomoProcess.Kill(); 
                }
                catch (Exception ex) { Log($"Cannot kill core process: {ex.Message}"); }

                _mihomoProcess.Dispose();
                _mihomoProcess = null;
                Log("Core stopped.");
            }
        }

        private void KillZombies()
        {
            Process[] processes;
            try
            {
                processes = Process.GetProcesses();
            }
            catch (Exception ex)
            {
                Log($"Cannot enumerate processes: {ex.Message}");
                return;
            }

            foreach (var process in processes)
            {
                string processName = "unknown";
                try
                {
                    processName = process.ProcessName;
                    if (processName.StartsWith("mihomo-", StringComparison.OrdinalIgnoreCase))
                        process.Kill();
                }
                catch (Exception ex) { Log($"Cannot stop process {processName}: {ex.Message}"); }
                finally { process.Dispose(); }
            }
        }
        #endregion

        #region UI Helpers
        private void Log(string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new Action(() => Log(text)));
                return;
            }

            txtLog.SelectionStart = txtLog.TextLength;
            txtLog.SelectionLength = 0;

            string lowerMessage = text.ToLower();

            if (lowerMessage.Contains("error"))
            {
                txtLog.SelectionColor = Color.Red;
                txtLog.AppendText("[ERROR] ");
            }
            else if (lowerMessage.Contains("warning"))
            {
                txtLog.SelectionColor = Color.Orange;
                txtLog.AppendText("[WARNING] ");
            }
            else
            {
                txtLog.AppendText("[INFO] ");
            }

            txtLog.SelectionColor = txtLog.ForeColor;
            txtLog.AppendText($"{text}{Environment.NewLine}");

            if (chkScrollToCaret.Checked)
            {
                txtLog.ScrollToCaret();
            }
        }

        private void SetupCustomCombo()
        {
            cmbCores.DrawMode = DrawMode.OwnerDrawFixed;
            cmbCores.DrawItem += (s, e) =>
            {
                if (e.Index < 0) return;
                e.DrawBackground();
                string text = cmbCores.Items[e.Index].ToString();
                bool downloaded = _versions.ContainsKey(text) && _versions[text];
                Brush brush = downloaded ? Brushes.Black : Brushes.Red;
                e.Graphics.DrawString(text, e.Font, brush, e.Bounds);
                e.DrawFocusRectangle();
            };
        }

        private void UpdateStatus()
        {
            _isRunning = _mihomoProcess != null && !_mihomoProcess.HasExited;

            if (_isRunning)
            {
                _status = $"🟢 Running ({DateTime.Now - _startTime:hh\\:mm\\:ss})";
            }
            else
            {
                _status = "🔴 Stopped";
                btnStart.Text = "START";
                cmbCores.Enabled = true;
                cmbConfigs.Enabled = true;
                btnClearCache.Enabled = true;
                btnClearGeo.Enabled = true;
                _trayIcon.Icon = Properties.Resources.Meta;
            }

            this.Text = $"Mihomo Launcher v{Application.ProductVersion} | {_status}";
            _trayIcon.Text = $"Mihomo Launcher\nCore: {cmbCores.SelectedItem}\nStatus: {_status}";
        }

        private void UpdateButtons()
        {
            if (cmbCores.SelectedIndex != -1)
            {
                var versionDownloaded = _versions[cmbCores.SelectedItem.ToString()];
                btnDownload.Enabled = !versionDownloaded;

                if (cmbConfigs.SelectedIndex != -1)
                {
                    btnStart.Enabled = versionDownloaded;
                    chkAutoStart.Enabled = versionDownloaded;
                    btnEdit.Enabled = true;
                }
                else
                {
                    btnEdit.Enabled = false;
                }
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            StartCore();
        }

        private void cmbCores_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void cmbConfigs_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            txtLog.Clear();
        }

        private void chkAutoStart_CheckedChanged(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                string configPath = Path.Combine(Application.StartupPath, "configs", cmbConfigs.SelectedItem.ToString());
                Process.Start(configPath);
            }
            catch (Exception ex)
            {
                Log($"Cannot open the config for edit: {ex.Message}");
            }
        }

        private void btnClearCache_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    DeleteFileIfPresent(Path.Combine(AppDirectory, "cache.db"));
                    DeleteDirectoryIfPresent(Path.Combine(AppDirectory, "proxies"));
                    DeleteDirectoryIfPresent(Path.Combine(AppDirectory, "rules"));

                    Log($"Successfully cleared cache");
                }
                catch (Exception ex)
                {
                    Log($"Cannot clear cache: {ex.Message}");
                }
            }
        }

        private void btnClearGeo_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    DeleteFileIfPresent(Path.Combine(AppDirectory, "GeoIP.dat"));
                    DeleteFileIfPresent(Path.Combine(AppDirectory, "GeoSite.dat"));

                    Log($"Successfully cleared geoassets");
                }
                catch (Exception ex)
                {
                    Log($"Cannot clear geoassets: {ex.Message}");
                }
            }
        }

        private void btnOpenConfigsDir_Click(object sender, EventArgs e)
        {
            Process.Start(Path.Combine(AppDirectory, "configs"));
        }

        private static void DeleteFileIfPresent(string path)
        {
            if (File.Exists(path)) File.Delete(path);
        }

        private static void DeleteDirectoryIfPresent(string path)
        {
            if (Directory.Exists(path)) Directory.Delete(path, true);
        }
        #endregion

        #region Tray & Form Logic
        private void SetupTrayIcon()
        {
            _trayIcon = new NotifyIcon()
            {
                Icon = Properties.Resources.Meta,
                Visible = true,
                ContextMenuStrip = new ContextMenuStrip()
            };

            _trayIcon.ContextMenuStrip.Items.Add("Show launcher", null, (s, e) => this.Show());
            _trayIcon.ContextMenuStrip.Items.Add("-");
            _trayIcon.ContextMenuStrip.Items.Add("Start", null, (s, e) => StartCore());
            _trayIcon.ContextMenuStrip.Items.Add("Stop", null, (s, e) => StopCore());
            _trayIcon.ContextMenuStrip.Items.Add("Restart", null, (s, e) => { StopCore(); StartCore(); });
            _trayIcon.ContextMenuStrip.Items.Add("-");
            _trayIcon.ContextMenuStrip.Items.Add("About", null, (s, e) => { AboutBox1 about = new AboutBox1(); about.Show(); });
            _trayIcon.ContextMenuStrip.Items.Add("-");
            _trayIcon.ContextMenuStrip.Items.Add("Exit", null, (s, e) => {
                StopCore();
                SaveSettings();
                Application.Exit();
            });

            _trayIcon.DoubleClick += (s, e) => this.Show();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
            base.OnFormClosing(e);
        }
        #endregion
    }
}
