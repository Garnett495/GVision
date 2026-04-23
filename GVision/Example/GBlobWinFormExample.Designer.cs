namespace GVision.Example
{
    partial class GBlobWinFormExample
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnLoadImage = new Sunny.UI.UIButton();
            this.btnRunProcess = new Sunny.UI.UIButton();
            this.numThreshold = new Sunny.UI.UIUpDownTextBox();
            this.uiGroupBox1 = new Sunny.UI.UIGroupBox();
            this.chkEnableMorphology = new Sunny.UI.UICheckBox();
            this.chkEnableBlur = new Sunny.UI.UICheckBox();
            this.chkInvertThreshold = new Sunny.UI.UICheckBox();
            this.UIlebal = new Sunny.UI.UILabel();
            this.lblMinArea = new Sunny.UI.UILabel();
            this.lblThreshold = new Sunny.UI.UILabel();
            this.numMinArea = new Sunny.UI.UIUpDownTextBox();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.uiGroupBox2 = new Sunny.UI.UIGroupBox();
            this.uiTableLayoutPanel1 = new Sunny.UI.UITableLayoutPanel();
            this.lblInfo = new Sunny.UI.UILabel();
            this.uiGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.uiGroupBox2.SuspendLayout();
            this.uiTableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnLoadImage
            // 
            this.btnLoadImage.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLoadImage.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnLoadImage.Location = new System.Drawing.Point(13, 35);
            this.btnLoadImage.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnLoadImage.Name = "btnLoadImage";
            this.btnLoadImage.Size = new System.Drawing.Size(100, 35);
            this.btnLoadImage.TabIndex = 0;
            this.btnLoadImage.Text = "Load";
            this.btnLoadImage.TipsFont = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnLoadImage.Click += new System.EventHandler(this.BtnLoadImage_Click);
            // 
            // btnRunProcess
            // 
            this.btnRunProcess.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRunProcess.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnRunProcess.Location = new System.Drawing.Point(119, 35);
            this.btnRunProcess.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnRunProcess.Name = "btnRunProcess";
            this.btnRunProcess.Size = new System.Drawing.Size(100, 35);
            this.btnRunProcess.TabIndex = 1;
            this.btnRunProcess.Text = "Run";
            this.btnRunProcess.TipsFont = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnRunProcess.Click += new System.EventHandler(this.BtnRunBlob_Click);
            // 
            // numThreshold
            // 
            this.numThreshold.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.numThreshold.DoubleStep = 1D;
            this.numThreshold.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.numThreshold.Location = new System.Drawing.Point(89, 78);
            this.numThreshold.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numThreshold.MinimumSize = new System.Drawing.Size(1, 16);
            this.numThreshold.Name = "numThreshold";
            this.numThreshold.Padding = new System.Windows.Forms.Padding(5);
            this.numThreshold.ShowText = false;
            this.numThreshold.Size = new System.Drawing.Size(150, 29);
            this.numThreshold.TabIndex = 2;
            this.numThreshold.Text = "0";
            this.numThreshold.TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
            this.numThreshold.Type = Sunny.UI.UITextBox.UIEditType.Integer;
            this.numThreshold.Watermark = "";
            // 
            // uiGroupBox1
            // 
            this.uiGroupBox1.Controls.Add(this.lblInfo);
            this.uiGroupBox1.Controls.Add(this.chkEnableMorphology);
            this.uiGroupBox1.Controls.Add(this.chkEnableBlur);
            this.uiGroupBox1.Controls.Add(this.chkInvertThreshold);
            this.uiGroupBox1.Controls.Add(this.UIlebal);
            this.uiGroupBox1.Controls.Add(this.lblMinArea);
            this.uiGroupBox1.Controls.Add(this.lblThreshold);
            this.uiGroupBox1.Controls.Add(this.numMinArea);
            this.uiGroupBox1.Controls.Add(this.btnLoadImage);
            this.uiGroupBox1.Controls.Add(this.numThreshold);
            this.uiGroupBox1.Controls.Add(this.btnRunProcess);
            this.uiGroupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiGroupBox1.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.uiGroupBox1.Location = new System.Drawing.Point(4, 5);
            this.uiGroupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiGroupBox1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiGroupBox1.Name = "uiGroupBox1";
            this.uiGroupBox1.Padding = new System.Windows.Forms.Padding(0, 32, 0, 0);
            this.uiTableLayoutPanel1.SetRowSpan(this.uiGroupBox1, 2);
            this.uiGroupBox1.Size = new System.Drawing.Size(292, 919);
            this.uiGroupBox1.TabIndex = 3;
            this.uiGroupBox1.Text = "Control";
            this.uiGroupBox1.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chkEnableMorphology
            // 
            this.chkEnableMorphology.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkEnableMorphology.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.chkEnableMorphology.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.chkEnableMorphology.Location = new System.Drawing.Point(3, 311);
            this.chkEnableMorphology.MinimumSize = new System.Drawing.Size(1, 1);
            this.chkEnableMorphology.Name = "chkEnableMorphology";
            this.chkEnableMorphology.Size = new System.Drawing.Size(150, 29);
            this.chkEnableMorphology.TabIndex = 9;
            this.chkEnableMorphology.Text = "EnableMorphology";
            // 
            // chkEnableBlur
            // 
            this.chkEnableBlur.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkEnableBlur.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.chkEnableBlur.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.chkEnableBlur.Location = new System.Drawing.Point(3, 276);
            this.chkEnableBlur.MinimumSize = new System.Drawing.Size(1, 1);
            this.chkEnableBlur.Name = "chkEnableBlur";
            this.chkEnableBlur.Size = new System.Drawing.Size(150, 29);
            this.chkEnableBlur.TabIndex = 8;
            this.chkEnableBlur.Text = "EnableBlur";
            // 
            // chkInvertThreshold
            // 
            this.chkInvertThreshold.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkInvertThreshold.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.chkInvertThreshold.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.chkInvertThreshold.Location = new System.Drawing.Point(3, 241);
            this.chkInvertThreshold.MinimumSize = new System.Drawing.Size(1, 1);
            this.chkInvertThreshold.Name = "chkInvertThreshold";
            this.chkInvertThreshold.Size = new System.Drawing.Size(150, 29);
            this.chkInvertThreshold.TabIndex = 7;
            this.chkInvertThreshold.Text = "InvertThreshold";
            // 
            // UIlebal
            // 
            this.UIlebal.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.UIlebal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.UIlebal.Location = new System.Drawing.Point(10, 163);
            this.UIlebal.Name = "UIlebal";
            this.UIlebal.Size = new System.Drawing.Size(55, 16);
            this.UIlebal.TabIndex = 6;
            this.UIlebal.Text = "Info :";
            // 
            // lblMinArea
            // 
            this.lblMinArea.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblMinArea.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblMinArea.Location = new System.Drawing.Point(10, 125);
            this.lblMinArea.Name = "lblMinArea";
            this.lblMinArea.Size = new System.Drawing.Size(72, 21);
            this.lblMinArea.TabIndex = 5;
            this.lblMinArea.Text = "MinArea";
            // 
            // lblThreshold
            // 
            this.lblThreshold.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblThreshold.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblThreshold.Location = new System.Drawing.Point(10, 83);
            this.lblThreshold.Name = "lblThreshold";
            this.lblThreshold.Size = new System.Drawing.Size(72, 16);
            this.lblThreshold.TabIndex = 4;
            this.lblThreshold.Text = "Threshold";
            // 
            // numMinArea
            // 
            this.numMinArea.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.numMinArea.DoubleStep = 1D;
            this.numMinArea.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.numMinArea.Location = new System.Drawing.Point(89, 117);
            this.numMinArea.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numMinArea.MinimumSize = new System.Drawing.Size(1, 16);
            this.numMinArea.Name = "numMinArea";
            this.numMinArea.Padding = new System.Windows.Forms.Padding(5);
            this.numMinArea.ShowText = false;
            this.numMinArea.Size = new System.Drawing.Size(150, 29);
            this.numMinArea.TabIndex = 3;
            this.numMinArea.Text = "0";
            this.numMinArea.TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
            this.numMinArea.Type = Sunny.UI.UITextBox.UIEditType.Integer;
            this.numMinArea.Watermark = "";
            // 
            // pictureBox
            // 
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox.Location = new System.Drawing.Point(5, 32);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(1044, 882);
            this.pictureBox.TabIndex = 4;
            this.pictureBox.TabStop = false;
            // 
            // uiGroupBox2
            // 
            this.uiGroupBox2.Controls.Add(this.pictureBox);
            this.uiGroupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiGroupBox2.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.uiGroupBox2.Location = new System.Drawing.Point(304, 5);
            this.uiGroupBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiGroupBox2.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiGroupBox2.Name = "uiGroupBox2";
            this.uiGroupBox2.Padding = new System.Windows.Forms.Padding(5, 32, 5, 5);
            this.uiTableLayoutPanel1.SetRowSpan(this.uiGroupBox2, 2);
            this.uiGroupBox2.Size = new System.Drawing.Size(1054, 919);
            this.uiGroupBox2.TabIndex = 5;
            this.uiGroupBox2.Text = "Picture";
            this.uiGroupBox2.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uiTableLayoutPanel1
            // 
            this.uiTableLayoutPanel1.ColumnCount = 2;
            this.uiTableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 300F));
            this.uiTableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel1.Controls.Add(this.uiGroupBox1, 0, 0);
            this.uiTableLayoutPanel1.Controls.Add(this.uiGroupBox2, 1, 0);
            this.uiTableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTableLayoutPanel1.Location = new System.Drawing.Point(0, 35);
            this.uiTableLayoutPanel1.Name = "uiTableLayoutPanel1";
            this.uiTableLayoutPanel1.RowCount = 2;
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel1.Size = new System.Drawing.Size(1362, 929);
            this.uiTableLayoutPanel1.TabIndex = 6;
            this.uiTableLayoutPanel1.TagString = null;
            // 
            // lblInfo
            // 
            this.lblInfo.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblInfo.Location = new System.Drawing.Point(60, 163);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(179, 16);
            this.lblInfo.TabIndex = 10;
            this.lblInfo.Text = "Message";
            // 
            // GBlobWinFormExample
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1362, 964);
            this.Controls.Add(this.uiTableLayoutPanel1);
            this.Name = "GBlobWinFormExample";
            this.Text = "GBlobWinFormExample";
            this.ZoomScaleRect = new System.Drawing.Rectangle(15, 15, 800, 450);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.GBlobWinFormExample_FormClosed);
            this.uiGroupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.uiGroupBox2.ResumeLayout(false);
            this.uiTableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UIButton btnLoadImage;
        private Sunny.UI.UIButton btnRunProcess;
        private Sunny.UI.UIUpDownTextBox numThreshold;
        private Sunny.UI.UIGroupBox uiGroupBox1;
        private Sunny.UI.UIUpDownTextBox numMinArea;
        private Sunny.UI.UILabel UIlebal;
        private Sunny.UI.UILabel lblMinArea;
        private Sunny.UI.UILabel lblThreshold;
        private Sunny.UI.UICheckBox chkEnableMorphology;
        private Sunny.UI.UICheckBox chkEnableBlur;
        private Sunny.UI.UICheckBox chkInvertThreshold;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel1;
        private Sunny.UI.UIGroupBox uiGroupBox2;
        private System.Windows.Forms.PictureBox pictureBox;
        private Sunny.UI.UILabel lblInfo;
    }
}