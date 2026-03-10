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
  <i>All indicators are free to use. If you find them helpful, donations are always appreciated but never required. If you'd like to use some of my code for a FREE project you're working on, go for it. Under no circumstance is anyone allowed to take my code, re-brand it and sell it for profit. These indicators are meant to be free and open source. Always.</i>
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

### 🔴 [RedTail Toolbar & Drawing Tools](https://github.com/3astbeast/RedTail-Toolbar)

A custom chart toolbar for NinjaTrader 8 that adds quick-access buttons for drawing tools and chart utilities, plus a suite of 6 custom drawing tools that replicate and extend functionality found in platforms like TradingView — all designed for speed during live trading.

**Toolbar Features**
- Installs as a persistent toolbar row at the top of your chart with one-click access to NinjaTrader's drawing tools
- **Configurable tool selection** — choose which drawing tools appear in the toolbar, saved per user
- **Hide/Show all drawings** — toggle all chart drawings visible or hidden with one click
- **Lock/Unlock all drawings** — prevent accidental moves during live trading
- **Indicator visibility manager** — toggle individual indicators on/off directly from the toolbar, with visibility state persisted across sessions
- **ATR display** — live ATR value shown in the toolbar with configurable period
- **Lag timer** — real-time latency monitor with configurable warning (yellow) and critical (red) thresholds
- **Break Even button** — one-click move stop to breakeven with optional tick offset
- **Pan Mode** — toggle free chart dragging without holding Ctrl

**Drawing Tools Included**

**RedTail FRVP Fib** — Click two anchor points to define a range, and the tool builds a full Fixed Range Volume Profile with Fibonacci levels, Anchored VWAP, and K-Means cluster detection inside the zone. Features include POC with Value Area, up to 10 customizable Fib levels with per-level colors, AVWAP with optional right extension, cluster levels (2–10 clusters), adaptive rendering with gradient fill and smoothing, boundary outline, and volume display types (Standard, Bullish, Bearish, Both). Essentially the FRVP indicator's analysis engine in a manual drawing tool.

**RedTail VP Zone** — Draw a selection rectangle on the chart to generate a volume profile within that zone. Displays POC, VAH/VAL lines, optional zone fill with configurable opacity, range high/low extension lines, and a full volume histogram with alignment options. Useful for quick ad-hoc volume analysis on any price range without needing a dedicated indicator.

**RedTail MTF Fib** — A multi-timeframe Fibonacci retracement drawing tool. Click two points to set your swing, then tag it with a timeframe label (e.g., Daily, Weekly, 4H, 1H, or custom). Up to 7 customizable Fib levels with per-level colors, optional price labels, right extension, and configurable line style/width/opacity. The timeframe label makes it easy to distinguish overlapping fibs from different timeframes.

**RedTail Horizontal Line** — An enhanced horizontal line drawing tool with built-in text labels, price display, left/right extension options, and a hover-activated edit button for quick price adjustment via popup. Configurable label position (Left, Right, Center, Above, Below), line color, text color, dash style, width, and opacity.

**RedTail Rectangle** — A feature-rich rectangle drawing tool with mid-line, extension lines, gradient fill, and labels. Fill modes include Solid, Gradient, and None. Optional mid-line with independent color/style and left/right extension. Top and bottom border lines can extend independently in either direction. Built-in label with configurable text and position.

**RedTail Measure Tool** — A comprehensive measurement drawing tool that shows bars & time, price change in points/ticks, dollar value, percentage, velocity (ticks/bar), $/minute, net P&L after commission, volume, delta, and average volume per bar — all within a single measurement overlay. Configurable contract count and round-trip commission for accurate P&L calculation. Auto-colors green for long and red for short. Text placement options inside or outside the zone.

---

### 🔴 [RedTail Volume](https://github.com/3astbeast/RedTail-Volume)

A buy/sell volume separation indicator for NinjaTrader 8 that splits each bar's volume into estimated buying and selling pressure using OHLC proxy ratios, then stacks the winning side on top for instant visual read. Includes Ripster-style volume statistics panels and daily range tracking — all rendered in a dedicated sub-panel below the chart.

