<p align="center">
  <img src="https://avatars.githubusercontent.com/u/209633456?v=4" width="200" alt="RedTail Indicators Logo"/>
</p>

<h1 align="center">RedTail Indicators</h1>

<p align="center">
  <b>Free NinjaTrader 8 indicators built for futures traders who demand precision.</b><br>
  Intraday trading focused tools for NQ, ES, GC, CL, and beyond.
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

### 🔴 [RedTail Volume Profile](https://github.com/3astbeast/RedTail-Volume-Profile)

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

### 🔴 [RedTail LVN Hunter](https://github.com/3astbeast/RedTail-LVN-Hunter)

A lightweight, standalone Low Volume Node detector for NinjaTrader 8. This is the LVN detection engine extracted from RedTail Volume Profile into its own focused indicator — ideal if you just want clean LVN zones on your chart without the full volume profile overlay.

**How It Works**
- Builds an internal volume profile across a configurable lookback range, then scans for local volume troughs — price levels where volume is significantly lower than surrounding levels
- Detected LVNs are drawn as shaded rectangles extending across the full chart, highlighting potential breakout zones and key support/resistance areas

**Lookback Modes**
- **Fixed Bars** — Analyzes the last N bars (configurable, 50–5000)
- **Session** — Resets and recalculates at the start of each new session

**Detection Settings**
- **Profile Number of Rows** — Controls granularity of the volume analysis (more rows = finer/noisier LVNs, fewer rows = broader/smoother zones)
- **LVN Detection %** — Sensitivity threshold for identifying troughs (lower = more sensitive)
- **Show Adjacent LVN Nodes** — Expands each detected LVN to include the neighboring price levels above and below, creating wider zone rectangles

**Display**
- Customizable fill color, fill opacity, border color, and border opacity for all LVN rectangles
- Optimized for performance — only calculates on real-time bars and the last historical bar to minimize loading time

---

### 🔴 [RedTail Market Structure](https://github.com/3astbeast/RedTail-Market-Structure)

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

### 🔴 [RedTail FRVP](https://github.com/3astbeast/RedTail-FRVP)

A standalone Fibonacci Retracement + Volume Profile indicator for NinjaTrader 8. This is the FRVP engine from RedTail Market Structure extracted into its own lightweight indicator — same powerful analysis without the order blocks, strong/weak levels, or other market structure overlays. Ideal if you want clean FRVP zones on your chart without the full market structure suite.

**How It Works**
- Runs its own internal market structure engine (BOS/CHoCH detection with configurable swing length) silently in the background
- When a structural shift is detected, it automatically builds a Fibonacci retracement zone with an embedded volume profile across the swing range
- Trigger mode: **CHoCH** or **BOS (Confirmed Trend)** — your choice
- Option to keep or replace previous FRVP zones as new ones form

**Volume Profile**
- Full volume profile rendered within each FRVP zone with **POC** and **Value Area** (VAH/VAL)
- Configurable row count (resolution), profile width, and left/right alignment
- Volume display: Standard, Bullish, Bearish, or Both (split delta with polarity tracking)
- **Adaptive rendering** with configurable smoothing passes and auto-sized bar heights
- **Gradient fill** option for visual depth
- Boundary outline with configurable color, opacity, and width

**Fibonacci Levels**
- Up to **10 fully customizable Fibonacci levels** with individual colors
- Default levels: 0, 23.6, 38.2, 50, 61.8, 78.6, 100 (set any level to -1 to disable)
- Optional price labels and level extension beyond the zone
- Configurable line width, style, and opacity

**Anchored VWAP**
- AVWAP automatically anchored from the swing origin point
- Optional right extension and label display
- Independent color, width, style, and opacity settings

**Cluster Level Detection**
- **K-Means clustering** identifies volume concentration zones within each FRVP
- Configurable cluster count (2–10), iterations, and rows per cluster for POC detection
- Up to 5 individually colored cluster levels with optional labels and right extension

**Alerts**
- Optional sound alerts on BOS and CHoCH events
- Configurable .wav sound files for each event type

---

### 🔴 [RedTail Goldbach Po3 Levels](https://github.com/3astbeast/RedTail-Goldbach-Po3)

A price level indicator for NinjaTrader 8 based on Goldbach number theory applied to Power of 3 (Po3) ranges. Maps mathematically significant price levels — Goldbach primes, semi-primes, inverted Goldbach numbers, and equilibrium points — onto configurable Po3 range partitions anchored from settlement price. Designed for traders who use ICT/Po3 methodology and want precise, mathematical reference levels on their charts.

**Two Operating Modes**
- **Dynamic Mode** — Settlement price sits at the center (50%) of the range, with Goldbach levels mapped symmetrically above and below
- **Fixed Mode** — Calculates the Po3 partition from base 0 that contains the current price, with optional half-shift offset for alignment tuning

**Po3 Range Sizes**
- Preset ranges following the Power of 3 sequence: 3, 9, 27, 81, 243, 729, 2187, 6561, 19683, 59049, 177,147, 531,441
- **Custom range** option for any value
- **Auto-Calculate Po3** — uses the 75th percentile of the Average Daily Range (configurable lookback) to automatically select the optimal Po3 range for the current instrument

**Level Types**
- **Goldbach Levels** — Premium (100, 97, 89, 83, 71, 59, 53) and Discount (0, 3, 11, 17, 29, 41, 47) levels derived from Goldbach prime partitions
- **Non-Goldbach / Semi-Prime Levels** — 23, 35, 65, 77
- **Inverted Goldbach Levels** — 14, 32, 38, 56, 74, 79, 92, 95, 98
- **Midpoint Levels** — Calculated between adjacent main levels
- **Equilibrium (50%)** — Drawn in Fixed mode as a separate reference
- **Premium/Discount area shading** with PD zone labels (High, Rejection, Order Block, FVG, Liq Void, Breaker, Mitigation)

