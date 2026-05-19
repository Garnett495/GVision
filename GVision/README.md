# GVision

適用於工業 AOI 系統的 .NET Framework 影像檢測類別庫，提供模組化的缺陷檢測演算法框架。

**版本**：1.0.4

---

## 目錄

- [專案概覽](#專案概覽)
- [技術棧](#技術棧)
- [架構圖](#架構圖)
- [模組說明](#模組說明)
- [快速開始](#快速開始)
- [API 索引](#api-索引)
- [依賴套件](#依賴套件)
- [子模組文件](#子模組文件)

---

## 專案概覽

GVision 是一個 **C# 類別庫（DLL）**，專為工業 AOI（Automated Optical Inspection）系統設計。核心設計理念：

- **統一介面**：所有檢測方法共用 `GInspectionRequest / GInspectionResult` 資料格式
- **模組化**：ROI、Viewer、演算法層彼此獨立，可單獨引用
- **可擴充**：新增檢測方法只需繼承 `GInspectionMethodBase`，實作核心邏輯即可

---

## 技術棧

| 項目 | 內容 |
|------|------|
| 語言 | C# |
| 框架 | .NET Framework 4.7.2 |
| 應用類型 | Class Library (.dll) |
| 影像處理 | Emgu.CV 4.4.0（OpenCV .NET 包裝） |
| UI 控制項 | SunnyUI 3.9.6、WinForms |
| 建置工具 | Visual Studio 2015+ |

---

## 架構圖

```
GVision
├── Core/                     ← 基底抽象類別
│   └── GInspectionMethodBase
│
├── Models/                   ← 通用資料模型
│   ├── GInspectionRequest    (檢測輸入)
│   ├── GInspectionResult     (檢測輸出)
│   ├── GDefectResult         (單一缺陷資訊)
│   └── GDefectStatistics     (統計資訊)
│
├── Algorithms/               ← 檢測演算法（各自獨立）
│   ├── Blob/                 (污點 / 異物檢測)
│   ├── Ring/                 (圓形幾何檢測)
│   ├── Focus/                (影像清晰度評分)
│   └── IGInspectionMethod    (共用介面)
│
├── Preprocessing/            ← 影像前處理工具
│   ├── GGrayPreprocessor
│   ├── GBlurPreprocessor
│   ├── GThresholdPreprocessor
│   └── GMorphologyPreprocessor
│
├── ROI/                      ← 感興趣區域（無 UI 依賴）
│   ├── Models/
│   ├── Services/
│   └── Core/
│
├── Viewer/                   ← WinForms 影像顯示控制項
│   └── Controls/GImageViewer
│
└── Rendering/                ← 缺陷標記繪製
    └── GOverlayRenderer
```

---

## 模組說明

### Algorithms — 檢測演算法

| 模組 | 功能 | 關鍵類別 |
|------|------|---------|
| **Blob** | 偵測影像中的污點、斑點、異物 | `GBlobInspectionMethod`、`GBlobParameter` |
| **Ring** | 檢查圓形產品幾何（內外圓、圓度、偏心） | `GRingInspectionMethod`、`GRingParameter` |
| **Focus** | 評估影像清晰度（Laplacian 評分） | `GFocusScorer` |

所有演算法繼承 `GInspectionMethodBase`，共用以下執行流程：

```
ROI 裁切 → 灰階轉換 → 模糊濾波 → 二值化 → 形態學 → 候選偵測 → 特徵評估 → 輸出結果
```

**GBlobParameter 關鍵參數：**

| 參數 | 說明 |
|------|------|
| `Threshold` | 二值化閾值（Fixed / Otsu / Adaptive 三種模式） |
| `EnableBlur` | 是否啟用高斯模糊 |
| `MorphologyType` | 形態學類型（Open / Close / Erode / Dilate / Gradient） |
| `MinArea` / `MaxArea` | 缺陷面積篩選範圍 |
| `MinWidth` / `MinHeight` | 缺陷尺寸篩選 |

---

### Preprocessing — 影像前處理

| 類別 | 功能 |
|------|------|
| `GGrayPreprocessor` | 彩色 → 灰階轉換 |
| `GBlurPreprocessor` | 高斯模糊降雜訊 |
| `GThresholdPreprocessor` | 二值化（Fixed / Otsu / Adaptive） |
| `GMorphologyPreprocessor` | 形態學運算（膨脹、腐蝕、開運算、閉運算等） |

---

### ROI — 感興趣區域

- 完全獨立於 UI，可在無 Viewer 的環境中使用
- 支援多個 ROI 同時啟用 / 停用管理
- 自動邊界修正與影像座標轉換
- XML 序列化支援設定儲存與載入

| 類別 | 職責 |
|------|------|
| `GRoiRegion` | ROI 資料模型（名稱、座標、啟用狀態） |
| `GRoiManager` | ROI 集合管理 |
| `GRoiSerializer` | XML 儲存 / 載入 |
| `GRoiHelper` | 座標合法化、轉換邏輯 |
| `GEmguRoiHelper` | Emgu.CV 影像裁切 |

---

### Viewer — WinForms 影像顯示

`GImageViewer` 控制項提供：

- 滾輪縮放（以滑鼠中心為縮放基準）
- 滑鼠拖曳平移，拖曳期間自動降低畫質以維持流暢度
- Overlay 疊圖（ROI 框、缺陷框、文字標注）
- 圖像座標與控制項座標雙向轉換

```csharp
viewer.LoadImage(bitmap);
viewer.SetOverlays(overlayList);
viewer.FitToWindow();
```

---

### Rendering — 結果繪製

`GOverlayRenderer`（靜態工具類）：

- 將缺陷結果標記繪製至原圖
- 標示 ROI 邊界
- 標注缺陷編號與面積數值

---

## 快速開始

### Blob 污點檢測

```csharp
using GVision.Algorithms.Blob;
using GVision.Models;
using GVision.ROI.Models;

// 1. 載入影像
Mat image = CvInvoke.Imread(@"D:\TestImages\sample.png");

// 2. 設定檢測參數
GBlobParameter param = new GBlobParameter();
param.Threshold    = 180;
param.EnableBlur   = true;
param.MinArea      = 5;

// 3. 建立檢測請求
GInspectionRequest request = new GInspectionRequest();
request.SourceImage      = image;
request.Parameter        = param;
request.Roi              = new GRoiRegion("InspectArea", new Rectangle(100, 100, 500, 400));
request.EnableDebugImage = true;

// 4. 執行檢測
GBlobInspectionMethod method = new GBlobInspectionMethod();
GInspectionResult result = method.Inspect(request);

// 5. 讀取結果
Console.WriteLine("判定：" + (result.IsOk ? "OK" : "NG"));
Console.WriteLine("缺陷數：" + result.Statistics.DefectCount);
Console.WriteLine("總缺陷面積：" + result.Statistics.TotalArea);
```

### Ring 圓形幾何檢測

```csharp
using GVision.Algorithms.Ring;
using GVision.Models;
using GVision.ROI.Models;

GRingParameter param = new GRingParameter();
GInspectionRequest request = new GInspectionRequest()
{
    SourceImage = image,
    Parameter   = param,
    Roi         = new GRoiRegion("RingROI", new Rectangle(100, 100, 400, 400))
};

GRingInspectionMethod method = new GRingInspectionMethod();
GInspectionResult result = method.Inspect(request);
```

### 擴充新增檢測方法

```csharp
public class GCustomInspectionMethod : GInspectionMethodBase
{
    protected override GInspectionResult ExecuteInspect(GInspectionRequest request)
    {
        // 實作自訂檢測邏輯
        return new GInspectionResult();
    }
}
```

---

## API 索引

| 類別 | 命名空間 | 說明 |
|------|---------|------|
| `GBlobInspectionMethod` | `GVision.Algorithms.Blob` | Blob 污點檢測執行器 |
| `GBlobParameter` | `GVision.Algorithms.Blob` | Blob 檢測參數 |
| `GRingInspectionMethod` | `GVision.Algorithms.Ring` | Ring 圓形幾何檢測執行器 |
| `GRingParameter` | `GVision.Algorithms.Ring` | Ring 檢測參數 |
| `GFocusScorer` | `GVision.Algorithms.Focus` | 影像清晰度評分 |
| `GInspectionRequest` | `GVision.Models` | 統一檢測輸入格式 |
| `GInspectionResult` | `GVision.Models` | 統一檢測輸出格式 |
| `GDefectResult` | `GVision.Models` | 單一缺陷資訊（位置、面積、分數） |
| `GDefectStatistics` | `GVision.Models` | 缺陷統計（總數、總面積、最大面積） |
| `GInspectionMethodBase` | `GVision.Core` | 所有檢測方法的基底類別 |
| `GRoiRegion` | `GVision.ROI.Models` | ROI 區域資料模型 |
| `GRoiManager` | `GVision.ROI.Services` | ROI 集合管理 |
| `GRoiSerializer` | `GVision.ROI.Services` | ROI XML 序列化 |
| `GImageViewer` | `GVision.Viewer.Controls` | WinForms 影像顯示控制項 |
| `GOverlayRenderer` | `GVision.Rendering` | 結果標記繪製工具 |

---

## 依賴套件

| 套件 | 版本 | 用途 |
|------|------|------|
| Emgu.CV | 4.4.0.4099 | OpenCV .NET 包裝，核心影像處理 |
| Emgu.CV.Bitmap | 4.4.0.4099 | Bitmap 格式轉換支援 |
| SunnyUI | 3.9.6 | UI 控制項庫（Viewer 模組使用） |
| System.Drawing.Common | 4.7.3 | GDI+ 繪圖支援 |
| System.Text.Json | 10.0.6 | JSON 序列化 |

---

## 子模組文件

- [ROI 模組詳細說明](ROI/README.md)
- [Ring 檢測流程與參數](Algorithms/Ring/README.md)
- [GImageViewer 控制項用法](Viewer/README.md)