**Buy/Sell Volume Separation**
- Uses candle OHLC to calculate a proxy buy/sell ratio for each bar — (Close - Low) / Range for buyers, (High - Close) / Range for sellers
- The **winning side always stacks on top**, giving you an immediate visual indication of who's in control and by how much
- Color-coded bars (default green for buy, red for sell) with the dominant side rendered first

**Volume Statistics Panel**
- **30-Day Average Volume** — calculated from daily bars for a true multi-day reference
- **Today's Cumulative Volume** — running total for the current session
- **% of 30-Day Average** — how today's volume compares to the daily norm
- **30-Bar Average** — rolling average on your current chart timeframe
- **Current Bar Volume** — raw volume for the active bar
- **% of 30-Bar Average** — relative volume for the current bar vs recent bars
- **Buy/Sell Percentage** — real-time split showing the dominant side (winner always displayed on the left for quick reading)
- **Unusual volume highlighting** — configurable threshold (default 200%) changes panel colors when volume spikes above normal

**Daily Range Panel**
- **30-Day Average Range** — calculated from daily bars
- **Today's Range** — current session high minus low
- **% of Average Range** — how today's range compares to the 30-day norm
- Color-coded highlighting when range exceeds thresholds

**Display**
- Optional **SMA overlay** on volume bars with configurable length
- Fully customizable panel colors — separate settings for normal, medium, and high states across both volume and range panels
- Exposed plot outputs for Buy Volume, Sell Volume, and Volume Average — usable by strategies or other indicators

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

### 🔴 [RedTail Auto Fibs](https://github.com/3astbeast/RedTail-Auto-Fibs)

An automatic multi-timeframe Fibonacci retracement indicator for NinjaTrader 8 that plots Daily, Weekly, and Monthly Fibonacci levels from the current period's developing high/low range — no manual drawing required.

**Three Timeframes**
- **Daily Fibs** — Retracements calculated from the current day's high and low, updating in real time as the range develops
- **Weekly Fibs** — Retracements from the current week's high and low
- **Monthly Fibs** — Retracements from the current month's high and low
- Each timeframe toggleable independently

**Per-Level Customization**
- Up to **10 Fibonacci levels per timeframe** (30 total), each with independent enable/disable, value, and color
- Default levels: 0, 23.6%, 38.2%, 50%, 61.8%, 78.6%, 100% with 3 additional extension slots (e.g., -27.2%, -61.8%, -100%)
- Set any level to disabled to remove it from the chart

**Display**
- Independent **line style, line width, and opacity** per timeframe — easily distinguish daily (solid), weekly (dash), and monthly (dash-dot) at a glance
- Optional **price labels** on each level with configurable font size
- Configurable **line extension** beyond current bars and **lookback days** for historical display
- Smart caching to avoid redundant redraws — only updates when high/low values actually change

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

### 🔴 [RedTail Quick Alert](https://github.com/3astbeast/RedTail-Quick-Alert)

A hotkey-driven price alert tool for NinjaTrader 8. Hover your mouse at any price level and press a keyboard shortcut to instantly place an alert line — no dialog boxes, no menus, no clicks on the price axis. Designed for speed during live trading.

**How It Works**
- Hover your cursor at the desired price on the chart and press **Ctrl+A** (default) to set an alert at that exact price
- Alert lines are drawn across the chart with a price label — when price crosses the level, a sound alert fires and optionally a popup appears
- Press **Ctrl+Shift+Delete** (default) to remove all alerts at once
- Click the **X button** rendered on any alert line to remove it individually

**Trigger Modes**
- **Cross** — triggers when price crosses through the alert level in either direction
- **Touch From Above** — triggers only when price approaches from above
- **Touch From Below** — triggers only when price approaches from below

**Alert Options**
- **Persistent alerts** — when enabled, alerts keep firing on each cross instead of one-shot
- Configurable **.wav sound file** and optional NinjaTrader popup on trigger
- Triggered alerts change color (default gray) so you can see which levels have already fired

