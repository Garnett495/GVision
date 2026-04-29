## 📌 GVision

GVision 是一個以 **AOI（自動光學檢測）** 為核心的影像處理 DLL，
提供 **模組化、可擴充、可重用** 的檢測架構，支援多種檢測方法（Blob、Ring 等）。

---

## 🚀 功能特色
```
🧩 Method-Based 模組化架構
🔍 Blob 檢測（第一版完成）
⭕ Ring 圓形檢測（新增）
🎯 ROI 機制（支援區域檢測）
📊 統一檢測結果格式（Result / Defect）
🔄 可擴充多種檢測方法（Particle / Scratch / Edge）
⚙️ 與 UI 解耦（WinForms / Console / Service）
```
---

## 🏗️ 專案架構

```plaintext
GVision
├─ Algorithms
│   ├─ Blob
│   │   ├─ GBlobInspectionMethod.cs
│   │   ├─ GBlobCandidateDetector.cs
│   │   ├─ GBlobFeatureExtractor.cs
│   │   ├─ GBlobEvaluator.cs
│   │   └─ GBlobParameter.cs
│   │
│   └─ Ring
│       ├─ GRingInspectionMethod.cs
│       ├─ GRingCandidateDetector.cs
│       ├─ GRingFeatureExtractor.cs
│       ├─ GRingEvaluator.cs
│       ├─ GRingFeature.cs
│       └─ GRingParameter.cs
│
├─ Core
│   └─ GInspectionMethodBase.cs
│
├─ Models
│   ├─ GInspectionRequest.cs
│   ├─ GInspectionResult.cs
│   ├─ GDefectResult.cs
│   └─ GDefectStatistics.cs
│
├─ Preprocessing
│   ├─ GGrayPreprocessor.cs
│   ├─ GBlurPreprocessor.cs
│   └─ GMorphologyPreprocessor.cs
│
└─ Interfaces
    ├─ IInspectionMethod.cs
    └─ IInspectionParameter.cs
```

---

## 🔄 檢測流程

### Blob

```
Image
 → Preprocessing
 → Threshold
 → Contour Detection
 → Feature Extraction
 → Filtering
 → Result
```

### Ring（新增）

```
Image
 → ROI
 → Threshold
 → Contour Detection
 → Outer / Inner Circle Detection
 → Feature Extraction（Radius / Circularity / Offset）
 → Evaluation（OK / NG）
 → Result
```

---

## 🧠 核心設計概念

### 1️⃣ Method-Based 架構

每種檢測都是獨立模組：

```text
GBlobInspectionMethod
GRingInspectionMethod
```

👉 好處：

* 可插拔
* 易維護
* 易測試
* 易擴充

---

### 2️⃣ 統一請求與回傳

Request

```csharp
GInspectionRequest
{
    Mat SourceImage;
    GRoiRegion Roi;
    IInspectionParameter Parameter;
}
```

Result

```csharp
GInspectionResult
{
    bool IsSuccess;
    bool IsOk;
    List<GDefectResult> Defects;
    GDefectStatistics Statistics;
    Mat ResultOverlay;
}
```

---

### 3️⃣ 分層設計（Ring）

```
GRingInspectionMethod
├─ CandidateDetector（找輪廓）
├─ FeatureExtractor（計算圓形特徵）
└─ Evaluator（判定 OK / NG）
```

---

### 4️⃣ ROI 設計

```csharp
GRoiRegion
{
    Rectangle Bounds;
    bool IsEnabled;
}
```

👉 支援：

* 區域檢測
* 未來多 ROI 擴充
* UI/演算法共用

---

## 🔧 使用方式

### Blob 範例

```csharp
var param = new GBlobParameter
{
    Threshold = 100,
    MinArea = 50
};

var request = new GInspectionRequest
{
    SourceImage = image,
    Parameter = param
};

var method = new GBlobInspectionMethod();
var result = method.Inspect(request);
```

---

### Ring 範例（新增）

```csharp
var param = new GRingParameter
{
    ThresholdValue = 80,
    MinCircularity = 0.85
};

var request = new GInspectionRequest
{
    SourceImage = image,
    Parameter = param
};

var method = new GRingInspectionMethod();
var result = method.Inspect(request);
```

---

## 📦 相依套件

* Emgu.CV（建議：4.6.0.5131）
* Emgu.CV.Bitmap
* Emgu.CV.runtime.windows

---

## ⚠️ 注意事項

* 不同 EmguCV 版本 API 可能不相容
* 建議固定版本避免問題
* UI 不建議與 Core 混合
* Threshold 與光源品質會直接影響結果

---

## 🧭 未來規劃

### 🔹 檢測能力擴充

* Particle Detection
* Scratch Detection
* Edge / Line 檢測
* Ring 表面缺陷（殘膠 / 油污）

### 🔹 架構進化

* 多演算法整合（Ring + Blob）
* Recipe 管理
* 自動參數優化（Auto Tune）

---

## 📄 License

### 📌 License Type

This project is licensed under the **MIT License**.

---

### 📜 MIT License

```text
MIT License

Copyright (c) Garnett.C 2026

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction...
```
