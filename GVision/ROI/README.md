# 📦 GVision.ROI

## 簡介
`GVision.ROI` 是一個**獨立、低相依性**的 ROI（Region of Interest）模組，  
可用於 AOI 檢測流程中，負責 ROI 定義、合法化、座標轉換與影像裁切。

設計重點：
- 不依賴 UI（Viewer 為可選）
- 可直接與演算法（Blob / Edge / AI）整合
- 支援未來擴充多 ROI / Ignore ROI

---

## 功能特色
- 🧩 模組化設計（完全獨立於 Viewer）
- 📐 ROI 合法性自動修正（避免越界）
- 🔄 ROI 座標轉換（Local ↔ Global）
- ✂️ ROI 影像裁切（支援 EmguCV）
- 💾 ROI 儲存 / 載入（XML）
- 🔌 易於整合 AOI 演算法

---

## 架構設計

```text
GVision.ROI
├── Models
│   └── GRoiRegion.cs        // ROI 資料模型
│
├── Core
│   ├── GRoiHelper.cs        // ROI 核心邏輯（無外部依賴）
│   └── GEmguRoiHelper.cs    // EmguCV 裁切功能
│
├── Services
│   ├── GRoiManager.cs       // ROI 管理
│   ├── GRoiCollection.cs    // 序列化用集合
│   └── GRoiSerializer.cs    // XML 儲存 / 載入
│
├── ViewerAdapter
│   └── IGImageViewerAdapter.cs // Viewer 擴充接口（選用）
```


## 核心概念
 - ROI 使用「原圖座標」
- 所有 ROI 都以 原始影像座標 為基準
- 不與 UI 綁定
- Viewer 僅負責顯示與操作


## 基本使用
### 建立 ROI
``` csharp
var roi = new GRoiRegion(
    "DetectROI",
    new Rectangle(100, 80, 300, 200));
```

### 取得有效 ROI
``` csharp
Rectangle validRoi = GRoiHelper.GetValidRoi(roi, image.Size);
```

### 裁切影像（EmguCV）
```csharp
Mat roiImage = GEmguRoiHelper.Crop(image, roi);
```

### 座標轉換
```csharp
// ROI 內 → 原圖
Rectangle global = GRoiHelper.ToGlobal(localRect, validRoi);

// 原圖 → ROI 內
Rectangle local = GRoiHelper.ToLocal(globalRect, validRoi);
```

### ROI 管理
```csharp
var manager = new GRoiManager();

manager.Add(new GRoiRegion("R1", new Rectangle(0, 0, 100, 100)));
manager.Add(new GRoiRegion("R2", new Rectangle(200, 200, 150, 150)));

var enabled = manager.GetEnabledRegions();
```


### ROI 儲存 / 載入
```csharp
// 儲存
GRoiSerializer.Save("roi.xml", manager.ToCollection());

// 載入
var data = GRoiSerializer.Load("roi.xml");
manager.LoadFromCollection(data);
```

### 與 Blob 檢測整合
```csharp
Rectangle validRoi = GRoiHelper.GetValidRoi(request.Roi, image.Size);

Mat roiImage = GEmguRoiHelper.Crop(image, request.Roi);

// 檢測結果轉回原圖座標
Rectangle global = GRoiHelper.ToGlobal(localRect, validRoi);
```



## 設計重點
- ROI ≠ Viewer
- ROI = 演算法輸入的一部分
- Viewer = ROI 操作工具（可選）



## 未來擴充方向
- 多 ROI 支援
- Ignore / Mask ROI
- Polygon ROI
- ROI 編輯器（滑鼠操作）
- 與 Recipe 系統整合