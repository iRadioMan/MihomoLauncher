namespace MihomoLauncher
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.cmbCores = new System.Windows.Forms.ComboBox();
            this.btnDownload = new System.Windows.Forms.Button();
            this.cmbConfigs = new System.Windows.Forms.ComboBox();
            this.chkAutoStart = new System.Windows.Forms.CheckBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnOpenConfigsDir = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnClearGeo = new System.Windows.Forms.Button();
            this.btnClearCache = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.chkScrollToCaret = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbCores
            // 
            this.cmbCores.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.cmbCores.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCores.Enabled = false;
            this.cmbCores.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cmbCores.FormattingEnabled = true;
            this.cmbCores.Location = new System.Drawing.Point(80, 45);
            this.cmbCores.Name = "cmbCores";
            this.cmbCores.Size = new System.Drawing.Size(284, 28);
            this.cmbCores.TabIndex = 0;
            this.cmbCores.SelectedIndexChanged += new System.EventHandler(this.cmbCores_SelectedIndexChanged);
            // 
            // btnDownload
            // 
            this.btnDownload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDownload.Enabled = false;
            this.btnDownload.Location = new System.Drawing.Point(381, 45);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(105, 28);
            this.btnDownload.TabIndex = 1;
            this.btnDownload.Text = "Download";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // cmbConfigs
            // 
            this.cmbConfigs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.cmbConfigs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbConfigs.Enabled = false;
            this.cmbConfigs.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cmbConfigs.FormattingEnabled = true;
            this.cmbConfigs.Location = new System.Drawing.Point(80, 90);
            this.cmbConfigs.Name = "cmbConfigs";
            this.cmbConfigs.Size = new System.Drawing.Size(284, 28);
            this.cmbConfigs.TabIndex = 4;
            this.cmbConfigs.SelectedIndexChanged += new System.EventHandler(this.cmbConfigs_SelectedIndexChanged);
            // 
            // chkAutoStart
            // 
            this.chkAutoStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkAutoStart.AutoSize = true;
            this.chkAutoStart.Enabled = false;
            this.chkAutoStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.chkAutoStart.Location = new System.Drawing.Point(370, 144);
            this.chkAutoStart.Name = "chkAutoStart";
            this.chkAutoStart.Size = new System.Drawing.Size(129, 19);
            this.chkAutoStart.TabIndex = 6;
            this.chkAutoStart.Text = "Start with Windows";
            this.chkAutoStart.UseVisualStyleBackColor = true;
            this.chkAutoStart.CheckedChanged += new System.EventHandler(this.chkAutoStart_CheckedChanged);
            // 
            // btnStart
            // 
            this.btnStart.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnStart.Enabled = false;
            this.btnStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnStart.Location = new System.Drawing.Point(209, 139);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(121, 28);
            this.btnStart.TabIndex = 7;
            this.btnStart.Text = "START";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnClearLog
            // 
            this.btnClearLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClearLog.Location = new System.Drawing.Point(10, 265);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(93, 25);
            this.btnClearLog.TabIndex = 9;
            this.btnClearLog.Text = "Clear log";
            this.btnClearLog.UseVisualStyleBackColor = true;
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnOpenConfigsDir);
            this.groupBox1.Controls.Add(this.btnEdit);
            this.groupBox1.Controls.Add(this.cmbConfigs);
            this.groupBox1.Controls.Add(this.cmbCores);
            this.groupBox1.Controls.Add(this.btnDownload);
            this.groupBox1.Controls.Add(this.btnStart);
            this.groupBox1.Controls.Add(this.chkAutoStart);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox1.Location = new System.Drawing.Point(10, 10);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(10, 10, 10, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(539, 185);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Management";
            // 
            // btnOpenConfigsDir
            // 
            this.btnOpenConfigsDir.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnOpenConfigsDir.Location = new System.Drawing.Point(34, 90);
            this.btnOpenConfigsDir.Name = "btnOpenConfigsDir";
            this.btnOpenConfigsDir.Size = new System.Drawing.Size(31, 28);
            this.btnOpenConfigsDir.TabIndex = 9;
            this.btnOpenConfigsDir.Text = "d";
            this.btnOpenConfigsDir.UseVisualStyleBackColor = true;
            this.btnOpenConfigsDir.Click += new System.EventHandler(this.btnOpenConfigsDir_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnEdit.Enabled = false;
            this.btnEdit.Location = new System.Drawing.Point(381, 90);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(105, 28);
            this.btnEdit.TabIndex = 8;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chkScrollToCaret);
            this.groupBox2.Controls.Add(this.btnClearGeo);
            this.groupBox2.Controls.Add(this.btnClearCache);
            this.groupBox2.Controls.Add(this.txtLog);
            this.groupBox2.Controls.Add(this.btnClearLog);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox2.Location = new System.Drawing.Point(10, 205);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(10, 5, 10, 10);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(539, 299);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Log";
            // 
            // btnClearGeo
            // 
            this.btnClearGeo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearGeo.Location = new System.Drawing.Point(401, 265);
            this.btnClearGeo.Name = "btnClearGeo";
            this.btnClearGeo.Size = new System.Drawing.Size(129, 25);
            this.btnClearGeo.TabIndex = 12;
            this.btnClearGeo.Text = "Clear geoassets";
            this.btnClearGeo.UseVisualStyleBackColor = true;
            this.btnClearGeo.Click += new System.EventHandler(this.btnClearGeo_Click);
            // 
            // btnClearCache
            // 
            this.btnClearCache.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearCache.Location = new System.Drawing.Point(298, 265);
            this.btnClearCache.Name = "btnClearCache";
            this.btnClearCache.Size = new System.Drawing.Size(97, 25);
            this.btnClearCache.TabIndex = 11;
            this.btnClearCache.Text = "Clear cache";
            this.btnClearCache.UseVisualStyleBackColor = true;
            this.btnClearCache.Click += new System.EventHandler(this.btnClearCache_Click);
            // 
            // txtLog
            // 
            this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLog.BackColor = System.Drawing.Color.Black;
            this.txtLog.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtLog.ForeColor = System.Drawing.Color.White;
            this.txtLog.Location = new System.Drawing.Point(10, 21);
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.txtLog.Size = new System.Drawing.Size(520, 238);
            this.txtLog.TabIndex = 10;
            this.txtLog.Text = "";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(559, 514);
            this.tableLayoutPanel1.TabIndex = 12;
            // 
            // chkScrollToCaret
            // 
            this.chkScrollToCaret.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkScrollToCaret.AutoSize = true;
            this.chkScrollToCaret.Checked = true;
            this.chkScrollToCaret.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkScrollToCaret.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.chkScrollToCaret.Location = new System.Drawing.Point(122, 269);
            this.chkScrollToCaret.Name = "chkScrollToCaret";
            this.chkScrollToCaret.Size = new System.Drawing.Size(79, 19);
            this.chkScrollToCaret.TabIndex = 13;
            this.chkScrollToCaret.Text = "Autoscroll";
            this.chkScrollToCaret.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(559, 514);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(575, 553);
            this.Name = "MainForm";
            this.Text = "Mihomo Launcher";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ComboBox cmbCores;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.ComboBox cmbConfigs;
        private System.Windows.Forms.CheckBox chkAutoStart;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RichTextBox txtLog;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnClearCache;
        private System.Windows.Forms.Button btnClearGeo;
        private System.Windows.Forms.Button btnOpenConfigsDir;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.CheckBox chkScrollToCaret;
    }
}

