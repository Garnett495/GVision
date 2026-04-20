using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Emgu.CV;

namespace GVision.Algorithms.Blob
{
    /// <summary>
    /// Blob 候選區偵測器。
    /// 
    /// 第一版先直接回傳二值影像本身，
    /// 讓後續 FeatureExtractor 以 FindContours 處理。
    /// </summary>
    public class GBlobCandidateDetector
    {
        /// <summary>
        /// 取得 Blob 候選資料。
        /// </summary>
        public virtual Mat Detect(Mat preprocessedImage, GBlobParameter parameter)
        {
            Mat output = new Mat();
            preprocessedImage.CopyTo(output);
            return output;
        }
    }
}
