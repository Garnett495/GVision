using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using Emgu.CV;

using GVision.ROI.Core;
using GVision.ROI.Models;
using GVision.ROI.Services;

namespace GVision.ROI.Example
{
    /// <summary>
    /// ROI 基本使用範例。
    /// 
    /// 此範例不需要 Viewer。
    /// </summary>
    public class GRoiBasicExample
    {
        public void Run()
        {
            // 建立 ROI
            GRoiRegion roi = new GRoiRegion(
                "BlobDetectROI",
                new Rectangle(100, 80, 300, 200));

            // 假設影像尺寸
            Size imageSize = new Size(1920, 1080);

            // 取得有效 ROI
            Rectangle validRoi = GRoiHelper.GetValidRoi(roi, imageSize);

            Console.WriteLine("Valid ROI = " + validRoi.ToString());

            // ROI Manager
            GRoiManager roiManager = new GRoiManager();
            roiManager.Add(roi);

            // 儲存 ROI
            GRoiSerializer.Save(
                @"D:\GVision\RoiSettings.xml",
                roiManager.ToCollection());

            // 載入 ROI
            GRoiCollection loaded = GRoiSerializer.Load(@"D:\GVision\RoiSettings.xml");

            roiManager.LoadFromCollection(loaded);
        }

        public void RunWithEmgu(Mat sourceImage)
        {
            if (sourceImage == null)
                return;

            GRoiRegion roi = new GRoiRegion(
                "BlobDetectROI",
                new Rectangle(100, 80, 300, 200));

            // 裁切 ROI 影像
            Mat roiImage = GEmguRoiHelper.Crop(sourceImage, roi);

            // 後續可丟給 Blob / Edge / Pattern 等檢測流程
            // blobDetector.Process(roiImage);
        }
    }
}
