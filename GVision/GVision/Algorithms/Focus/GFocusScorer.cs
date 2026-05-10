using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace GVision.Algorithms.Focus
{
    /// <summary>
    /// 清晰度分數計算器。
    /// 使用 Laplacian Variance 作為清晰度評估方式。
    /// </summary>
    public class GFocusScorer
    {
        /// <summary>
        /// 計算 ROI 清晰度分數。
        /// </summary>
        public GFocusScoreResult Calculate(Mat source, Rectangle roi, GFocusParameter parameter)
        {
            GFocusScoreResult result = new GFocusScoreResult();

            if (source == null || source.IsEmpty)
            {
                result.Message = "Source image is null or empty.";
                return result;
            }

            if (parameter == null)
                parameter = new GFocusParameter();

            Rectangle safeRoi = GetSafeRoi(source, roi);

            if (safeRoi.Width <= 0 || safeRoi.Height <= 0)
            {
                result.Message = "ROI is outside image range.";
                return result;
            }

            result.RoiBounds = safeRoi;
            result.ScoreMode = parameter.ScoreMode;

            try
            {
                if (parameter.ScoreMode == GFocusScoreMode.NineGrid)
                {
                    CalculateNineGrid(source, safeRoi, parameter, result);
                }
                else
                {
                    double score = CalculateSingleScore(source, safeRoi);
                    result.Score = score;
                }

                result.Success = true;
                result.IsPass = result.Score >= parameter.MinScore;
                result.Message = "OK";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.IsPass = false;
                result.Score = 0;
                result.Message = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 計算單一 ROI 清晰度分數。
        /// </summary>
        public double CalculateSingleScore(Mat source, Rectangle roi)
        {
            Rectangle safeRoi = GetSafeRoi(source, roi);

            if (safeRoi.Width <= 0 || safeRoi.Height <= 0)
                return 0;

            using (Mat roiMat = new Mat(source, safeRoi))
            using (Mat gray = ConvertToGray(roiMat))
            using (Mat laplacian = new Mat())
            {
                CvInvoke.Laplacian(gray, laplacian, DepthType.Cv64F);

                MCvScalar mean = new MCvScalar();
                MCvScalar stddev = new MCvScalar();

                CvInvoke.MeanStdDev(laplacian, ref mean, ref stddev);

                double score = stddev.V0 * stddev.V0;
                return score;
            }
        }

        /// <summary>
        /// 計算九宮格清晰度分數。
        /// </summary>
        private void CalculateNineGrid(
            Mat source,
            Rectangle roi,
            GFocusParameter parameter,
            GFocusScoreResult result)
        {
            List<GFocusBlockResult> blocks = new List<GFocusBlockResult>();

            int blockWidth = roi.Width / 3;
            int blockHeight = roi.Height / 3;

            if (blockWidth <= 0 || blockHeight <= 0)
            {
                result.Score = CalculateSingleScore(source, roi);
                return;
            }

            double totalScore = 0;
            double totalWeight = 0;

            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    Rectangle blockRect = new Rectangle(
                        roi.X + col * blockWidth,
                        roi.Y + row * blockHeight,
                        blockWidth,
                        blockHeight);

                    if (col == 2)
                        blockRect.Width = roi.Right - blockRect.X;

                    if (row == 2)
                        blockRect.Height = roi.Bottom - blockRect.Y;

                    double weight = GetBlockWeight(row, col, parameter);
                    double score = CalculateSingleScore(source, blockRect);

                    GFocusBlockResult block = new GFocusBlockResult();
                    block.Row = row;
                    block.Column = col;
                    block.Bounds = blockRect;
                    block.Score = score;
                    block.Weight = weight;

                    blocks.Add(block);

                    totalScore += score * weight;
                    totalWeight += weight;
                }
            }

            result.Blocks = blocks;

            if (totalWeight <= 0)
                result.Score = 0;
            else
                result.Score = totalScore / totalWeight;
        }

        /// <summary>
        /// 取得九宮格權重。
        /// </summary>
        private double GetBlockWeight(int row, int col, GFocusParameter parameter)
        {
            if (parameter == null)
                return 1.0;

            if (!parameter.UseCenterWeight)
                return 1.0;

            if (row == 1 && col == 1)
                return parameter.CenterWeight;

            return 1.0;
        }

        /// <summary>
        /// 將影像轉成灰階。
        /// </summary>
        private Mat ConvertToGray(Mat source)
        {
            Mat gray = new Mat();

            if (source.NumberOfChannels == 1)
            {
                source.CopyTo(gray);
            }
            else if (source.NumberOfChannels == 3)
            {
                CvInvoke.CvtColor(source, gray, ColorConversion.Bgr2Gray);
            }
            else if (source.NumberOfChannels == 4)
            {
                CvInvoke.CvtColor(source, gray, ColorConversion.Bgra2Gray);
            }
            else
            {
                source.CopyTo(gray);
            }

            return gray;
        }

        /// <summary>
        /// 取得不超出影像範圍的 ROI。
        /// </summary>
        private Rectangle GetSafeRoi(Mat source, Rectangle roi)
        {
            if (source == null || source.IsEmpty)
                return Rectangle.Empty;

            Rectangle imageRect = new Rectangle(0, 0, source.Width, source.Height);
            Rectangle safeRoi = Rectangle.Intersect(imageRect, roi);

            return safeRoi;
        }
    }
}
