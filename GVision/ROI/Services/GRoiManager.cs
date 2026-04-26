using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using GVision.ROI.Models;

namespace GVision.ROI.Services
{
    /// <summary>
    /// ROI 管理器。
    /// 
    /// 負責管理多個 ROI，不負責 UI 操作，也不依賴 Viewer。
    /// </summary>
    public class GRoiManager
    {
        private readonly List<GRoiRegion> _regions;

        public IList<GRoiRegion> Regions
        {
            get { return _regions.AsReadOnly(); }
        }

        public int Count
        {
            get { return _regions.Count; }
        }

        public GRoiManager()
        {
            _regions = new List<GRoiRegion>();
        }

        /// <summary>
        /// 新增 ROI。
        /// </summary>
        public void Add(GRoiRegion roi)
        {
            if (roi == null)
                throw new ArgumentNullException("roi");

            _regions.Add(roi);
        }

        /// <summary>
        /// 新增矩形 ROI。
        /// </summary>
        public GRoiRegion Add(string name, Rectangle bounds)
        {
            GRoiRegion roi = new GRoiRegion(name, bounds);
            Add(roi);
            return roi;
        }

        /// <summary>
        /// 移除 ROI。
        /// </summary>
        public bool Remove(GRoiRegion roi)
        {
            if (roi == null)
                return false;

            return _regions.Remove(roi);
        }

        /// <summary>
        /// 依名稱移除 ROI。
        /// </summary>
        public bool RemoveByName(string name)
        {
            GRoiRegion roi = GetByName(name);

            if (roi == null)
                return false;

            return _regions.Remove(roi);
        }

        /// <summary>
        /// 清除所有 ROI。
        /// </summary>
        public void Clear()
        {
            _regions.Clear();
        }

        /// <summary>
        /// 依名稱取得 ROI。
        /// </summary>
        public GRoiRegion GetByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            for (int i = 0; i < _regions.Count; i++)
            {
                if (_regions[i].Name == name)
                    return _regions[i];
            }

            return null;
        }

        /// <summary>
        /// 取得第一個啟用中的 ROI。
        /// 
        /// 適合 Blob 第一版只需要一個 ROI 的情境。
        /// </summary>
        public GRoiRegion GetFirstEnabled()
        {
            for (int i = 0; i < _regions.Count; i++)
            {
                if (_regions[i].IsEnabled)
                    return _regions[i];
            }

            return null;
        }

        /// <summary>
        /// 取得所有啟用中的 ROI。
        /// </summary>
        public List<GRoiRegion> GetEnabledRegions()
        {
            List<GRoiRegion> result = new List<GRoiRegion>();

            for (int i = 0; i < _regions.Count; i++)
            {
                if (_regions[i].IsEnabled)
                    result.Add(_regions[i]);
            }

            return result;
        }

        /// <summary>
        /// 匯出成可序列化集合。
        /// </summary>
        public GRoiCollection ToCollection()
        {
            GRoiCollection collection = new GRoiCollection();

            for (int i = 0; i < _regions.Count; i++)
            {
                collection.Items.Add(_regions[i].Clone());
            }

            return collection;
        }

        /// <summary>
        /// 從集合載入 ROI。
        /// </summary>
        public void LoadFromCollection(GRoiCollection collection)
        {
            _regions.Clear();

            if (collection == null || collection.Items == null)
                return;

            for (int i = 0; i < collection.Items.Count; i++)
            {
                if (collection.Items[i] != null)
                    _regions.Add(collection.Items[i].Clone());
            }
        }
    }
}
