namespace XisoGUI
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnCheckAll;
private System.Windows.Forms.Button btnUncheckAll;
private System.Windows.Forms.TextBox txtExe;
        private System.Windows.Forms.Button btnBrowseExe;
        private System.Windows.Forms.ListView lvQueue;
        private System.Windows.Forms.ColumnHeader colPath;
        private System.Windows.Forms.ColumnHeader colType;
        private System.Windows.Forms.ColumnHeader colStatus;
        private System.Windows.Forms.Button btnAddFiles;
        private System.Windows.Forms.Button btnBrowseInput;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnClear;
        
        private System.Windows.Forms.Button btnOpenOutput;private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.Button btnBrowseOutput;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.DataGridView gridOptions;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colOptOn;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOptFlag;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOptDesc;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.CheckBox chkContinueOnError;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            txtExe = new TextBox();
            btnBrowseExe = new Button();
            lvQueue = new ListView();
            colPath = new ColumnHeader();
            colType = new ColumnHeader();
            colStatus = new ColumnHeader();
            btnAddFiles = new Button();
            btnBrowseInput = new Button();
            btnRemove = new Button();
            btnClear = new Button();
            btnCheckAll = new Button();
            btnUncheckAll = new Button();
            txtOutput = new TextBox();
            btnBrowseOutput = new Button();
            btnRun = new Button();
            btnStop = new Button();
            btnOpenOutput = new Button();
            txtLog = new TextBox();
            gridOptions = new DataGridView();
            colOptOn = new DataGridViewCheckBoxColumn();
            colOptFlag = new DataGridViewTextBoxColumn();
            colOptDesc = new DataGridViewTextBoxColumn();
            lblCount = new Label();
            chkContinueOnError = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)gridOptions).BeginInit();
            SuspendLayout();
            // 
            // txtExe
            // 
            txtExe.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtExe.Location = new Point(12, 12);
            txtExe.Name = "txtExe";
            txtExe.PlaceholderText = "Path to extract-xiso.exe";
            txtExe.Size = new Size(640, 23);
            txtExe.TabIndex = 17;
            // 
            // btnBrowseExe
            // 
            btnBrowseExe.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowseExe.Location = new Point(658, 12);
            btnBrowseExe.Name = "btnBrowseExe";
            btnBrowseExe.Size = new Size(90, 23);
            btnBrowseExe.TabIndex = 16;
            btnBrowseExe.Text = "Browse...";
            btnBrowseExe.Click += btnBrowseExe_Click;
            // 
            // lvQueue
            // 
            lvQueue.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lvQueue.Columns.AddRange(new ColumnHeader[] { colPath, colType, colStatus });
            lvQueue.Location = new Point(12, 80);
            lvQueue.Name = "lvQueue";
            lvQueue.Size = new Size(736, 260);
            lvQueue.TabIndex = 13;
            lvQueue.UseCompatibleStateImageBehavior = false;
            lvQueue.View = View.Details;
            // 
            // colPath
            // 
            colPath.Text = "Path";
            colPath.Width = 450;
            // 
            // colType
            // 
            colType.Text = "Type";
            colType.Width = 80;
            // 
            // colStatus
            // 
            colStatus.Text = "Status";
            colStatus.Width = 120;
            // 
            // btnAddFiles
            // 
            btnAddFiles.Location = new Point(12, 346);
            btnAddFiles.Name = "btnAddFiles";
            btnAddFiles.Size = new Size(90, 27);
            btnAddFiles.TabIndex = 12;
            btnAddFiles.Text = "Add files...";
            btnAddFiles.Click += btnAddFiles_Click;
            // 
            // btnBrowseInput
            // 
            btnBrowseInput.Location = new Point(108, 346);
            btnBrowseInput.Name = "btnBrowseInput";
            btnBrowseInput.Size = new Size(116, 27);
            btnBrowseInput.TabIndex = 10;
            btnBrowseInput.Text = "Add from folder...";
            btnBrowseInput.Click += btnAddFolder_Click;
            // 
            // btnRemove
            // 
            btnRemove.Location = new Point(230, 346);
            btnRemove.Name = "btnRemove";
            btnRemove.Size = new Size(90, 27);
            btnRemove.TabIndex = 9;
            btnRemove.Text = "Remove";
            btnRemove.Click += btnRemove_Click;
            // 
            // btnClear
            // 
            btnClear.Location = new Point(326, 346);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(90, 27);
            btnClear.TabIndex = 8;
            btnClear.Text = "Clear";
            btnClear.Click += btnClear_Click;
            // 
            // btnCheckAll
            // 
            btnCheckAll.Location = new Point(12, 46);
            btnCheckAll.Name = "btnCheckAll";
            btnCheckAll.Size = new Size(90, 27);
            btnCheckAll.TabIndex = 18;
            btnCheckAll.Text = "Check all";
            btnCheckAll.UseVisualStyleBackColor = true;
            btnCheckAll.Click += btnCheckAll_Click;
            // 
            // btnUncheckAll
            // 
            btnUncheckAll.Location = new Point(108, 46);
            btnUncheckAll.Name = "btnUncheckAll";
            btnUncheckAll.Size = new Size(90, 27);
            btnUncheckAll.TabIndex = 19;
            btnUncheckAll.Text = "Uncheck all";
            btnUncheckAll.UseVisualStyleBackColor = true;
            btnUncheckAll.Click += btnUncheckAll_Click;
            // 
            // txtOutput
            // 
            txtOutput.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtOutput.Location = new Point(12, 498);
            txtOutput.Name = "txtOutput";
            txtOutput.PlaceholderText = "Output directory (used for Extract/Rewrite targets and Create output .iso)";
            txtOutput.Size = new Size(640, 23);
            txtOutput.TabIndex = 6;
            // 
            // btnBrowseOutput
            // 
            btnBrowseOutput.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnBrowseOutput.Location = new Point(658, 498);
            btnBrowseOutput.Name = "btnBrowseOutput";
            btnBrowseOutput.Size = new Size(90, 23);
            btnBrowseOutput.TabIndex = 5;
            btnBrowseOutput.Text = "Browse...";
            btnBrowseOutput.Click += btnBrowseOutput_Click;
            // 
            // btnRun
            // 
            btnRun.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnRun.Location = new Point(12, 527);
            btnRun.Name = "btnRun";
            btnRun.Size = new Size(100, 30);
            btnRun.TabIndex = 4;
            btnRun.Text = "Run";
            btnRun.Click += btnRun_Click;
            // 
            // btnStop
            // 
            btnStop.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnStop.Location = new Point(118, 527);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(100, 30);
            btnStop.TabIndex = 3;
            btnStop.Text = "Stop";
            btnStop.Click += btnStop_Click;
            // 
            // btnOpenOutput
            // 
            btnOpenOutput.Location = new Point(422, 346);
            btnOpenOutput.Name = "btnOpenOutput";
            btnOpenOutput.Size = new Size(140, 27);
            btnOpenOutput.TabIndex = 11;
            btnOpenOutput.Text = "Open output folder";
            btnOpenOutput.UseVisualStyleBackColor = true;
            btnOpenOutput.Click += btnOpenOutput_Click;
            // 
            // txtLog
            // 
            txtLog.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtLog.Location = new Point(12, 563);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.Size = new Size(736, 140);
            txtLog.TabIndex = 2;
            // 
            // gridOptions
            // 
            gridOptions.AllowUserToAddRows = false;
            gridOptions.AllowUserToDeleteRows = false;
            gridOptions.AllowUserToResizeRows = false;
            gridOptions.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gridOptions.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridOptions.Columns.AddRange(new DataGridViewColumn[] { colOptOn, colOptFlag, colOptDesc });
            gridOptions.Location = new Point(12, 379);
            gridOptions.Name = "gridOptions";
            gridOptions.RowHeadersVisible = false;
            gridOptions.Size = new Size(736, 110);
            gridOptions.TabIndex = 7;
            // 
            // colOptOn
            // 
            colOptOn.HeaderText = "On";
            colOptOn.Name = "colOptOn";
            colOptOn.Width = 40;
            // 
            // colOptFlag
            // 
            colOptFlag.HeaderText = "Flag";
            colOptFlag.Name = "colOptFlag";
            colOptFlag.Width = 60;
            // 
            // colOptDesc
            // 
            colOptDesc.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colOptDesc.HeaderText = "Description";
            colOptDesc.Name = "colOptDesc";
            // 
            // lblCount
            // 
            lblCount.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblCount.AutoSize = true;
            lblCount.Location = new Point(650, 52);
            lblCount.Name = "lblCount";
            lblCount.Size = new Size(53, 15);
            lblCount.TabIndex = 0;
            lblCount.Text = "0 item(s)";
            // 
            // chkContinueOnError
            // 
            chkContinueOnError.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            chkContinueOnError.AutoSize = true;
            chkContinueOnError.Location = new Point(224, 537);
            chkContinueOnError.Name = "chkContinueOnError";
            chkContinueOnError.Size = new Size(120, 19);
            chkContinueOnError.TabIndex = 1;
            chkContinueOnError.Text = "Continue on error";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(760, 715);
            Controls.Add(lblCount);
            Controls.Add(chkContinueOnError);
            Controls.Add(txtLog);
            Controls.Add(btnStop);
            Controls.Add(btnRun);
            Controls.Add(btnBrowseOutput);
            Controls.Add(txtOutput);
            Controls.Add(gridOptions);
            Controls.Add(btnClear);
            Controls.Add(btnOpenOutput);
            Controls.Add(btnRemove);
            Controls.Add(btnBrowseInput);
            Controls.Add(btnUncheckAll);
            Controls.Add(btnCheckAll);
            Controls.Add(btnAddFiles);
            Controls.Add(lvQueue);
            Controls.Add(btnBrowseExe);
            Controls.Add(txtExe);
            MinimumSize = new Size(700, 600);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "XisoGUI — front-end for extract-xiso";
            ((System.ComponentModel.ISupportInitialize)gridOptions).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
