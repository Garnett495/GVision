using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;

namespace GVision.ROI.Services
{
    /// <summary>
    /// ROI XML 儲存 / 載入工具。
    /// 
    /// 第一版先使用 XML，方便與 Recipe 整合。
    /// </summary>
    public static class GRoiSerializer
    {
        /// <summary>
        /// 儲存 ROI 集合。
        /// </summary>
        public static void Save(string filePath, GRoiCollection collection)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("filePath is null or empty.");

            if (collection == null)
                collection = new GRoiCollection();

            string dir = Path.GetDirectoryName(filePath);

            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            XmlSerializer serializer = new XmlSerializer(typeof(GRoiCollection));

            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                serializer.Serialize(fs, collection);
            }
        }

        /// <summary>
        /// 載入 ROI 集合。
        /// 
        /// 檔案不存在時回傳空集合。
        /// </summary>
        public static GRoiCollection Load(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("filePath is null or empty.");

            if (!File.Exists(filePath))
                return new GRoiCollection();

            XmlSerializer serializer = new XmlSerializer(typeof(GRoiCollection));

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                GRoiCollection collection = serializer.Deserialize(fs) as GRoiCollection;

                if (collection == null)
                    return new GRoiCollection();

                return collection;
            }
        }
    }
}
