using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using GVision.Viewer.Enums;
using GVision.Viewer.Models;

namespace GVision.Viewer.Controls
{
    /// <summary>
    /// GVision 影像顯示控制項。
    /// 
    /// 功能：
    /// 1. 顯示 Bitmap 圖片
    /// 2. 滑鼠滾輪縮放
    /// 3. 滑鼠左鍵拖曳平移
    /// 4. Fit to Window
    /// 5. Overlay 顯示，例如 ROI / Blob / Text
    /// 6. Viewer 座標與 Image 座標轉換
    /// 7. 拖曳時降低繪圖品質，提升操作流暢度
    /// </summary>
    [ToolboxItem(true)]
    public class GImageViewer : Control
    {
        #region Fields

        private Bitmap _image;
        private readonly List<GOverlayItem> _overlays = new List<GOverlayItem>();

        private float _zoom = 1.0f;
        private PointF _offset = new PointF(0, 0);

        private bool _isDragging;
        private bool _isInteractiveRendering;
        private Point _lastMousePoint;

        /// <summary>
        /// 控制拖曳時的重繪頻率，避免滑鼠事件太密集造成卡頓。
        /// </summary>
        private readonly Stopwatch _dragRenderStopwatch = new Stopwatch();

        private const float MinZoom = 0.05f;
        private const float MaxZoom = 50.0f;
        private const float ZoomFactor = 1.15f;
        private const int DragRenderIntervalMs = 16; // 約 60 FPS

        #endregion

        #region Properties

        /// <summary>
        /// 目前 Viewer 顯示的圖片。
        /// </summary>
        [Browsable(false)]
        public Bitmap Image
        {
            get { return _image; }
        }

        /// <summary>
        /// 目前縮放倍率，1.0 代表原圖大小。
        /// </summary>
        [Browsable(true)]
        [Category("GVision")]
        [Description("目前 Viewer 的縮放倍率。")]
        public float Zoom
        {
            get { return _zoom; }
            set
            {
                if (value < MinZoom) value = MinZoom;
                if (value > MaxZoom) value = MaxZoom;

                _zoom = value;
                Invalidate();
            }
        }

        /// <summary>
        /// 是否回傳滑鼠目前所在的圖片座標。
        /// </summary>
        [Browsable(true)]
        [Category("GVision")]
        [Description("是否顯示滑鼠所在的圖片座標。")]
        public bool ShowMouseImageCoordinate { get; set; }

        /// <summary>
        /// 是否顯示 Viewer 中心十字線，主要用於 Debug。
        /// </summary>
        [Browsable(true)]
        [Category("GVision")]
        [Description("是否顯示中心十字線。")]
        public bool ShowCenterCross { get; set; }

        protected override Size DefaultSize
        {
            get { return new Size(400, 300); }
        }

        #endregion

        #region Events

        /// <summary>
        /// 滑鼠移動時，回傳對應的圖片座標。
        /// </summary>
        public event EventHandler<PointF> MouseImagePointChanged;

        /// <summary>
        /// 滑鼠點擊時，回傳對應的圖片座標。
        /// </summary>
        public event EventHandler<PointF> ImageClicked;

        #endregion

        #region Constructor

        public GImageViewer()
        {
            DoubleBuffered = true;
            BackColor = Color.Black;

            ShowMouseImageCoordinate = true;
            ShowCenterCross = false;

            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);

            _dragRenderStopwatch.Start();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 載入圖片。
        /// 傳入的 Bitmap 會被複製一份，外部可自行 Dispose 原始 Bitmap。
        /// </summary>
        public void LoadImage(Bitmap image)
        {
            if (_image != null)
            {
                _image.Dispose();
                _image = null;
            }

            if (image != null)
            {
                _image = new Bitmap(image);
                FitToWindow();
            }

            Invalidate();
        }

        /// <summary>
        /// 清除圖片與 Overlay，並重置 Viewer 狀態。
        /// </summary>
        public void ClearImage()
        {
            if (_image != null)
            {
                _image.Dispose();
                _image = null;
            }

            _overlays.Clear();
            _zoom = 1.0f;
            _offset = new PointF(0, 0);

            Invalidate();
        }

        /// <summary>
        /// 將圖片縮放並置中到 Viewer 範圍內。
        /// </summary>
        public void FitToWindow()
        {
            if (_image == null || Width <= 0 || Height <= 0)
                return;

            float scaleX = (float)Width / _image.Width;
            float scaleY = (float)Height / _image.Height;

            _zoom = Math.Min(scaleX, scaleY);

            float displayWidth = _image.Width * _zoom;
            float displayHeight = _image.Height * _zoom;

            _offset = new PointF(
                (Width - displayWidth) / 2f,
                (Height - displayHeight) / 2f
            );

            Invalidate();
        }