**Hotkey Customization**
- Configurable alert hotkey letter/key and modifier (None, Ctrl, Shift, Ctrl+Shift)
- Configurable remove-all hotkey and modifier
- Supports letters, function keys, Delete, Space, and other standard keys

**Visual**
- **Crosshair preview line** — shows a dotted line at your cursor position before you commit the alert
- Customizable alert line color, triggered line color, line style, line width, and label font size
- Price labels displayed on the Y-axis panel

---

### 🔴 [RedTail EMA Cloud](https://github.com/3astbeast/RedTail-EMA-Cloud)

A multi-layered moving average cloud indicator for NinjaTrader 8 that renders up to 5 independent EMA (or SMA) clouds on your chart for visual trend identification at a glance. Each cloud is formed by the filled region between a short and long moving average pair, changing color based on bullish or bearish crossover state.

**5 Independent Clouds**
- **Cloud 1** — Default 8/9 (tight scalping cloud)
- **Cloud 2** — Default 5/12 (short-term momentum)
- **Cloud 3** — Default 34/50 (medium-term trend)
- **Cloud 4** — Default 72/89 (intermediate trend)
- **Cloud 5** — Default 180/200 (long-term trend)
- Each cloud can be toggled on/off independently — use any combination that fits your style

**MA Type Selection**
- Switch between **EMA** and **SMA** globally — all 5 clouds recalculate with the selected type

**Visual Customization**
- Independent **bullish and bearish colors** for each cloud
- Independent **opacity** control per cloud (0–100)
- **Cloud leading offset** — shift clouds forward by N bars for anticipatory visual alignment
- Optional **EMA line overlay** — draws the individual short and long MA lines on top of the clouds with rising/falling color coding

**Alerts**
- Optional long and short crossover alerts when the short MA crosses above or below the long MA

---

### 🔴 [RedTail Candle Shadows](https://github.com/3astbeast/RedTail-Candle-Shadows)

A visual enhancement indicator for NinjaTrader 8 that draws subtle drop shadows beneath candle bodies and wicks for improved depth and readability. A small cosmetic touch that makes charts easier on the eyes during long trading sessions.

**Shadow Rendering**
- Shadows drawn on both **candle bodies** and **wicks** (each toggleable independently)
- Configurable **X and Y pixel offset** to control shadow direction and distance
- **Wick opacity multiplier** — wicks get a lighter shadow than bodies for a natural look
- Handles dojis gracefully with a minimum body height

**Optional Blur Effect**
- Simulates a soft shadow with multiple render passes at decreasing opacity
- Configurable **blur passes** (1–3) for varying softness levels

**Display**
- Customizable **shadow color** and **opacity** (1–100%)
- **Body width adjustment** — fine-tune the shadow width relative to the candle
- Rendered via SharpDX for minimal performance impact
- Optimized for scalping on low timeframes

---

### 🔴 [RedTail Swing Anchored VWAP](https://github.com/3astbeast/RedTail-Swing-Anchored-VWAP)

An adaptive swing-anchored VWAP indicator for NinjaTrader 8 that automatically detects swing highs and lows, then anchors a VWAP from each pivot point using EWMA (Exponentially Weighted Moving Average) smoothing instead of traditional cumulative VWAP — producing a smoother, more responsive VWAP that adapts to changing market conditions.

> Converted from Pine Script by Zeiierman (CC BY-NC-SA 4.0).

**How It Works**
- Detects swing highs and lows using a configurable lookback period
- When a swing direction change occurs (new swing high after a series of lows, or vice versa), the VWAP re-anchors from the pivot bar and recalculates forward to the current bar
- Uses EWMA instead of cumulative VWAP — an exponential decay function weights recent volume more heavily, so the VWAP reacts faster to current price action while still respecting the anchor point
- The VWAP line changes color based on the current swing direction: bullish (anchored from a swing low) or bearish (anchored from a swing high)

