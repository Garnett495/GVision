using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Drawing;
using Emgu.CV;

namespace GVision.Algorithms.Focus.Examples
{
    /// <summary>
    /// GFocusScorer 使用範例。
    /// </summary>
    public static class GFocusScorerExample
    {
        /// <summary>
        /// 單一 ROI 清晰度分數範例。
        /// </summary>
        public static GFocusScoreResult CalculateSingleRoiExample(Mat image)
        {
            if (image == null || image.IsEmpty)
                throw new ArgumentException("image is null or empty.");

            GFocusParameter parameter = new GFocusParameter();
            parameter.ScoreMode = GFocusScoreMode.SingleRoi;
            parameter.MinScore = 100.0;

            Rectangle roi = new Rectangle(100, 100, 300, 200);

            GFocusScorer scorer = new GFocusScorer();

            GFocusScoreResult result = scorer.Calculate(
                image,
                roi,
                parameter);

            return result;
        }

        /// <summary>
        /// 九宮格清晰度分數範例。
        /// </summary>
        public static GFocusScoreResult CalculateNineGridExample(Mat image)
        {
            if (image == null || image.IsEmpty)
                throw new ArgumentException("image is null or empty.");

            GFocusParameter parameter = new GFocusParameter();
            parameter.ScoreMode = GFocusScoreMode.NineGrid;
            parameter.MinScore = 100.0;
            parameter.UseCenterWeight = true;
            parameter.CenterWeight = 2.0;

            Rectangle roi = new Rectangle(100, 100, 300, 200);

            GFocusScorer scorer = new GFocusScorer();

            GFocusScoreResult result = scorer.Calculate(
                image,
                roi,
                parameter);

            return result;
        }

        /// <summary>
        /// 從圖片路徑載入並計算單一 ROI 分數。
        /// </summary>
        public static GFocusScoreResult CalculateSingleRoiFromFile(string imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
                throw new ArgumentException("imagePath is null or empty.");

            using (Mat image = CvInvoke.Imread(imagePath))
            {
                return CalculateSingleRoiExample(image);
            }
        }

        /// <summary>
        /// 從圖片路徑載入並計算九宮格分數。
        /// </summary>
        public static GFocusScoreResult CalculateNineGridFromFile(string imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
                throw new ArgumentException("imagePath is null or empty.");

            using (Mat image = CvInvoke.Imread(imagePath))
            {
                return CalculateNineGridExample(image);
            }
        }

        /// <summary>
        /// 顯示九宮格每一格分數。
        /// 可在 Console 或 Debug 時使用。
        /// </summary>
        public static string BuildNineGridScoreText(GFocusScoreResult result)
        {
            if (result == null)
                return string.Empty;

            if (result.Blocks == null || result.Blocks.Count == 0)
                return string.Empty;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.AppendLine("Nine Grid Focus Score");

            for (int i = 0; i < result.Blocks.Count; i++)
            {
                GFocusBlockResult block = result.Blocks[i];

                sb.AppendLine(string.Format(
                    "Block({0},{1}) Score={2:F2}, Weight={3:F1}",
                    block.Row,
                    block.Column,
                    block.Score,
                    block.Weight));
            }

            sb.AppendLine(string.Format("Final Score={0:F2}", result.Score));
            sb.AppendLine(string.Format("Result={0}", result.IsPass ? "PASS" : "FAIL"));

            return sb.ToString();
        }
    }
}