**Po3 Stop Runs & Standard Deviation Levels**
- **Stop run levels** projected beyond the range boundaries at configurable distances (PO3-2, PO3-3, or PO3-4 subdivisions)
- **Standard deviation levels** — evenly spaced interval lines radiating from the fix price, useful for measuring extensions

**Smart Level Merging**
- When multiple levels fall within a configurable tick threshold, they merge into a single labeled line showing all confluent levels — reduces chart clutter while preserving information

**Settlement Detection**
- **Auto-detects settlement times** per instrument (ES/NQ at 4:00 PM, GC at 1:30 PM, CL at 2:30 PM, etc.)
- Manual override for settlement hour/minute and session start time
- Timezone-aware with configurable timezone setting
- Uses 1-minute data series for precise settlement price lookup

**Info Box**
- On-chart info panel showing active Po3 range, fix price, range high/low, calculated ADR, and recommended Po3
- Configurable position (Top Left, Top Right, Bottom Left, Bottom Right) and font size

---

### 🔴 [RedTail Key Levels](https://github.com/3astbeast/RedTail-Key-Levels)

An all-in-one reference level indicator for NinjaTrader 8 that plots pivots, prior session ranges, Monday range, Globex session range, RTH range, and Fibonacci retracements — all on a single chart with smart level merging to keep things clean.

**Pivot Points**
- Standard pivot calculation (PP, R1, R2, R3, S1, S2, S3) with **midline levels** between each (R1M, R2M, R3M, S1M, S2M, S3M)
- Pivot range: **Daily**, **Weekly**, or **Monthly**
- HLC source: **Intraday calculation**, **Daily bars**, or **User-defined values**
- Toggle pivots and midlines independently

**Prior Session Ranges**
- **Previous Day High/Low (PDH/PDL)**
- **Previous Week High/Low (PWH/PWL)**
- **Previous Month High/Low (PMH/PML)**
- Each toggleable independently

**Monday Range**
- Tracks the Monday session (Sunday 6 PM – Monday 5 PM) high and low
- Plots as reference levels for the rest of the week — useful for ICT Monday range concepts

**Globex Session Range**
- Tracks the full Globex week (Sunday 6 PM – Friday 5 PM) high and low
- Plots developing range in real time

**RTH Session Range**
- Tracks the current Regular Trading Hours session (9:30 AM – 4:00 PM ET) high and low
- EST timezone-aware detection

**Fibonacci Levels**
- Up to **10 configurable Fibonacci retracement levels** plotted between the prior session's high and low
- Default levels: 23.6%, 38.2%, 50%, 61.8%, 78.6% (set any to 0.0 to disable)

**Level Merging**
- Configurable **merge tolerance** — when multiple levels from different categories land within the same tick threshold, they collapse into a single line to reduce clutter

**Display**
- All levels exposed as plot outputs (PP, R1–R3, S1–S3, midlines, PDH/PDL, PWH/PWL, PMH/PML, Monday H/L, Globex H/L, NYH/NYL, Fib levels)
- Historical mode toggle to show or hide previous sessions' levels
- Configurable line width for all levels

---

### 🔴 [RedTail Auto-VWAP](https://github.com/3astbeast/RedTail-Auto-VWAP)

An all-in-one VWAP and key level indicator for NinjaTrader 8 that automatically plots multiple anchored VWAPs across every timeframe that matters for futures trading — session, daily, weekly, monthly, yearly — plus Opening Range and Initial Balance zones with full historical lookback.

**VWAP Types**
- **NY Session VWAP** — Anchored to the 9:30 AM ET open, calculates only during RTH (9:30 AM – 4:00 PM ET)
- **Previous Day NY VWAP** — Yesterday's NY Session VWAP, continuing to develop with today's data
- **Session VWAP** — Full futures session (6:00 PM ET start), with optional standard deviation bands
- **Previous Session VWAP** — Prior session's VWAP with optional bands, continuing forward
- **HOD VWAP** — Anchored VWAP from the High of Day, auto-resets on each new high
- **LOD VWAP** — Anchored VWAP from the Low of Day, auto-resets on each new low
- **Monthly VWAP** — Anchored to the start of each calendar month
- **Yearly VWAP** — Anchored to the start of each calendar year
- **HOY VWAP** — Anchored VWAP from the High of Year, auto-resets on each new yearly high

Each VWAP has independent color, line style, and optional standard deviation bands with configurable multiplier.

**Opening Range & Initial Balance**
- **NY Opening Range** — Configurable time window (default 9:30–9:45 AM ET) with high/low lines, fill shading, and text labels
- **Day Initial Balance** — Configurable time window (default 9:30–10:30 AM ET) with independent styling
- Both support **historical lookback** — display previous sessions' ranges on the chart
- **Overlapping level merging** — automatically consolidates OR/IB lines that sit on the same price

**Display & Rendering**
- All VWAPs rendered via SharpDX for smooth, high-performance chart rendering
- Dynamic session coloring option — VWAP line changes color based on whether price is above or below
- VWAP labels with configurable font size
- Full EST timezone-aware session detection that handles both time-based and tick/range charts correctly
- Historical VWAP rendering — display previous sessions' VWAP lines on the chart

**Voice Alerts**
- Built-in text-to-speech voice alert system — generates spoken alerts per instrument (e.g., "MNQ has touched the Session VWAP")
- **Touch alerts** — fires when price crosses a VWAP level
- **Approach alerts** — fires when price is within a configurable tick distance
- Configurable cooldown timer to prevent spam
- Fallback to standard .wav sound files if voice generation fails

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
