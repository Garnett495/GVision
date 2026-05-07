using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


using GVision.Viewer.Controls;
using GVision.Viewer.Enums;
using GVision.Viewer.Models;

namespace GVision.Viewer.Example
{
    /// <summary>
    /// GImageViewer 使用範例。
    /// 
    /// 功能：
    /// 1. 載入圖片
    /// 2. 清除圖片
    /// 3. Fit to Window
    /// 4. 顯示 Overlay
    /// 5. 取得滑鼠所在圖片座標
    /// </summary>
    public partial class GViewerExampleForm : Form
    {
        private GImageViewer _viewer;
        private Button _btnLoadImage;
        private Button _btnFit;
        private Button _btnClear;
        private Button _btnShowOverlay;
        private Label _lblCoordinate;

        public GViewerExampleForm()
        {
            InitializeComponent();
            InitViewerExampleUI();
        }

        /// <summary>
        /// 建立範例 UI。
        /// </summary>
        private void InitViewerExampleUI()
        {
            Text = "GVision Viewer Example";
            Width = 1000;
            Height = 700;

            _viewer = new GImageViewer();
            _viewer.Dock = DockStyle.Fill;
            _viewer.BackColor = Color.Black;
            _viewer.MouseImagePointChanged += Viewer_MouseImagePointChanged;
            _viewer.ImageClicked += Viewer_ImageClicked;

            Panel topPanel = new Panel();
            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 45;

            _btnLoadImage = new Button();
            _btnLoadImage.Text = "Load Image";
            _btnLoadImage.Left = 10;
            _btnLoadImage.Top = 8;
            _btnLoadImage.Width = 100;
            _btnLoadImage.Click += BtnLoadImage_Click;

            _btnFit = new Button();
            _btnFit.Text = "Fit";
            _btnFit.Left = 120;
            _btnFit.Top = 8;
            _btnFit.Width = 80;
            _btnFit.Click += BtnFit_Click;

            _btnClear = new Button();
            _btnClear.Text = "Clear";
            _btnClear.Left = 210;
            _btnClear.Top = 8;
            _btnClear.Width = 80;
            _btnClear.Click += BtnClear_Click;

            _btnShowOverlay = new Button();
            _btnShowOverlay.Text = "Show Overlay";
            _btnShowOverlay.Left = 300;
            _btnShowOverlay.Top = 8;
            _btnShowOverlay.Width = 110;
            _btnShowOverlay.Click += BtnShowOverlay_Click;

            _lblCoordinate = new Label();
            _lblCoordinate.Text = "X: -, Y: -";
            _lblCoordinate.Left = 430;
            _lblCoordinate.Top = 13;
            _lblCoordinate.Width = 250;
            _lblCoordinate.ForeColor = Color.Black;

            topPanel.Controls.Add(_btnLoadImage);
            topPanel.Controls.Add(_btnFit);
            topPanel.Controls.Add(_btnClear);
            topPanel.Controls.Add(_btnShowOverlay);
            topPanel.Controls.Add(_lblCoordinate);

            Controls.Add(_viewer);
            Controls.Add(topPanel);
        }

        /// <summary>
        /// 載入圖片範例。
        /// </summary>
        private void BtnLoadImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png";

                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                using (Bitmap bmp = new Bitmap(dialog.FileName))
                {
                    _viewer.LoadImage(bmp);
                }
            }
        }

        /// <summary>
        /// 將圖片縮放至符合 Viewer 視窗大小。
        /// </summary>
        private void BtnFit_Click(object sender, EventArgs e)
        {
            _viewer.FitToWindow();
        }

        /// <summary>
        /// 清除圖片與 Overlay。
        /// </summary>
        private void BtnClear_Click(object sender, EventArgs e)
        {
            _viewer.ClearImage();
        }

        /// <summary>
        /// 顯示 Overlay 範例。
        /// Rect 使用 Image 座標，不是畫面座標。
        /// </summary>
        private void BtnShowOverlay_Click(object sender, EventArgs e)
        {
            List<GOverlayItem> overlays = new List<GOverlayItem>();

            overlays.Add(new GOverlayItem
            {
                Type = GOverlayType.Rectangle,
                Rect = new RectangleF(100, 80, 200, 120),
                Text = "ROI 1",
                Color = Color.Lime,
                LineWidth = 2,
                IsVisible = true
            });

            overlays.Add(new GOverlayItem
            {
                Type = GOverlayType.Rectangle,
                Rect = new RectangleF(380, 220, 160, 100),
                Text = "Blob Area: 3500",
                Color = Color.Red,
                LineWidth = 2,
                IsVisible = true
            });

            _viewer.SetOverlays(overlays);
        }

        /// <summary>
        /// 顯示目前滑鼠所在的圖片座標。
        /// </summary>
        private void Viewer_MouseImagePointChanged(object sender, PointF e)
        {
            _lblCoordinate.Text = string.Format("X: {0:0.0}, Y: {1:0.0}", e.X, e.Y);
        }

        /// <summary>
        /// 點擊 Viewer 時取得圖片座標。
        /// </summary>
        private void Viewer_ImageClicked(object sender, PointF e)
        {
            Text = string.Format("GVision Viewer Example - Click X:{0:0.0}, Y:{1:0.0}", e.X, e.Y);
        }
    }
}