**Adaptive Price Tracking**
- **Adaptive Price Tracking (APT)** — Controls the EWMA half-life, which determines how quickly the VWAP reacts to new data. Lower values = tighter/faster tracking, higher values = smoother/slower.
- **ATR-Adaptive Mode** — When enabled, the APT value automatically adjusts based on current volatility relative to its own smoothed average. In high-volatility environments the VWAP tightens up; in low-volatility conditions it smooths out. Controlled by a Volatility Bias parameter that sets how aggressively volatility influences the adaptation.

**Historical Display**
- Optionally show all previous VWAP segments from prior swing anchors, creating a visual trail of swing-to-swing VWAPs across the chart

**Visual**
- Independent bullish and bearish VWAP colors
- Configurable line width (1–10), opacity (0–100%), and dash style
- VWAP value exposed as a plot output for use in the data box, crosshair, or by other indicators/strategies
- Rendered via SharpDX for performance

---

### 🔴 [Session Opening Bar Range](https://github.com/3astbeast/Session-Opening-Bar-Range)

A session opening bar range indicator for NinjaTrader 8 that captures the first bar's high, low, and midpoint of a configurable session, then projects those levels forward with optional statistical extensions, range extensions, and OR rotation levels. Useful for opening range breakout strategies and session-based level work.

