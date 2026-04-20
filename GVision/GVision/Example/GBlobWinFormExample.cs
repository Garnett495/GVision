using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


using Sunny.UI;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using GVision.Algorithms.Blob;
using GVision.Models;


namespace GVision.Example
{
    /// <summary>
    /// 使用 SunnyUI 製作的 Blob 測試介面範例。
    /// 
    /// 說明：
    /// 1. 按鈕、Label、輸入欄位改用 SunnyUI 控制項
    /// 2. 影像顯示區暫時保留 PictureBox，方便後續加 ROI 框選功能
    /// </summary>
    public partial class GBlobWinFormExample : UIForm
    {
        private Mat _sourceImage;

        public GBlobWinFormExample()
        {
            InitializeComponent();
        }        

        /// <summary>
        /// 載入圖片。
        /// </summary>
        private void BtnLoadImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff";

                if (ofd.ShowDialog() != DialogResult.OK)
                    return;

                if (_sourceImage != null)
                {
                    _sourceImage.Dispose();
                    _sourceImage = null;
                }

                if (pictureBox.Image != null)
                {
                    pictureBox.Image.Dispose();
                    pictureBox.Image = null;
                }

                _sourceImage = CvInvoke.Imread(ofd.FileName, ImreadModes.AnyColor);

                if (_sourceImage == null || _sourceImage.IsEmpty)
                {
                    lblInfo.Text = "圖片載入失敗。";
                    return;
                }

                using (var img = _sourceImage.ToImage<Emgu.CV.Structure.Bgr, byte>())
                {
                    // 將 img 轉成 Bitmap 需使用 Bitmap 建構子
                    pictureBox.Image = img.ToBitmap();
                }
                lblInfo.Text = "圖片載入完成。";
            }
        }

        /// <summary>
        /// 執行 Blob 檢測。
        /// </summary>
        private void BtnRunBlob_Click(object sender, EventArgs e)
        {
            if (_sourceImage == null)
            {
                this.ShowWarningTip("請先載入圖片。");
                return;
            }

            GBlobParameter parameter = new GBlobParameter();
            parameter.Threshold = numThreshold.IntValue;
            parameter.MinArea = numMinArea.IntValue;
            parameter.MaxArea = 999999;
            parameter.MinWidth = 1;
            parameter.MinHeight = 1;
            parameter.InvertThreshold = chkInvertThreshold.Checked;
            parameter.EnableBlur = chkEnableBlur.Checked;
            parameter.BlurKernelSize = 3;
            parameter.EnableMorphology = chkEnableMorphology.Checked;
            parameter.MorphologyKernelSize = 3;
            parameter.EnableDebugImages = false;

            GInspectionRequest request = new GInspectionRequest();
            request.SourceImage = _sourceImage;
            request.Parameter = parameter;
            request.Roi = null;
            request.EnableDebugImage = true;
            request.ImageId = "SunnyUI_Test";

            GBlobInspectionMethod method = new GBlobInspectionMethod();
            GInspectionResult result = method.Inspect(request);

            lblInfo.Text = string.Format(
                "Success={0}, OK={1}, Defects={2}, TotalArea={3:0.##}, Message={4}",
                result.IsSuccess,
                result.IsOk,
                result.Statistics.DefectCount,
                result.Statistics.TotalDefectArea,
                result.Message);

            if (result.ResultOverlay != null)
            {
                if (pictureBox.Image != null)
                {
                    pictureBox.Image.Dispose();
                    pictureBox.Image = null;
                }

                using (var img = result.ResultOverlay.ToImage<Emgu.CV.Structure.Bgr, byte>())
                {
                    pictureBox.Image = img.ToBitmap();
                }
                result.ResultOverlay.Dispose();
            }
        }

        /// <summary>
        /// 關閉視窗時釋放資源。
        /// </summary>
        private void GBlobWinFormExample_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_sourceImage != null)
            {
                _sourceImage.Dispose();
                _sourceImage = null;
            }

            if (pictureBox.Image != null)
            {
                pictureBox.Image.Dispose();
                pictureBox.Image = null;
            }

            base.OnFormClosed(e);
        }
    }
}