        /// <summary>
        /// 重置為原始比例與原始位置。
        /// </summary>
        public void ResetView()
        {
            _zoom = 1.0f;
            _offset = new PointF(0, 0);
            Invalidate();
        }

        /// <summary>
        /// 設定 Overlay 清單。
        /// Overlay 座標必須使用 Image 座標，不是 Viewer 畫面座標。
        /// </summary>
        public void SetOverlays(List<GOverlayItem> overlays)
        {
            _overlays.Clear();

            if (overlays != null)
            {
                _overlays.AddRange(overlays);
            }

            Invalidate();
        }

        /// <summary>
        /// 清除所有 Overlay。
        /// </summary>
        public void ClearOverlays()
        {
            _overlays.Clear();
            Invalidate();
        }

        /// <summary>
        /// Viewer 座標轉 Image 座標。
        /// ROI 設定與滑鼠取點時會用到。
        /// </summary>
        public PointF ViewerToImage(Point viewerPoint)
        {
            if (_zoom <= 0)
                return PointF.Empty;

            return new PointF(
                (viewerPoint.X - _offset.X) / _zoom,
                (viewerPoint.Y - _offset.Y) / _zoom
            );
        }

        /// <summary>
        /// Image 座標轉 Viewer 座標。
        /// Overlay 繪製時會用到。
        /// </summary>
        public PointF ImageToViewer(PointF imagePoint)
        {
            return new PointF(
                imagePoint.X * _zoom + _offset.X,
                imagePoint.Y * _zoom + _offset.Y
            );
        }

        /// <summary>
        /// Image Rectangle 轉 Viewer Rectangle。
        /// </summary>
        public RectangleF ImageRectToViewer(RectangleF imageRect)
        {
            PointF p = ImageToViewer(new PointF(imageRect.X, imageRect.Y));

            return new RectangleF(
                p.X,
                p.Y,
                imageRect.Width * _zoom,
                imageRect.Height * _zoom
            );
        }

        #endregion

        #region Paint

        /// <summary>
        /// Viewer 核心繪圖流程。
        /// 拖曳 / 縮放時會降低繪圖品質，放開後恢復高品質。
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.Clear(BackColor);

            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                DrawDesignMode(g);
                return;
            }

            if (_image == null)
            {
                DrawEmptyViewer(g);
                return;
            }

            ApplyRenderQuality(g);
            DrawImage(g);
            DrawOverlays(g);

