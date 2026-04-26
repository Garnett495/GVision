## GVision.Viewer

### 簡介

`GVision.Viewer` 是一個基於 WinForms 的影像顯示控制項，專為 AOI 系統設計，提供高效能圖片檢視、縮放、拖曳與檢測結果疊加顯示功能。

---

## 功能特色 🚀

* 🖼️ 高效圖片顯示（Bitmap）
* 🔍 滾輪縮放（以滑鼠為中心）
* ✋ 滑鼠拖曳平移
* 📐 Fit to Window 自動縮放
* 🧩 Overlay 顯示（ROI / Blob / Text）
* 🎯 座標轉換（Viewer ↔ Image）
* ⚡ 拖曳時自動降畫質提升流暢度

---

## 架構說明

```text
GVision.Viewer
├── Controls
│   └── GImageViewer.cs
├── Models
│   └── GOverlayItem.cs
└── Enums
    └── GOverlayType.cs
```

---

## 使用方式

### 1️⃣ 加入控制項

```csharp
private GImageViewer _viewer;

private void InitViewer()
{
    _viewer = gImageViewer1; // 使用 Designer 控制項
}
```

---

### 2️⃣ 載入圖片

```csharp
using (Bitmap bmp = new Bitmap(@"D:\Test\image.jpg"))
{
    _viewer.LoadImage(bmp);
}
```

---

### 3️⃣ 顯示 Overlay

```csharp
List<GOverlayItem> overlays = new List<GOverlayItem>();

overlays.Add(new GOverlayItem
{
    Type = GOverlayType.Rectangle,
    Rect = new RectangleF(100, 80, 200, 120),
    Text = "Blob",
    Color = Color.Lime,
    LineWidth = 2
});

_viewer.SetOverlays(overlays);
```

---

### 4️⃣ 座標取得

```csharp
_viewer.MouseImagePointChanged += (s, p) =>
{
    Console.WriteLine($"X={p.X}, Y={p.Y}");
};
```

---

## 方法說明

* `LoadImage(Bitmap image)`

  * 載入圖片並自動縮放至畫面

* `FitToWindow()`

  * 將圖片縮放並置中

* `ResetView()`

  * 重置縮放與位置

* `SetOverlays(List<GOverlayItem> overlays)`

  * 設定顯示的檢測結果

* `ViewerToImage(Point p)`

  * Viewer → Image 座標轉換

* `ImageToViewer(PointF p)`

  * Image → Viewer 座標轉換

---

## 設計重點

* Overlay 一律使用 **Image 座標**
* Viewer 負責顯示，不負責演算法
* 拖曳時降低繪圖品質，提升操作流暢度
* 適用於 AOI 檢測、ROI 設定、結果檢視

---

## 注意事項

* 建議不要與 `PictureBox` 混用
* `LoadImage()` 會複製 Bitmap，外部可自行 Dispose
* 高解析圖片建議搭配 ROI 使用提升效能

---
