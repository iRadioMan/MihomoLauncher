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
            Directory.CreateDirectory("cores");
            Directory.CreateDirectory("configs");
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
            foreach (var file in Directory.GetFiles(@"configs"))
            {
                if (file.EndsWith(".yaml"))
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
                key.SetValue("LatestVersion", latestVersion.ToString() ?? "");
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
                if (key == null) return;
                string lastCore = key.GetValue("LastCore")?.ToString();
                string lastConfig = key.GetValue("LastConfig")?.ToString();
                latestVersion = (key.GetValue("LatestVersion")?.ToString() ?? latestVersion); // For first program start (no data)
                chkAutoStart.Checked = Convert.ToInt32(key.GetValue("AutoStart") ?? 0) == 1;

                if (!string.IsNullOrEmpty(lastCore)) cmbCores.SelectedItem = lastCore;
                if (!string.IsNullOrEmpty(lastConfig)) cmbConfigs.SelectedItem = lastConfig;
            }
        }
        #endregion

        #region GitHub API & Downloading
        private async Task RefreshVersions()
        {
            this.Text = "Mihomo Launcher | 🌐 Fetching versions...";
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "MihomoLauncher-App");
                    var response = await client.GetStringAsync("https://api.github.com/repos/MetaCubeX/mihomo/releases");
                    var releases = JArray.Parse(response);

                    _versions.Clear();

                    foreach (var release in releases)
                    {
                        string ver = release["tag_name"].ToString();
                        var path = Path.Combine("cores", ver, ExeName);
                        bool isDownloaded = File.Exists(path);
                        _versions.Add(ver, isDownloaded);
                    }

                    string newLatestVersion = releases[1]["tag_name"].ToString(); // Ignore Alpha release [0]

                    if (latestVersion == null) // For first program start (no data)
                    {
                        latestVersion = newLatestVersion;
                    }

                    if (newLatestVersion != latestVersion) 
                    { 
                        Log($"NEW VERSION AVAILABLE: {latestVersion}");

                        latestVersion = newLatestVersion;
                    }
                }
            }
            catch (Exception ex) 
            { 
                Log($"Error fetching versions from GitHub: {ex.Message}");
                Log($"Checking locally available versions...");

                foreach (var dir in Directory.GetDirectories("cores"))
                {
                    var version = Path.GetFileName(dir);

                    if (File.Exists(Path.Combine("cores", version, ExeName)))
                    {
                        Log($"Local version found: {version}");
                        _versions.Add(version, true);
                    }
                }
            }

            cmbCores.DataSource = _versions.Keys.ToList();
            cmbCores.Enabled = true;

            this.Text = "Mihomo Launcher";
        }

        private async void btnDownload_Click(object sender, EventArgs e)
        {
            SaveSettings();
            _statusTimer.Stop();

            string version = cmbCores.SelectedItem.ToString();
            string url = Url.Replace("{version}", version);

            btnDownload.Enabled = false;
            cmbCores.Enabled = false;
            await DownloadCore(url, version);
            await RefreshVersions();
            LoadSettings();
            _statusTimer.Start();
            btnDownload.Enabled = true;
            cmbCores.Enabled = true;
        }

        private async Task DownloadCore(string url, string version)
        {
            Log($"Starting download {version}...");

            var destinationPath = @"cores\" + version + @"\";
            var file = destinationPath + "mihomo.zip";
            Directory.CreateDirectory(destinationPath);

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "MihomoLauncher-App");
                    var downloadStream = await client.GetStreamAsync(url);
                    using (var fileStream = new FileStream(file, FileMode.Create, FileAccess.Write))
                        await downloadStream.CopyToAsync(fileStream);

                    ZipFile.ExtractToDirectory(file, destinationPath);
                    File.Delete(file);

                    Log($"Download successful");
                }
            }
            catch (Exception ex) { Log($"Error downloading version: {ex.Message}"); }
        }
        #endregion

        #region Core Management
        private void StartCore()
        {
            if (!btnStart.Enabled) return; // Double check if button is enabled (ex: for tray controls)

            cmbCores.Enabled = false;
            cmbConfigs.Enabled = false;

            SaveSettings();

            if (_mihomoProcess != null && !_mihomoProcess.HasExited)
            {
                StopCore();
                return;
            }

            string corePath = Path.Combine(Application.StartupPath, "cores", cmbCores.SelectedItem.ToString(), ExeName);
            string configPath = Path.Combine(Application.StartupPath, "configs", cmbConfigs.SelectedItem.ToString());

            if (!File.Exists(corePath)) { Log($"Critical error: file {corePath} not found!"); return; }

            KillZombies();

            _mihomoProcess = new Process();
            _mihomoProcess.StartInfo = new ProcessStartInfo
            {
                FileName = corePath,
                Arguments = $"-f \"{configPath}\" -d \"{Application.StartupPath}\"",
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

        private void StopCore()
        {
            if (_mihomoProcess != null)
            {
                try 
                { 
                    _mihomoProcess.Kill(); 
                }
                catch (Exception ex) { Log($"Cannot kill core process: {ex.Message}"); }

                _mihomoProcess = null;
                Log("Core stopped.");
            }
        }

        private void KillZombies()
        {
            var currentVersion = cmbCores.SelectedItem?.ToString();
            var processes = Process.GetProcesses()
                                   .Where(p => p.ProcessName.StartsWith("mihomo-", StringComparison.OrdinalIgnoreCase));

            foreach (var p in processes)
            {
                try
                { 
                    p.Kill(); 
                } 
                catch (Exception ex) { Log($"Cannot kill process {p.ProcessName}: {ex.Message}"); }
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
            txtLog.ScrollToCaret();
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
                _trayIcon.Icon = Properties.Resources.Meta;
            }

            this.Text = $"Mihomo Launcher | {_status}";
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