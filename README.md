<p align="center">
  <img src="https://avatars.githubusercontent.com/u/209633456?v=4" width="200" alt="RedTail Indicators Logo"/>
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

### 🔴 [RedTail Market Structure V2](https://github.com/3astbeast/RedTail-Market-Structure)

A full Smart Money / ICT-style market structure indicator for NinjaTrader 8 that goes far beyond basic BOS detection. Combines swing structure analysis, volumized order blocks, integrated Fibonacci retracements with volume profiles, strong/weak level scoring, equal highs/lows detection, liquidity sweep identification, and a built-in voice alert system — all in one indicator.

**Market Structure Core**
- **Break of Structure (BOS)** and **Change of Character (CHoCH)** detection with configurable swing length
- BOS confirmation mode: **Candle Close** or **Wicks**
- Real-time **trend state tracking** (Bullish Confirmed, Bearish Confirmed, CHoCH Pending) with on-chart display
- **50% retracement lines** drawn automatically on each structural leg
- Swing high/low labels with customizable font and colors

**Volumized Order Blocks**
- Automatically detects bullish and bearish order blocks using ATR-based displacement filtering
- **Breaker block conversion** — when an OB is broken, it flips to a breaker zone
- Zone invalidation by **wick** or **close** (configurable)
- Zone density control: One, Low, Medium, or High
- Draw style: **Full Range** or **Wick Only**
- Historical zone display with optional auto-deletion of broken boxes

**FRVP (Fibonacci Retracement + Volume Profile)**
- Triggers on **CHoCH** or **BOS (Confirmed Trend)** — your choice
- Builds a full volume profile across the retracement range with POC, Value Area, and VA lines
- Up to **10 customizable Fibonacci levels** with individual colors
- **Anchored VWAP** from the swing origin, with optional extension
- **Cluster level detection** using K-Means clustering to find volume concentration zones within the FRVP
- Adaptive rendering with gradient fill support
- Volume display: Standard, Bullish, Bearish, or Both

**Strong & Weak Levels**
- Scores swing highs/lows based on volume analysis and structural context
- Configurable volume multiplier and minimum score threshold
- Mitigation modes: **Remove**, **Keep (Fade)**, or **Keep (Fade) + Clear New Session**

**Equal Highs / Equal Lows**
- Detects equal swing highs and lows within a configurable tick tolerance
- Draws extending lines marking potential liquidity pools
- Visual labels ("EQH" / "EQL") at each detection point

**Liquidity Sweeps**
- Identifies sweep candles that pierce and reject from key levels
- Configurable minimum depth (ticks), minimum wick percentage, and optional volume filter
- Can sweep both strong/weak levels and order block zones
- Visual markers with directional arrows

**Displacement Candles**
- Highlights candles with outsized range relative to ATR
- Configurable ATR multiplier and minimum body percentage filter

**Alert System**
- Separate alert sounds for BOS, CHoCH, OB creation, OB touch, AVWAP touch, Fib level touch, and liquidity sweeps
- **Built-in voice alert engine** using text-to-speech — generates spoken alerts per instrument (e.g., "NQ Bullish CHoCH")
- Configurable cooldown timer to prevent alert spam
- Fallback to standard .wav sounds if voice generation fails

---

### 🔴 [RedTail Auto-VWAP](https://github.com/3astbeast/RedTail-Auto-VWAP)

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
