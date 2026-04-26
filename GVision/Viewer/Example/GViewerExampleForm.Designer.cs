namespace GVision.Viewer.Example
{
    partial class GViewerExampleForm
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
            this.gImageViewer1 = new GVision.Viewer.Controls.GImageViewer();
            this.SuspendLayout();
            // 
            // gImageViewer1
            // 
            this.gImageViewer1.BackColor = System.Drawing.Color.Black;
            this.gImageViewer1.Location = new System.Drawing.Point(54, 40);
            this.gImageViewer1.Name = "gImageViewer1";
            this.gImageViewer1.ShowCenterCross = false;
            this.gImageViewer1.ShowMouseImageCoordinate = true;
            this.gImageViewer1.Size = new System.Drawing.Size(400, 300);
            this.gImageViewer1.TabIndex = 0;
            this.gImageViewer1.Text = "gImageViewer1";
            this.gImageViewer1.Zoom = 1F;
            // 
            // GViewerExampleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.gImageViewer1);
            this.Name = "GViewerExampleForm";
            this.Text = "GViewerExampleForm";
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.GImageViewer gImageViewer1;
    }
}