> Original TradingView Pine Script by **[@notprofessorgreen](https://twitter.com/notprofessorgreen)**. NinjaTrader 8 conversion by @_hawkeye_13.

**Session Presets**
- New York RTH (9:30 AM – 4:00 PM), New York Futures (8:00 AM – 5:00 PM), London (2:00 AM – 8:00 AM), Asia (7:00 PM – 2:00 AM), Midnight to 5 PM, ZB/Gold/Silver OR, CL OR, and Custom with HHMM start/end times
- Configurable timezone with common presets (America/New_York, America/Chicago, Europe/London, Asia/Tokyo)

**Opening Bar Range**
- Configurable OBR timeframe (e.g., 5-minute opening bar) — adds a secondary data series at the specified resolution
- Draws high, low, and optional midline with bullish/bearish color coding based on the opening bar's close vs open
- Projection offset extends levels forward by a configurable number of bars
- Historical session display with configurable max history count

**Statistical Levels**
- Computed from a rolling lookback of historical OR ranges (default 60 periods)
- Two standard deviation bands above and below the OR range, with configurable multipliers (default 1σ and 2σ)
- Independent line color, width, and label size

**Range Extensions**
- Projects the OR range size above and below as equidistant levels
- Configurable range multiplier and number of extension levels (up to 20)
- Independent line color, width, label color, and label size

**OR Rotations**
- 5 rotation levels above and below the OR high/low at a fixed increment (e.g., 65 points for NQ, 15 for ES)
- Independent line color, style, width, label color, and label size

**Rendering**
- All rendering via SharpDX for performance
- Labels positionable on the left or right side of the zone
- Optional price display in labels

---

### 🔴 [Session Statistical Levels](https://github.com/3astbeast/Session-Statistical-Levels)

A percentile-based session range level indicator for NinjaTrader 8 that tracks Asia, London, and NY sessions independently, calculates historical range distributions using percentile statistics, and projects those levels from the current session's open price. Answers the question: "Based on the last N sessions, how far is price likely to move from here?"

> Original TradingView Pine Script by **[@notprofessorgreen](https://twitter.com/notprofessorgreen)**. NinjaTrader 8 conversion by @_hawkeye_13.

**Sessions**
- **Asia** (7:00 PM – 2:00 AM ET), **London** (2:00 AM – 8:00 AM ET), **NY** (8:00 AM – 4:00 PM ET) — each toggleable independently
- **Custom session** with configurable HHMM start/end and custom label
- All times in Eastern Time

**Statistical Levels (from session open)**
- **Median (P50)** — 50th percentile of historical session ranges
- **IQR Band (P25/P75)** — Interquartile range boundaries
- **P10/P90** — 10th and 90th percentile extremes
- **P95** — 95th percentile for identifying historically extreme sessions
- **Mean** — Average range with optional ±1 standard deviation band
- All levels projected symmetrically above and below the session open price
- Each level type independently toggleable

**Directional MAE/MFE Levels**
- Tracks Max Adverse Excursion and Max Favorable Excursion from session open, separated by bullish vs bearish days
- **Bull MFE50/MFE75** — Median and 75th percentile upside on bullish sessions
- **Bull MAE50** — Median downside on bullish sessions
- **Bear MFE50/MFE75** — Median and 75th percentile downside on bearish sessions
- **Bear MAE50** — Median upside on bearish sessions

**Fill Shading**
- Optional IQR and P90 band fills between upper and lower levels for quick visual range assessment

**Stats Table**
- On-chart statistics table showing per-session lookback count, P25/P50/P75/P90 values, Mean/SD, and bull/bear session counts
- Positionable at any corner of the chart

**Configuration**
- Configurable lookback period (default 100 sessions, 50+ recommended)
- Independent color settings for every level type (Median, IQR, P10/P90, P95, Mean, StDev, MAE, MFE) plus fill colors
- Label sizes: Tiny, Small, Normal

---

### 🔴 [Sessions VP with Previous Session VP & Opens](https://github.com/3astbeast/Sessions-VP)

A dual-session volume profile indicator for NinjaTrader 8 that builds volume profiles for both the current and previous session simultaneously, overlays the previous session's profile onto the current session for direct comparison, and plots daily (6 PM ET) and weekly open levels. Supports 8 session types from intraday forex sessions to yearly periods.

> Original TradingView Pine Script by **[@notprofessorgreen](https://twitter.com/notprofgreen)** (lucymatos). NinjaTrader 8 conversion by @_hawkeye_13.

**Dual Volume Profiles**
- **Current Session Profile** — Live-updating volume profile with POC, VAH/VAL, optional Value Area box, session boundary box, and session label. Configurable resolution (5–100 rows), Value Area percentage, and bar mode.
- **Previous Session Profile** — Snapshots the completed prior session's volume profile and overlays it onto the current session's price range for direct visual comparison. Previous POC/VAH/VAL extend forward as dashed lines to the current bar.

**Session Types**
- Tokyo, London, New York, Daily, Weekly, Monthly, Quarterly, and Yearly
- Current and previous session types are independently selectable — you can run a Daily current profile against a Weekly previous profile, for example

**Volume Bar Modes**
- **Mode 1** — Green (up volume) only
- **Mode 2** — Green + Red stacked side by side (default)
- **Mode 3** — Green right, Red left of the session anchor

**Forex Session Boxes**
- Optional Tokyo, London, and New York session range boxes with labels, drawn as dashed boundary rectangles with fill

**Open Levels**
- **6 PM ET Daily Open** — Horizontal line from the first bar at or after 6 PM Eastern, extending to the current bar
- **Weekly Open** — Horizontal line from Sunday 6 PM ET, extending to the current bar
- Both with configurable color, width, line style (Solid/Dashed/Dotted), and optional labels

**Visual**
- Fully independent color, opacity, and line width settings for every element across both current and previous sessions
- Customizable POC/VAH/VAL label text with prefixed "C:" and "P:" to distinguish current vs previous
- All rendering via SharpDX

---

## 🛠 Installation

1. Download the `.cs` file from the indicator's repository
2. Copy the `.cs` to `Documents\NinjaTrader 8\bin\Custom\Indicators`
3. Open NinjaTrader (if not already open)
4. In Control Center, go to **New → NinjaScript Editor**
5. Expand the Indicator tree, find your new indicator, double-click to open it
6. At the top of the Editor window, click the **Compile** button
7. That's it!

> **Note:** Market Structure and Auto VWAP have special installation instructions which can be found in the README in their repositories.

## 📬 Contact

Have questions, feature requests, or bugs to report? Open an **Issue** on the relevant indicator's repo.

---

<p align="center">
  <a href="https://buymeacoffee.com/dmwyzlxstj">
    <img src="https://img.shields.io/badge/☕_Buy_Me_a_Coffee-FFDD00?style=flat-square&logo=buy-me-a-coffee&logoColor=black" alt="Buy Me a Coffee"/>
  </a>
</p>
