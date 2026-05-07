## GVision.Algorithms.Ring

## 簡介

提供塑膠環（Ring）幾何檢測功能，主要用於判斷內外圓是否符合規格，包括圓形度、半徑範圍與中心偏移等條件。適用於 AOI 圓形產品檢測場景。

---

## 功能特色

🔵 支援內外圓自動檢測
📐 提供圓形度（Circularity）與圓度誤差判斷
📍 支援中心偏移檢測
📦 完整模組化架構（Detector / Feature / Evaluator）
🧩 可與 GVision ROI / Blob / Viewer 無縫整合

---

## 架構設計

```text
GRingInspectionMethod
    ↓
GRingCandidateDetector   // 找輪廓
    ↓
GRingFeatureExtractor    // 計算幾何特徵
    ↓
GRingEvaluator           // 判斷 OK / NG
    ↓
GInspectionResult
```

---

## 使用方式

### 基本範例

```csharp
Mat image = CvInvoke.Imread(@"D:\Test\ring.jpg");

GRingParameter param = new GRingParameter();

GInspectionRequest request = new GInspectionRequest()
{
    SourceImage = image,
    Parameter = param,
    Roi = null
};

GRingInspectionMethod method = new GRingInspectionMethod();

GInspectionResult result = method.Inspect(request);

bool isOK = result.Defects == null || result.Defects.Count == 0;
```

---

## ROI 使用

```csharp
request.Roi = new GRoiRegion(
    "RingROI",
    new Rectangle(100, 100, 400, 400));
```

---

## 參數說明

### Threshold

| 參數                | 說明     |
| ----------------- | ------ |
| `ThresholdValue`  | 二值化門檻值 |
| `InvertThreshold` | 是否反相   |

---

### Outer Circle

| 參數                       | 說明     |
| ------------------------ | ------ |
| `MinOuterRadius`         | 外圓最小半徑 |
| `MaxOuterRadius`         | 外圓最大半徑 |
| `EnableOuterCircleCheck` | 是否檢查外圓 |

---

### Inner Circle

| 參數                       | 說明     |
| ------------------------ | ------ |
| `MinInnerRadius`         | 內圓最小半徑 |
| `MaxInnerRadius`         | 內圓最大半徑 |
| `EnableInnerCircleCheck` | 是否檢查內圓 |

---

### Shape

| 參數                  | 說明            |
| ------------------- | ------------- |
| `MinCircularity`    | 最小圓形度（越接近1越圓） |
| `MaxRoundnessError` | 最大圓度誤差        |

---

### Position

| 參數                        | 說明       |
| ------------------------- | -------- |
| `MaxCenterOffset`         | 中心偏移容許值  |
| `EnableCenterOffsetCheck` | 是否檢查中心偏移 |

---

## 判定結果

當發生以下情況會判定為 NG：

```text
OuterCircleNotFound
InnerCircleNotFound
OuterRadiusNG
InnerRadiusNG
OuterCircularityNG
InnerCircularityNG
CenterOffsetNG
RoundnessNG
```

---

## 適用場景

* 塑膠環尺寸檢測
* 內孔是否變形
* 偏心判斷
* 基本幾何品質檢查

---

## 後續擴充

* 塑膠殘膠 / 異物檢測
* 油污檢測
* Ring 表面缺陷分析
* 多演算法整合（Ring + Blob）

---

## 注意事項

* 建議先放寬參數確認能偵測到圓形，再逐步收斂條件
* ROI 設定會直接影響檢測結果
* 光源與對比度會影響 Threshold 表現

---
