using GVision.ROI.Models;
using GVision.Viewer.Enums;
using GVision.Viewer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;


namespace GVision.Viewer.Controls
{
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

        private Point _currentMousePoint;
        private PointF _currentImagePoint;
        private bool _hasMousePoint;

        private bool _isRoiRotating = false;
        private PointF _roiRotateCenterImagePoint;
        private float _roiRotateStartMouseAngle;
        private float _roiRotateStartRoiAngle;

        private readonly Stopwatch _dragRenderStopwatch = new Stopwatch();

        private const float MinZoom = 0.05f;
        private const float MaxZoom = 50.0f;
        private const float ZoomFactor = 1.15f;
        private const int DragRenderIntervalMs = 16;

        private GRoiRegion _editableRoi;
        private bool _isRoiEditing;
        private RoiEditMode _roiEditMode = RoiEditMode.None;

        private PointF _roiStartImagePoint;
        private Rectangle _roiStartBounds;
        private float _roiStartAngle;
        private string _roiOverlayInfoText = string.Empty;

        private const int RoiHitRange = 8;
        private const int RoiMinSize = 10;
        private const float RoiRotateStep = 5f;

        #endregion

        #region Properties

        [Browsable(false)]
        public Bitmap Image
        {
            get { return _image; }
        }

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

        [Browsable(true)]
        [Category("GVision")]
        [Description("是否顯示滑鼠所在的圖片座標。")]
        public bool ShowMouseImageCoordinate { get; set; }

        [Browsable(true)]
        [Category("GVision")]
        [Description("是否在左上角顯示滑鼠位置的影像資訊。")]
        public bool ShowPixelInfoOverlay { get; set; }

        [Browsable(true)]
        [Category("GVision")]
        [Description("是否顯示 RGB 數值。")]
        public bool ShowRgbValue { get; set; }

        [Browsable(true)]
        [Category("GVision")]
        [Description("是否顯示灰階值。")]
        public bool ShowGrayValue { get; set; }

        [Browsable(true)]
        [Category("GVision")]
        [Description("是否顯示中心十字線。")]
        public bool ShowCenterCross { get; set; }

        [Browsable(true)]
        [Category("GVision")]
        [Description("是否允許使用滑鼠左鍵拖曳平移圖片。")]
        public bool EnableImagePan { get; set; }

        [Browsable(true)]
        [Category("GVision")]
        [Description("是否啟用 ROI 編輯功能。")]
        public bool EnableRoiEdit { get; set; }

        [Browsable(false)]
        public string RoiOverlayInfoText
        {
            get { return _roiOverlayInfoText; }
            set
            {
                _roiOverlayInfoText = value;
                Invalidate();
            }
        }


        public event EventHandler<GRoiRegion> RoiChanged;

        protected override Size DefaultSize
        {
            get { return new Size(400, 300); }
        }

        #endregion

        #region Events

        public event EventHandler<PointF> MouseImagePointChanged;

        public event EventHandler<PointF> ImageClicked;

        #endregion

        #region Constructor

        public GImageViewer()
        {
            DoubleBuffered = true;
            BackColor = Color.Black;

            ShowMouseImageCoordinate = true;
            ShowPixelInfoOverlay = true;
            ShowRgbValue = true;
            ShowGrayValue = true;
            ShowCenterCross = false;
            EnableImagePan = true;
            EnableRoiEdit = false;

            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);

