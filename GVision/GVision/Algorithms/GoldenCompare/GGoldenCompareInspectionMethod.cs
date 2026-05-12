using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

using GVision.Core;
using GVision.Models;
using GVision.Preprocessing;
using GVision.ROI.Core;

namespace GVision.Algorithms.GoldenCompare
{
    /// <summary>
    /// GoldenCompare Golden 圖比對方法。
    ///
    /// 用途：
    /// 將待測影像的 ROI 區域與預先存放的 Golden 參考圖做差分比對，
    /// 輸出相似度分數（0~100）並標示差異缺陷區域。
    ///
    /// 演算法流程：
    /// 1. 裁切 ROI、灰階轉換
    /// 2. 載入 Golden 圖、灰階轉換
    /// 3. 若啟用 NormalizeSize，將 source 縮放至 Golden 尺寸
    /// 4. 計算絕對差分圖 + 相似度分數
    /// 5. 找出差分缺陷區域
    /// 6. 與 ScoreThreshold 比對判定 OK / NG
    /// </summary>
    public class GGoldenCompareInspectionMethod : GInspectionMethodBase
    {
        public override string Name
        {
            get { return "GoldenCompare"; }
        }

        public override GInspectionResult Inspect(GInspectionRequest request)
        {
            Mat sourceCrop = null;
            Mat sourceGray = null;
            Mat sourceNorm = null;
            Mat goldenMat = null;
            Mat goldenGray = null;
            Mat diffMat = null;
            Mat diffBgr = null;
            Mat overlayMat = null;
            Mat binaryDiffDebug = null;

            GInspectionResult result = new GInspectionResult();

            try
            {
                // Step 1：基礎驗證
                ValidateRequest(request);

                // Step 2：取得並驗證參數
                GGoldenCompareParameter parameter = GetParameter<GGoldenCompareParameter>(request);
                ValidateGoldenCompareParameter(parameter);

                // Step 4：驗證 Golden 圖路徑
                if (string.IsNullOrEmpty(parameter.GoldenImagePath))
                    return FailResult("GoldenImagePath is empty.");

                if (!File.Exists(parameter.GoldenImagePath))
                    return FailResult(string.Format("Golden image not found: {0}", parameter.GoldenImagePath));

                bool enableDebug = request.EnableDebugImage || parameter.EnableDebugImages;

                Size imageSize = request.SourceImage.Size;

                // Step 6：取得有效 ROI
                Rectangle validRoi = GRoiHelper.GetValidRoi(request.Roi, imageSize);

                // Step 7：裁切 Source + 灰階
                sourceCrop = new Mat(request.SourceImage, validRoi);
                sourceGray = GGrayPreprocessor.Process(sourceCrop);

                // Step 8：載入 Golden + 灰階
                goldenMat = CvInvoke.Imread(parameter.GoldenImagePath, ImreadModes.AnyColor);
                if (goldenMat == null || goldenMat.IsEmpty)
                    return FailResult("Failed to load golden image.");

                goldenGray = GGrayPreprocessor.Process(goldenMat);

                // Step 9：尺寸正規化
                if (parameter.NormalizeSize && sourceGray.Size != goldenGray.Size)
                {
                    sourceNorm = new Mat();
                    CvInvoke.Resize(sourceGray, sourceNorm, goldenGray.Size, 0, 0, Inter.Linear);
                }
                else
                {
                    sourceNorm = sourceGray.Clone();
                }

                // Step 10：計算 diff + score
                var analyzer = new GGoldenCompareDiffAnalyzer();
                diffMat = analyzer.ComputeDiff(sourceNorm, goldenGray);
                double score = analyzer.ComputeScore(diffMat);

                // Step 11：找缺陷（座標已含 ROI 偏移，轉回原圖座標）
                List<GDefectResult> defects = analyzer.FindDefects(
                    diffMat,
                    validRoi,
                    parameter.DiffThreshold,
                    parameter.MinDefectArea);

                // Step 12：判定 OK / NG
                bool isOk = score >= parameter.ScoreThreshold;

                // Step 13：填寫 result
                // 注意：TotalDefectArea 欄位此處作為 score 的 carrier（與 Blob 用法不同）
                result.IsSuccess = true;
                result.IsOk = isOk;
                result.Message = string.Format("Score={0:F2} | {1}", score, isOk ? "OK" : "NG");
                result.Defects = defects;
                result.Statistics.DefectCount = defects.Count;
                // TotalDefectArea 暫存 score 值供上層系統讀取，非面積加總
                result.Statistics.TotalDefectArea = score;
                result.Statistics.MaxDefectArea = defects.Count > 0
                    ? defects.Max(d => d.Area)
                    : 0;

                // Step 14：ResultOverlay
                overlayMat = request.SourceImage.Clone();

                // 將差分圖轉 BGR 並半透明疊加在原圖上
                diffBgr = new Mat();
                CvInvoke.CvtColor(diffMat, diffBgr, ColorConversion.Gray2Bgr);
                CvInvoke.AddWeighted(overlayMat, 0.7, diffBgr, 0.3, 0, overlayMat);

                // 每個缺陷畫紅框
                foreach (GDefectResult defect in defects)
                {
                    if (defect.BoundingBox != Rectangle.Empty)
                        CvInvoke.Rectangle(overlayMat, defect.BoundingBox, new MCvScalar(0, 0, 255), 2);
                }

                // 顯示分數與 OK/NG
                CvInvoke.PutText(
                    overlayMat,
                    string.Format("Score={0:F1} {1}", score, isOk ? "OK" : "NG"),
                    new Point(10, 30),
                    FontFace.HersheySimplex,
                    1.0,
                    isOk ? new MCvScalar(0, 255, 0) : new MCvScalar(0, 0, 255),
                    2);

                result.ResultOverlay = overlayMat;
                overlayMat = null; // 移交 result 管理，不在 finally 釋放

                // Step 15：Debug images
                if (enableDebug)
                {
                    AddDebugImage(result, "01_SourceCrop", sourceCrop);
                    AddDebugImage(result, "02_GoldenGray", goldenGray);
                    AddDebugImage(result, "03_SourceGray", sourceNorm);
                    AddDebugImage(result, "04_Diff", diffMat);

                    // 產生 binary diff 用於 debug 顯示
                    binaryDiffDebug = new Mat();
                    CvInvoke.Threshold(diffMat, binaryDiffDebug, parameter.DiffThreshold, 255, ThresholdType.Binary);
                    AddDebugImage(result, "05_BinaryDiff", binaryDiffDebug);

                    AddDebugImage(result, "06_ResultOverlay", result.ResultOverlay);
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.IsOk = false;
                result.Message = ex.Message;
            }
            finally
            {
                // Step 16：釋放所有中間 Mat
                if (sourceCrop != null) sourceCrop.Dispose();
                if (sourceGray != null) sourceGray.Dispose();
                if (sourceNorm != null) sourceNorm.Dispose();
                if (goldenMat != null) goldenMat.Dispose();
                if (goldenGray != null) goldenGray.Dispose();
                if (diffMat != null) diffMat.Dispose();
                if (diffBgr != null) diffBgr.Dispose();
                if (overlayMat != null) overlayMat.Dispose();
                if (binaryDiffDebug != null) binaryDiffDebug.Dispose();
            }

            return result;
        }

        // ── Validation ────────────────────────────────────────────────────────

        protected virtual void ValidateGoldenCompareParameter(GGoldenCompareParameter parameter)
        {
            ValidateParameter(parameter);
            parameter.Validate();
        }

        // ── Helper methods ────────────────────────────────────────────────────

        /// <summary>
        /// 建立失敗結果，IsSuccess = false。
        /// </summary>
        private GInspectionResult FailResult(string message)
        {
            return new GInspectionResult
            {
                IsSuccess = false,
                IsOk = false,
                Message = message
            };
        }

        /// <summary>
        /// 將 Mat 加入 result.DebugImages。
        /// 若 mat 為 null 或 Empty 則忽略。
        /// </summary>
        private void AddDebugImage(GInspectionResult result, string name, Mat mat)
        {
            if (result == null || mat == null || mat.IsEmpty)
                return;

            Bitmap bitmap = mat.ToBitmap();
            result.DebugImages.Add(new GDebugImage(name, new Bitmap(bitmap)));
            bitmap.Dispose();
        }
    }
}
