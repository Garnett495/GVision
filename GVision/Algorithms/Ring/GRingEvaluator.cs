using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using GVision.Models;

namespace GVision.Algorithms.Ring
{
    /// <summary>
    /// Ring 檢測結果評估器。
    /// </summary>
    public class GRingEvaluator
    {
        /// <summary>
        /// 根據 Ring 特徵與參數產生缺陷結果。
        /// </summary>
        public virtual List<GDefectResult> Evaluate(
            GRingFeature feature,
            Rectangle validRoi,
            GRingParameter parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException("parameter");

            List<GDefectResult> defects = new List<GDefectResult>();

            if (feature == null)
            {
                defects.Add(CreateDefect(
                    GRingDefectType.OuterCircleNotFound,
                    validRoi,
                    "Ring feature is null."));

                return defects;
            }

            if (parameter.EnableOuterCircleCheck)
                CheckOuterCircle(feature, validRoi, parameter, defects);

            if (parameter.EnableInnerCircleCheck)
                CheckInnerCircle(feature, validRoi, parameter, defects);

            if (parameter.EnableCenterOffsetCheck)
                CheckCenterOffset(feature, validRoi, parameter, defects);

            return defects;
        }

        private void CheckOuterCircle(
            GRingFeature feature,
            Rectangle validRoi,
            GRingParameter parameter,
            List<GDefectResult> defects)
        {
            if (!feature.HasOuterCircle)
            {
                defects.Add(CreateDefect(
                    GRingDefectType.OuterCircleNotFound,
                    validRoi,
                    "Outer circle not found."));

                return;
            }

            if (feature.OuterRadius < parameter.MinOuterRadius ||
                feature.OuterRadius > parameter.MaxOuterRadius)
            {
                defects.Add(CreateDefect(
                    GRingDefectType.OuterRadiusNG,
                    ToRect(feature.OuterCenter, feature.OuterRadius),
                    "Outer radius NG."));
            }

            if (feature.OuterCircularity < parameter.MinCircularity)
            {
                defects.Add(CreateDefect(
                    GRingDefectType.OuterCircularityNG,
                    ToRect(feature.OuterCenter, feature.OuterRadius),
                    "Outer circularity NG."));
            }

            if (feature.OuterRoundnessError > parameter.MaxRoundnessError)
            {
                defects.Add(CreateDefect(
                    GRingDefectType.RoundnessNG,
                    ToRect(feature.OuterCenter, feature.OuterRadius),
                    "Outer roundness NG."));
            }
        }

        private void CheckInnerCircle(
            GRingFeature feature,
            Rectangle validRoi,
            GRingParameter parameter,
            List<GDefectResult> defects)
        {
            if (!feature.HasInnerCircle)
            {
                defects.Add(CreateDefect(
                    GRingDefectType.InnerCircleNotFound,
                    validRoi,
                    "Inner circle not found."));

                return;
            }

            if (feature.InnerRadius < parameter.MinInnerRadius ||
                feature.InnerRadius > parameter.MaxInnerRadius)
            {
                defects.Add(CreateDefect(
                    GRingDefectType.InnerRadiusNG,
                    ToRect(feature.InnerCenter, feature.InnerRadius),
                    "Inner radius NG."));
            }

            if (feature.InnerCircularity < parameter.MinCircularity)
            {
                defects.Add(CreateDefect(
                    GRingDefectType.InnerCircularityNG,
                    ToRect(feature.InnerCenter, feature.InnerRadius),
                    "Inner circularity NG."));
            }

            if (feature.InnerRoundnessError > parameter.MaxRoundnessError)
            {
                defects.Add(CreateDefect(
                    GRingDefectType.RoundnessNG,
                    ToRect(feature.InnerCenter, feature.InnerRadius),
                    "Inner roundness NG."));
            }
        }

        private void CheckCenterOffset(
            GRingFeature feature,
            Rectangle validRoi,
            GRingParameter parameter,
            List<GDefectResult> defects)
        {
            if (!feature.HasOuterCircle || !feature.HasInnerCircle)
                return;

            if (feature.CenterOffset > parameter.MaxCenterOffset)
            {
                defects.Add(CreateDefect(
                    GRingDefectType.CenterOffsetNG,
                    validRoi,
                    "Center offset NG."));
            }
        }

        private GDefectResult CreateDefect(
            GRingDefectType defectType,
            Rectangle boundingBox,
            string message)
        {
            GDefectResult defect = new GDefectResult();

            defect.DefectType = defectType.ToString();
            defect.BoundingBox = boundingBox;
            //defect.Message = message;

            return defect;
        }

        private Rectangle ToRect(PointF center, double radius)
        {
            int x = (int)Math.Round(center.X - radius);
            int y = (int)Math.Round(center.Y - radius);
            int size = (int)Math.Round(radius * 2.0);

            return new Rectangle(x, y, size, size);
        }
    }
}