            _dragRenderStopwatch.Start();
        }

        #endregion

        #region Public Methods

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

            _hasMousePoint = false;
            Invalidate();
        }

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
            _hasMousePoint = false;

            Invalidate();
        }

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

        public void ResetView()
        {
            _zoom = 1.0f;
            _offset = new PointF(0, 0);
            Invalidate();
        }

        public void SetOverlays(List<GOverlayItem> overlays)
        {
            _overlays.Clear();

            if (overlays != null)
                _overlays.AddRange(overlays);

            Invalidate();
        }

        public void ClearOverlays()
        {
            _overlays.Clear();
            Invalidate();
        }

        public PointF ViewerToImage(Point viewerPoint)
        {
            if (_zoom <= 0)
                return PointF.Empty;

            return new PointF(
                (viewerPoint.X - _offset.X) / _zoom,
                (viewerPoint.Y - _offset.Y) / _zoom
            );
        }

        public PointF ImageToViewer(PointF imagePoint)
        {
            return new PointF(
                imagePoint.X * _zoom + _offset.X,
                imagePoint.Y * _zoom + _offset.Y
            );
        }

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

        public void SetEditableRoi(GRoiRegion roi)
        {
            if (roi == null)
            {
                ClearEditableRoi();
                return;
            }

            _editableRoi = roi.Clone();

            if (_editableRoi.Width <= 0 || _editableRoi.Height <= 0)
            {
                _editableRoi.X = 0;
                _editableRoi.Y = 0;
                _editableRoi.Width = 100;
                _editableRoi.Height = 100;
            }

            _editableRoi.IsEnabled = true;
            EnableRoiEdit = true;
            EnableImagePan = false;

            Invalidate();
        }

        public void ClearEditableRoi()
        {
            _editableRoi = null;
            _isRoiEditing = false;
            _roiEditMode = RoiEditMode.None;

            EnableRoiEdit = false;
            EnableImagePan = true;

            Cursor = Cursors.Default;
            Invalidate();
        }

        public GRoiRegion GetEditableRoi()
        {
            if (_editableRoi == null)
                return null;

            return _editableRoi.Clone();
        }

        public void SaveCurrentView(string filePath, ImageFormat format)
        {
            if (_image == null)
                throw new InvalidOperationException("沒有圖片可以儲存");

            using (Bitmap bmp = new Bitmap(this.Width, this.Height))
            {
                this.DrawToBitmap(bmp, new Rectangle(0, 0, this.Width, this.Height));
                bmp.Save(filePath, format);
            }
        }

        #endregion

        #region Paint

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
            DrawEditableRoi(g);

            if (ShowCenterCross)
                DrawCenterCross(g);

            if (ShowPixelInfoOverlay)
                DrawPixelInfoOverlay(g);
        }

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
                        DrawRotatedRectangle(g, pen, rect, item.Angle);

                        if (!string.IsNullOrEmpty(item.Text))
                            g.DrawString(item.Text, font, brush, rect.X, rect.Y - 18);
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

        private void DrawRotatedRectangle(Graphics g, Pen pen, RectangleF rect, float angle)
        {
            if (Math.Abs(angle) < 0.001f)
            {
                g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
                return;
            }

            float centerX = rect.X + rect.Width / 2f;
            float centerY = rect.Y + rect.Height / 2f;

            GraphicsState state = g.Save();

            g.TranslateTransform(centerX, centerY);
            g.RotateTransform(angle);
            g.TranslateTransform(-centerX, -centerY);

            g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);

            g.Restore(state);
        }

        private void DrawPixelInfoOverlay(Graphics g)
        {
            if (_image == null || !_hasMousePoint)
                return;

            int x = (int)Math.Floor(_currentImagePoint.X);
            int y = (int)Math.Floor(_currentImagePoint.Y);

            if (!IsImagePointValid(x, y))
                return;

            Color color = _image.GetPixel(x, y);
            int gray = ToGray(color);

            string text = string.Format("X: {0}, Y: {1}", x, y);

            if (ShowRgbValue)
                text += string.Format("\r\nR: {0}, G: {1}, B: {2}", color.R, color.G, color.B);

            if (ShowGrayValue)
                text += string.Format("\r\nGray: {0}", gray);

            using (Font font = new Font("Consolas", 10))
            {
                SizeF textSize = g.MeasureString(text, font);

                RectangleF bgRect = new RectangleF(
                    8,
                    8,
                    textSize.Width + 12,
                    textSize.Height + 10
                );

                using (Brush bgBrush = new SolidBrush(Color.FromArgb(180, 0, 0, 0)))
                using (Brush textBrush = new SolidBrush(Color.Lime))
                using (Pen borderPen = new Pen(Color.FromArgb(220, 80, 80, 80)))
                {
                    g.FillRectangle(bgBrush, bgRect);
                    g.DrawRectangle(borderPen, bgRect.X, bgRect.Y, bgRect.Width, bgRect.Height);
                    g.DrawString(text, font, textBrush, bgRect.X + 6, bgRect.Y + 5);
                }
            }
        }

        private bool IsImagePointValid(int x, int y)
        {
            if (_image == null)
                return false;

            return x >= 0 &&
                   y >= 0 &&
                   x < _image.Width &&
                   y < _image.Height;
        }

        private int ToGray(Color color)
        {
            return (int)Math.Round(
                color.R * 0.299 +
                color.G * 0.587 +
                color.B * 0.114
            );
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

        private void DrawEditableRoi(Graphics g)
        {
            if (_editableRoi == null || !_editableRoi.IsEnabled)
                return;

            RectangleF rect = ImageRectToViewer(_editableRoi.ToRectangle());

            using (Pen pen = new Pen(Color.Lime, 2f))
            using (Brush brush = new SolidBrush(Color.Lime))
            using (Font font = new Font("Consolas", 10))
            {
                DrawRotatedRectangle(g, pen, rect, _editableRoi.Angle);

                string extraInfo = string.IsNullOrEmpty(_roiOverlayInfoText) ? string.Empty: " " + _roiOverlayInfoText;

                string text = string.Format(
                    "ROI X:{0} Y:{1} W:{2} H:{3} A:{4:0} \n{5}",
                    _editableRoi.X,
                    _editableRoi.Y,
                    _editableRoi.Width,
                    _editableRoi.Height,
                    _editableRoi.Angle,
                    extraInfo);

                float textY = rect.Y - 18;
                if (textY < 0)
                    textY = rect.Y + 4;

                g.DrawString(text, font, brush, rect.X, textY);
            }
        }



        #endregion

        #region Mouse Events

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

            UpdateMouseInfo(e.Location);

            Invalidate();

            _isInteractiveRendering = false;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            Focus();

            if (_image == null)
                return;

            if (EnableRoiEdit && _editableRoi != null)
            {
                if (HandleRoiMouseDown(e))
                    return;
            }

            if (e.Button == MouseButtons.Left && EnableImagePan)
            {
                _isDragging = true;
                _isInteractiveRendering = true;
                _lastMousePoint = e.Location;
                Cursor = Cursors.Hand;
                _dragRenderStopwatch.Restart();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            UpdateMouseInfo(e.Location);

            if (_image != null && ShowMouseImageCoordinate)
            {
                if (MouseImagePointChanged != null)
                    MouseImagePointChanged(this, _currentImagePoint);
            }

            if (EnableRoiEdit && _editableRoi != null)
            {
                if (HandleRoiMouseMove(e))
                    return;
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
            else
            {
                if (ShowPixelInfoOverlay)
                    Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (_isRoiRotating && e.Button == MouseButtons.Middle)
            {
                _isRoiRotating = false;
                _isInteractiveRendering = false;

                Capture = false;
                Cursor = Cursors.Default;

                RaiseRoiChanged();
                Invalidate();

                return;
            }

            if (_isRoiEditing)
            {
                _isRoiEditing = false;
                _roiEditMode = RoiEditMode.None;
                _isInteractiveRendering = false;

                Capture = false;
                Cursor = Cursors.Default;

                RaiseRoiChanged();
                Invalidate();
                return;
            }

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

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            _hasMousePoint = false;

            if (_isDragging)
            {
                _isDragging = false;
                _isInteractiveRendering = false;
                Cursor = Cursors.Default;
            }

            Invalidate();
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if (e.Button == MouseButtons.Left)
                FitToWindow();

            // 中鍵雙擊：ROI 角度歸零
            if (e.Button == MouseButtons.Middle)
            {
                if (EnableRoiEdit && _editableRoi != null)
                {
                    PointF imagePoint = ViewerToImage(e.Location);
                    RoiEditMode hitMode = GetRoiEditMode(imagePoint);

                    if (hitMode != RoiEditMode.None)
                    {
                        _editableRoi.Angle = 0f;

                        _isRoiRotating = false;
                        Capture = false;
                        Cursor = Cursors.Default;

                        RaiseRoiChanged();
                        Invalidate();
                        return;
                    }
                }
            }
        }

        private void UpdateMouseInfo(Point location)
        {
            _currentMousePoint = location;
            _currentImagePoint = ViewerToImage(location);
            _hasMousePoint = _image != null;
        }

        #endregion

        #region ----- ROI 滑鼠處理方法 -----

        private bool HandleRoiMouseDown(MouseEventArgs e)
        {
            PointF imagePoint = ViewerToImage(e.Location);

            // 中鍵：開始拖曳旋轉 ROI
            if (e.Button == MouseButtons.Middle)
            {
                RoiEditMode hitMode = GetRoiEditMode(imagePoint);

                if (hitMode != RoiEditMode.None)
                {
                    Rectangle roiRect = _editableRoi.ToRectangle();

                    _roiRotateCenterImagePoint = new PointF(
                        roiRect.X + roiRect.Width / 2f,
                        roiRect.Y + roiRect.Height / 2f
                    );

                    _roiRotateStartMouseAngle = GetAngleByCenter(
                        _roiRotateCenterImagePoint,
                        imagePoint
                    );

                    _roiRotateStartRoiAngle = _editableRoi.Angle;

                    _isRoiRotating = true;
                    _isInteractiveRendering = true;
                    Capture = true;
                    Cursor = Cursors.Cross;

                    return true;
                }

                return false;
            }

            // 右鍵：保留原本選單功能
            if (e.Button == MouseButtons.Right)
                return false;

            // 左鍵：移動 / 縮放 ROI
            if (e.Button != MouseButtons.Left)
                return false;

            RoiEditMode mode = GetRoiEditMode(imagePoint);

            if (mode == RoiEditMode.None)
                return false;

            _isRoiEditing = true;
            _roiEditMode = mode;
            _roiStartImagePoint = imagePoint;
            _roiStartBounds = _editableRoi.ToRectangle();
            _roiStartAngle = _editableRoi.Angle;

            _isInteractiveRendering = true;

            Capture = true;
            Cursor = GetCursorByEditMode(mode);

            return true;
        }

        private bool HandleRoiMouseMove(MouseEventArgs e)
        {
            PointF imagePoint = ViewerToImage(e.Location);

            if (_isRoiRotating)
            {
                float currentMouseAngle = GetAngleByCenter(
                    _roiRotateCenterImagePoint,
                    imagePoint
                );

                float deltaAngle = currentMouseAngle - _roiRotateStartMouseAngle;

                _editableRoi.Angle = NormalizeAngle(_roiRotateStartRoiAngle + deltaAngle);

                RaiseRoiChanged();
                Invalidate();
                return true;
            }

            if (!_isRoiEditing)
            {
                RoiEditMode mode = GetRoiEditMode(imagePoint);
                Cursor = GetCursorByEditMode(mode);
                return mode != RoiEditMode.None;
            }

            int dx = (int)Math.Round(imagePoint.X - _roiStartImagePoint.X);
            int dy = (int)Math.Round(imagePoint.Y - _roiStartImagePoint.Y);

            Rectangle newRect = _roiStartBounds;

            if (_roiEditMode == RoiEditMode.Move)
            {
                newRect.X = _roiStartBounds.X + dx;
                newRect.Y = _roiStartBounds.Y + dy;
            }
            else if (_roiEditMode == RoiEditMode.ResizeLeft)
            {
                newRect.X = _roiStartBounds.X + dx;
                newRect.Width = _roiStartBounds.Width - dx;
            }
            else if (_roiEditMode == RoiEditMode.ResizeRight)
            {
                newRect.Width = _roiStartBounds.Width + dx;
            }
            else if (_roiEditMode == RoiEditMode.ResizeTop)
            {
                newRect.Y = _roiStartBounds.Y + dy;
                newRect.Height = _roiStartBounds.Height - dy;
            }
            else if (_roiEditMode == RoiEditMode.ResizeBottom)
            {
                newRect.Height = _roiStartBounds.Height + dy;
            }

            newRect = FixRoiBounds(newRect);

            _editableRoi.X = newRect.X;
            _editableRoi.Y = newRect.Y;
            _editableRoi.Width = newRect.Width;
            _editableRoi.Height = newRect.Height;
            _editableRoi.Angle = _roiStartAngle;

            RaiseRoiChanged();

            // 這行是關鍵：拖曳過程中即時重繪
            Invalidate();

            return true;
        }


        #endregion

        #region ----- ROI helper methods ----
        private RoiEditMode GetRoiEditMode(PointF imagePoint)
        {
            if (_editableRoi == null)
                return RoiEditMode.None;

            Rectangle roiRect = _editableRoi.ToRectangle();

            int x = (int)imagePoint.X;
            int y = (int)imagePoint.Y;

            float hitRange = RoiHitRange / _zoom;
            if (hitRange < 2f)
                hitRange = 2f;

            RectangleF left = new RectangleF(
                roiRect.Left - hitRange,
                roiRect.Top,
                hitRange * 2,
                roiRect.Height);

            RectangleF right = new RectangleF(
                roiRect.Right - hitRange,
                roiRect.Top,
                hitRange * 2,
                roiRect.Height);

            RectangleF top = new RectangleF(
                roiRect.Left,
                roiRect.Top - hitRange,
                roiRect.Width,
                hitRange * 2);

            RectangleF bottom = new RectangleF(
                roiRect.Left,
                roiRect.Bottom - hitRange,
                roiRect.Width,
                hitRange * 2);

            if (left.Contains(x, y))
                return RoiEditMode.ResizeLeft;

            if (right.Contains(x, y))
                return RoiEditMode.ResizeRight;

            if (top.Contains(x, y))
                return RoiEditMode.ResizeTop;

            if (bottom.Contains(x, y))
                return RoiEditMode.ResizeBottom;

            if (roiRect.Contains(x, y))
                return RoiEditMode.Move;

            return RoiEditMode.None;
        }

        private Cursor GetCursorByEditMode(RoiEditMode mode)
        {
            if (mode == RoiEditMode.ResizeLeft || mode == RoiEditMode.ResizeRight)
                return Cursors.SizeWE;

            if (mode == RoiEditMode.ResizeTop || mode == RoiEditMode.ResizeBottom)
                return Cursors.SizeNS;

            if (mode == RoiEditMode.Move)
                return Cursors.SizeAll;

            return Cursors.Default;
        }

        private Rectangle FixRoiBounds(Rectangle rect)
        {
            if (rect.Width < RoiMinSize)
                rect.Width = RoiMinSize;

            if (rect.Height < RoiMinSize)
                rect.Height = RoiMinSize;

            if (_image == null)
                return rect;

            if (rect.X < 0)
                rect.X = 0;

            if (rect.Y < 0)
                rect.Y = 0;

            if (rect.Right > _image.Width)
                rect.X = _image.Width - rect.Width;

            if (rect.Bottom > _image.Height)
                rect.Y = _image.Height - rect.Height;

            if (rect.X < 0)
                rect.X = 0;

            if (rect.Y < 0)
                rect.Y = 0;

            return rect;
        }

        private void RaiseRoiChanged()
        {
            if (RoiChanged != null && _editableRoi != null)
            {
                RoiChanged(this, _editableRoi.Clone());
            }
        }

        private float GetAngleByCenter(PointF center, PointF point)
        {
            double dx = point.X - center.X;
            double dy = point.Y - center.Y;

            double angle = Math.Atan2(dy, dx) * 180.0 / Math.PI;

            return (float)angle;
        }

        private float NormalizeAngle(float angle)
        {
            while (angle < 0f)
                angle += 360f;

            while (angle >= 360f)
                angle -= 360f;

            return angle;
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

            menu.MenuItems.Add("Save Image", delegate
            {
                SaveCurrentViewByDialog();
            });

            menu.MenuItems.Add("-");

            menu.MenuItems.Add("Show Pixel Info", delegate
            {
                ShowPixelInfoOverlay = !ShowPixelInfoOverlay;
                Invalidate();
            });

            menu.Show(this, location);
        }

        private void SaveCurrentViewByDialog()
        {
            if (_image == null)
            {
                MessageBox.Show("目前沒有圖片可儲存");
                return;
            }

            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Title = "儲存圖片";
                dialog.Filter = "PNG Image (*.png)|*.png|JPEG Image (*.jpg)|*.jpg|Bitmap (*.bmp)|*.bmp";
                dialog.DefaultExt = "png";
                dialog.FileName = "Viewer_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");

                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    ImageFormat format = ImageFormat.Png;

                    if (dialog.FilterIndex == 2)
                        format = ImageFormat.Jpeg;
                    else if (dialog.FilterIndex == 3)
                        format = ImageFormat.Bmp;

                    SaveCurrentView(dialog.FileName, format);

                    MessageBox.Show("儲存成功");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("儲存失敗: " + ex.Message);
                }
            }
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

        private enum RoiEditMode
        {
            None,
            Move,
            ResizeLeft,
            ResizeRight,
            ResizeTop,
            ResizeBottom
        }
    }
}