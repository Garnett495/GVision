## 📌 GVision

GVision 是一個以 **AOI（自動光學檢測）** 為目標設計的影像處理 DLL，  
專注於提供 **可擴充、模組化、可重用** 的檢測架構，支援多種影像缺陷檢測（如 Blob、Particle、Scratch）。

---

## 🚀 功能特色
- 🧩 模組化檢測架構（Method-Based Design）
- 🔍 Blob 檢測（已完成第一版）
- 🧱 前處理模組（Gray / Blur / Morphology）
- 🎯 ROI 機制（預留擴充）
- 📊 檢測結果統一格式（Result / Statistics）
- 🔄 可擴充多種檢測方法（Particle / Scratch / Edge...）
- ⚙️ 與 UI 解耦（支援 WinForm / Console / Service）

---

## 🏗️ 專案架構

```plaintext
GVision
├─ Algorithms
│   └─ Blob
│       ├─ GBlobInspectionMethod.cs
│       ├─ GBlobCandidateDetector.cs
│       ├─ GBlobFeatureExtractor.cs
│       ├─ GBlobEvaluator.cs
│       └─ GBlobParameter.cs
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

## 🔄 檢測流程（Blob）

```
Image
 → ROI（預留）
 → Preprocessing（Gray / Blur / Morphology）
 → Threshold（二值化）
 → Blob Detection（Contour）
 → Feature Extraction（Area / Width / Height）
 → Filtering（條件篩選）
 → Result Output
 ```
 
 ## 🧠 核心設計概念
 1️⃣ Method-Based 架構

每一種檢測都是獨立 Method：

GBlobInspectionMethod
未來可擴充：
GParticleInspectionMethod
GScratchInspectionMethod

👉 好處：

可插拔
易維護
易測試

2️⃣ 統一請求與回傳

Request
```csharp
GInspectionRequest
{
    Mat SourceImage;
    Rectangle? Roi;
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

3️⃣ 分層設計（Blob）
```
GBlobInspectionMethod
├─ CandidateDetector（找候選）
├─ FeatureExtractor（抽特徵）
└─ Evaluator（篩選結果）
```

4️⃣ 前處理模組化
```
GGrayPreprocessor
GBlurPreprocessor
GMorphologyPreprocessor
```
👉 可自由組合：
```csharp
if (param.EnableBlur)
    image = GBlurPreprocessor.Apply(image);

if (param.EnableMorphology)
    image = GMorphologyPreprocessor.Open(image);
```


## 🔧 使用方式（Blob 範例）
```csharp
var param = new GBlobParameter
{
    Threshold = 100,
    MinArea = 50,
    EnableBlur = true,
    EnableMorphology = true
};

var request = new GInspectionRequest
{
    SourceImage = image,
    Parameter = param,
    Roi = null
};

var method = new GBlobInspectionMethod();
var result = method.Inspect(request);
```



## 📦 相依套件
- Emgu.CV (建議版本：4.6.0.5131)
- Emgu.CV.Bitmap
- Emgu.CV.runtime.windows



## ⚠️ 注意事項
- 不同 EmguCV 版本 API 可能不相容（如 ImreadModes、ElementShape）
- 建議固定版本避免開發問題
- UI 不建議與 Core 混在同一專案



## 🧭 未來規劃
🔹 檢測能力擴充
- Particle Detection
- Scratch Detection
- Edge / Line 檢測



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
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.