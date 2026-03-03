<p align="center">
  <img src="https://github.com/3astbeast/RedTailIndicators/blob/main/redtail-logo.png" width="200" alt="RedTail Indicators Logo"/>
</p>

<h1 align="center">RedTail Indicators</h1>

<p align="center">
  <b>Free NinjaTrader 8 indicators built for futures traders who demand precision.</b><br>
  Scalping-focused tools for MES, MNQ, MGC, and beyond.
</p>

<p align="center">
  <a href="https://buymeacoffee.com/dmwyzlxstj">
    <img src="https://img.shields.io/badge/☕_Buy_Me_a_Coffee-Support_My_Work-FFDD00?style=for-the-badge&logo=buy-me-a-coffee&logoColor=black" alt="Buy Me a Coffee"/>
  </a>
</p>

<p align="center">
  <i>All indicators are free to use. If you find them helpful, donations are always appreciated but never required.</i>
</p>

---

## 📊 Indicators

---

### 🔴 [RedTail Volume Profile V2](https://github.com/3astbeast/RedTail-Volume-Profile)

A comprehensive volume profile indicator for NinjaTrader 8 with institutional-grade features packed into nearly 10,000 lines of NinjaScript. This is not a basic volume profile — it's a full-featured analysis suite designed for serious futures scalpers.

**Profile Modes**
- **Session** — Per-session profiles with configurable lookback
- **Visible Range** — Dynamically calculates across your visible chart area
- **Weekly / Monthly** — Higher timeframe profiles
- **Composite** — Aggregate profiles across custom date ranges (days back, weeks back, or custom start/end dates)
- **Anchored** — Session-by-session profiles pinned to their respective time periods

**Key Features**
- **Point of Control (POC)** and **Value Area (VAH/VAL)** with full visual customization
- **Naked Levels** — Tracks daily and weekly naked POC, VAH, and VAL levels that haven't been revisited, with configurable touch count removal and max levels to display
- **Previous Day Levels** — POC, VAH, VAL, High, and Low from the prior session
- **Previous Week Levels** — POC, VAH, VAL, High, and Low from the prior week
- **Overnight Session Levels** — Separate POC, VAH, VAL, High, and Low for the overnight session (default 6:00 PM – 8:30 AM, fully configurable)
- **Dual Profile Mode** — Overlays both weekly and session profiles simultaneously with independent settings for each
- **Low Volume Nodes (LVN)** — Detects and highlights low-volume gaps in the profile
- **Move Profiles** — Automatically detects breakout moves from consolidation and builds volume profiles for each move
- **Candle Profiles** — Tick-based volume profiles rendered on individual candles
- **DOM Visualization (Domdicator)** — Live order book depth visualization directly on the chart with historical order tracking
- **Gradient Fill** — Optional gradient rendering for visual depth
- **Adaptive Rendering** — Auto-adjusts bar sizing with smoothing for clean visuals at any zoom level

**Volume Display Types:** Standard, Bullish, Bearish, or Both (split delta)

**Alerts**
- Proximity alerts when price approaches any key level (Previous Day, Previous Week, Overnight, Naked Daily, Naked Weekly)
- Configurable distance in ticks with sound alerts
- Auto-rearm on new session

**Additional Details**
- Every line, level, and label is independently customizable (color, dash style, thickness, opacity)
- Touch count tracking with optional label display
- British/US date format support
- Price value labels on all reference levels
- Exposed plot outputs for POC, VAH, VAL, PD levels, and Overnight levels — usable by strategies or other indicators

---

### 🔴 [RedTail Auto-VWAP](https://github.com/3astbeast/RedTail-Auto-VWAP)

> *Description coming soon — uploading code for detailed writeup.*

---

### 🔴 [RedTail Market Structure](https://github.com/3astbeast/RedTail-Market-Structure)

> *Description coming soon — uploading code for detailed writeup.*

---

### 🔴 [RedTail EMA Cloud](https://github.com/3astbeast/RedTail-EMA-Cloud)

> *Description coming soon — uploading code for detailed writeup.*

---

## 🛠 Installation

1. Download the `.cs` file from the indicator's repository
2. Open NinjaTrader 8
3. Go to **Tools → Import → NinjaScript Add-On**
4. Select the downloaded file and click **OK**
5. The indicator will appear in your **Indicators** list — add it to any chart

## 📬 Contact

Have questions, feature requests, or bugs to report? Open an **Issue** on the relevant indicator's repo.

---

<p align="center">
  <a href="https://buymeacoffee.com/dmwyzlxstj">
    <img src="https://img.shields.io/badge/☕_Buy_Me_a_Coffee-FFDD00?style=flat-square&logo=buy-me-a-coffee&logoColor=black" alt="Buy Me a Coffee"/>
  </a>
</p>