            if (ShowCenterCross)
            {
                DrawCenterCross(g);
            }
        }

        /// <summary>
        /// 根據目前是否正在互動操作，切換繪圖品質。
        /// </summary>
        private void ApplyRenderQuality(Graphics g)
        {
            if (_isInteractiveRendering)
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.SmoothingMode = SmoothingMode.None;
                g.PixelOffsetMode = PixelOffsetMode.Half;
            }
            else
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            }
        }

        private void DrawImage(Graphics g)
        {
            RectangleF destRect = new RectangleF(
                _offset.X,
                _offset.Y,
                _image.Width * _zoom,
                _image.Height * _zoom
            );

            g.DrawImage(_image, destRect);
        }

        /// <summary>
        /// 繪製 Overlay，例如 ROI、Blob 框、中心點、文字。
        /// </summary>
        private void DrawOverlays(Graphics g)
        {
            foreach (GOverlayItem item in _overlays)
            {
                if (item == null || !item.IsVisible)
                    continue;

                using (Pen pen = new Pen(item.Color, item.LineWidth))
                using (Brush brush = new SolidBrush(item.Color))
                using (Font font = new Font("Arial", 10))
                {
                    if (item.Type == GOverlayType.Rectangle)
                    {
                        RectangleF rect = ImageRectToViewer(item.Rect);
                        g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);

                        if (!string.IsNullOrEmpty(item.Text))
                        {
                            g.DrawString(item.Text, font, brush, rect.X, rect.Y - 18);
                        }
                    }
                    else if (item.Type == GOverlayType.Cross)
                    {
                        PointF p = ImageToViewer(item.Point);
                        float size = 8;

                        g.DrawLine(pen, p.X - size, p.Y, p.X + size, p.Y);
                        g.DrawLine(pen, p.X, p.Y - size, p.X, p.Y + size);
                    }
                    else if (item.Type == GOverlayType.Text)
                    {
                        PointF p = ImageToViewer(item.Point);
                        g.DrawString(item.Text, font, brush, p);
                    }
                }
            }
        }

        private void DrawDesignMode(Graphics g)
        {
            using (Pen pen = new Pen(Color.Gray, 1))
            using (Brush brush = new SolidBrush(Color.Gray))
            {
                g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
                g.DrawString("GVision GImageViewer", Font, brush, new PointF(10, 10));
            }
        }

        private void DrawEmptyViewer(Graphics g)
        {
            using (Brush brush = new SolidBrush(Color.Gray))
            {
                g.DrawString("No Image", Font, brush, new PointF(10, 10));
            }
        }

        private void DrawCenterCross(Graphics g)
        {
            using (Pen pen = new Pen(Color.DarkGray, 1))
            {
                float centerX = Width / 2f;
                float centerY = Height / 2f;

                g.DrawLine(pen, centerX - 10, centerY, centerX + 10, centerY);
                g.DrawLine(pen, centerX, centerY - 10, centerX, centerY + 10);
            }
        }

        #endregion

        #region Mouse Events

        /// <summary>
        /// 滑鼠滾輪縮放。
        /// 重點：以滑鼠所在圖片座標為縮放中心。
        /// </summary>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (_image == null)
                return;

            _isInteractiveRendering = true;

            PointF beforeZoomImagePoint = ViewerToImage(e.Location);

            if (e.Delta > 0)
                _zoom *= ZoomFactor;
            else
                _zoom /= ZoomFactor;

            if (_zoom < MinZoom) _zoom = MinZoom;
            if (_zoom > MaxZoom) _zoom = MaxZoom;

            PointF afterZoomViewerPoint = ImageToViewer(beforeZoomImagePoint);

            _offset.X += e.X - afterZoomViewerPoint.X;
            _offset.Y += e.Y - afterZoomViewerPoint.Y;

            Invalidate();

            // 滾輪是瞬間事件，這裡直接恢復高品質，並再重繪一次。
            _isInteractiveRendering = false;
            Invalidate();
        }

        /// <summary>
        /// 開始拖曳圖片。
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            Focus();

            if (_image == null)
                return;

            if (e.Button == MouseButtons.Left)
            {
                _isDragging = true;
                _isInteractiveRendering = true;
                _lastMousePoint = e.Location;
                Cursor = Cursors.Hand;
                _dragRenderStopwatch.Restart();
            }
        }

        /// <summary>
        /// 拖曳圖片時更新 Offset。
        /// 使用重繪頻率限制，避免滑鼠事件過密造成卡頓。
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_image != null && ShowMouseImageCoordinate)
            {
                if (MouseImagePointChanged != null)
                {
                    MouseImagePointChanged(this, ViewerToImage(e.Location));
                }
            }

            if (_isDragging)
            {
                int dx = e.X - _lastMousePoint.X;
                int dy = e.Y - _lastMousePoint.Y;

                _offset.X += dx;
                _offset.Y += dy;

                _lastMousePoint = e.Location;

                if (_dragRenderStopwatch.ElapsedMilliseconds >= DragRenderIntervalMs)
                {
                    _dragRenderStopwatch.Restart();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// 結束拖曳圖片。
        /// 放開滑鼠後恢復高品質繪圖。
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.Button == MouseButtons.Left)
            {
                _isDragging = false;
                _isInteractiveRendering = false;
                Cursor = Cursors.Default;
                Invalidate();

                if (_image != null && ImageClicked != null)
                {
                    ImageClicked(this, ViewerToImage(e.Location));
                }
            }

            if (e.Button == MouseButtons.Right)
            {
                ShowContextMenu(e.Location);
            }
        }

        /// <summary>
        /// 滑鼠離開 Viewer 時，避免拖曳狀態卡住。
        /// </summary>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (_isDragging)
            {
                _isDragging = false;
                _isInteractiveRendering = false;
                Cursor = Cursors.Default;
                Invalidate();
            }
        }

        /// <summary>
        /// 雙擊左鍵時自動 Fit to Window。
        /// </summary>
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if (e.Button == MouseButtons.Left)
            {
                FitToWindow();
            }
        }

        #endregion

        #region Context Menu

        private void ShowContextMenu(Point location)
        {
            ContextMenu menu = new ContextMenu();

            menu.MenuItems.Add("Fit To Window", delegate
            {
                FitToWindow();
            });

            menu.MenuItems.Add("Reset View", delegate
            {
                ResetView();
            });

            menu.MenuItems.Add("Clear Overlay", delegate
            {
                ClearOverlays();
            });

            menu.Show(this, location);
        }

        #endregion

        #region Resize / Dispose

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_image != null)
                {
                    _image.Dispose();
                    _image = null;
                }
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}