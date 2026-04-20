using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GVision.Models;
using GVision.ROI;

namespace GVision.Algorithms.Blob
{
    /// <summary>
    /// Blob 評估器。
    /// 將 BlobFeature 轉換成對外的 GDefectResult。
    /// </summary>
    public class GBlobEvaluator
    {
        /// <summary>
        /// 將 Blob 特徵評估為缺陷結果。
        /// </summary>
        public virtual List<GDefectResult> Evaluate(List<GBlobFeature> features, Rectangle validRoi, GBlobParameter parameter)
        {
            List<GDefectResult> defects = new List<GDefectResult>();

            if (features == null || features.Count == 0)
                return defects;

            for (int i = 0; i < features.Count; i++)
            {
                GBlobFeature feature = features[i];

                if (!IsMatch(feature, parameter))
                    continue;

                GDefectResult defect = new GDefectResult();
                defect.DefectType = "Blob";
                defect.BoundingBox = GRoiHelper.ToGlobal(feature.BoundingBox, validRoi);
                defect.Center = GRoiHelper.ToGlobal(feature.Center, validRoi);
                defect.Area = feature.Area;
                defect.Width = feature.Width;
                defect.Height = feature.Height;
                defect.Score = feature.Score;
                defect.Remark = feature.Remark;

                defects.Add(defect);
            }

            return defects;
        }

        /// <summary>
        /// Blob 條件判定。
        /// </summary>
        protected virtual bool IsMatch(GBlobFeature feature, GBlobParameter parameter)
        {
            if (feature == null)
                return false;

            if (feature.Area < parameter.MinArea)
                return false;

            if (feature.Area > parameter.MaxArea)
                return false;

            if (feature.Width < parameter.MinWidth)
                return false;

            if (feature.Height < parameter.MinHeight)
                return false;

            return true;
        }
    }
}
