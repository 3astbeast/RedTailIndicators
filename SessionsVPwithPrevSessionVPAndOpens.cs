#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Core;
using NinjaTrader.Data;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.NinjaScript;
using NinjaTrader.NinjaScript.DrawingTools;
using SharpDX;
using SharpDX.Direct2D1;
#endregion

// Enums must be in global namespace for NT8 auto-generated partial class compatibility
public enum SessionTypeEnum
{
    Tokyo,
    London,
    NewYork,
    Daily,
    Weekly,
    Monthly,
    Quarterly,
    Yearly
}

public enum BarModeEnum
{
    Mode1,  // Green only
    Mode2,  // Green + Red stacked right
    Mode3   // Green right, Red left
}

public enum LineStyleEnum
{
    Solid,
    Dotted,
    Dashed
}

namespace NinjaTrader.NinjaScript.Indicators
{
    /// <summary>
    /// Sessions & Volume Profile with Previous Session VP Overlay and Daily/Weekly Opens.
    /// Converted from Pine Script by lucymatos — NinjaTrader 8 version by RedTail Indicators.
    /// </summary>
    public class SessionsVPwithPrevSessionVPAndOpens : Indicator
    {

        #region Private Fields

        // --- Current session state ---
        private int curZoneStart;
        private bool curActiveZone;
        private double[] curVpGreen;
        private double[] curVpRed;
        private double[] curZoneBounds;
        private List<double> curLtfO, curLtfC, curLtfH, curLtfL, curLtfV;
        private double curProfHigh, curProfLow;

        // --- Previous session state ---
        private int prevZoneStart;
        private bool prevActiveZone;
        private double[] prevVpGreen;
        private double[] prevVpRed;
        private double[] prevZoneBounds;
        private List<double> prevLtfO, prevLtfC, prevLtfH, prevLtfL, prevLtfV;
        private double prevProfHigh, prevProfLow;

        // --- Snapshot of completed previous session ---
        private double[] snapGreen;
        private double[] snapRed;
        private double[] snapBounds;
        private double snapHigh, snapLow, snapPoc, snapVah, snapVal;
        private bool snapReady;

        // --- Forex session tracking ---
        private int tokyoStart, londonStart, nyStart;
        private bool tokyoActive, londonActive, nyActive;

        // --- Open levels ---
        private double open6pm;
        private double openWeekly;
        private bool open6pmSet;
        private bool openWeeklySet;
        private int open6pmBar;
        private int openWeeklyBar;

        // --- Session detection helpers ---
        private int lastUtcHour;
        private int lastDayOfWeek;
        private int lastWeekOfYear;
        private int lastMonth;
        private int lastYear;
        private int lastDayOfMonth;

        // --- Drawing tag counters ---
        private int drawTagCounter;
        private string curSessionTag;
        private string prevOverlayTag;

        // --- SharpDX resources ---
        private SharpDX.Direct2D1.Brush dxCurBullBrush;
        private SharpDX.Direct2D1.Brush dxCurBearBrush;
        private SharpDX.Direct2D1.Brush dxPrevBullBrush;
        private SharpDX.Direct2D1.Brush dxPrevBearBrush;
        private SharpDX.Direct2D1.Brush dxCurPocBrush;
        private SharpDX.Direct2D1.Brush dxCurVahBrush;
        private SharpDX.Direct2D1.Brush dxCurValBrush;
        private SharpDX.Direct2D1.Brush dxPrevPocBrush;
        private SharpDX.Direct2D1.Brush dxPrevVahBrush;
        private SharpDX.Direct2D1.Brush dxPrevValBrush;
        private SharpDX.Direct2D1.Brush dxCurBoxBrush;
        private SharpDX.Direct2D1.Brush dxCurBoxBgBrush;
        private SharpDX.Direct2D1.Brush dxCurVaBrush;
        private SharpDX.Direct2D1.Brush dxPrevVaBrush;
        private SharpDX.Direct2D1.Brush dxPm6Brush;
        private SharpDX.Direct2D1.Brush dxWeeklyBrush;
        private SharpDX.Direct2D1.Brush dxForexBoxBrush;
        private SharpDX.Direct2D1.Brush dxTextBrush;
        private SharpDX.Direct2D1.Brush dxCurLabelBrush;
        private SharpDX.Direct2D1.Brush dxPrevLabelBrush;
        private SharpDX.DirectWrite.TextFormat textFormatSmall;
        private SharpDX.DirectWrite.TextFormat textFormatLabel;
        private bool brushesValid;

        // --- Completed session drawing data ---
        private List<SessionDrawData> completedSessions;

        // --- Forex completed boxes ---
        private List<ForexBoxData> completedForexBoxes;

        // --- Open level history ---
        private List<OpenLevelData> open6pmLevels;
        private List<OpenLevelData> weeklyOpenLevels;

        #endregion

        #region Data Classes

        private class SessionDrawData
        {
            public int LeftBar;
            public int RightBar;
            public double High;
            public double Low;
            public double Poc;
            public double Vah;
            public double Val;
            public double[] VpGreen;
            public double[] VpRed;
            public double[] ZoneBounds;
            public int Resolution;
            public bool IsCurrent; // false = completed current session draw
            public string SessionType;
        }

        private class ForexBoxData
        {
            public int LeftBar;
            public int RightBar;
            public double High;
            public double Low;
            public string Label;
        }

        private class OpenLevelData
        {
            public int StartBar;
            public double Price;
            public string Label;
        }

        // Snapshot overlay drawn over current session range
        private class PrevOverlayData
        {
            public int LeftBar;
            public int RightBar;
            public double High;
            public double Low;
            public double Poc;
            public double Vah;
            public double Val;
            public double[] VpGreen;
            public double[] VpRed;
            public double[] ZoneBounds;
            public int Resolution;
            public string SessionType;
        }

        private PrevOverlayData prevOverlay;

        #endregion

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description                 = "Sessions & Volume Profile with previous session VP overlay and daily/weekly opens. Converted from Pine Script (lucymatos).";
                Name                        = "Sessions & VP with prev session VP & daily/weekly opens";
                Calculate                   = Calculate.OnBarClose;
                IsOverlay                   = true;
                DisplayInDataBox            = false;
                DrawOnPricePanel            = true;
                ScaleJustification          = ScaleJustification.Right;
                IsSuspendedWhileInactive    = true;
                MaximumBarsLookBack         = MaximumBarsLookBack.Infinite;

                // Current Session defaults
                CurSessionType      = SessionTypeEnum.Daily;
                CurShowProfile      = true;
                CurShowPoc          = true;
                CurShowVA           = true;
                CurShowVABox        = false;
                CurShowLiveZone     = true;
                CurShowSessionBox   = true;
                CurShowSessionLabel = true;
                CurResolution       = 30;
                CurValueAreaPct     = 70;
                CurBarMode          = BarModeEnum.Mode2;

                CurBullColor    = Brushes.Green;
                CurBearColor    = Brushes.Red;
                CurVABoxColor   = Brushes.CornflowerBlue;
                CurPocColor     = Brushes.Red;
                CurPocWidth     = 1;
                CurVahColor     = Brushes.Aqua;
                CurVahWidth     = 1;
                CurValColor     = Brushes.Aqua;
                CurValWidth     = 1;
                CurBoxColor     = Brushes.Orange;
                CurBoxWidth     = 1;

                CurBullOpacity  = 50;
                CurBearOpacity  = 50;
                CurVABoxOpacity = 90;
                CurBoxBgOpacity = 100;

                CurShowLevelLabels = true;
                CurPocLabel     = "POC";
                CurVahLabel     = "VAH";
                CurValLabel     = "VAL";
                CurLabelColor   = Brushes.Gray;

                // Previous Session defaults
                PrevSessionType      = SessionTypeEnum.Daily;
                PrevShowProfile      = true;
                PrevShowPoc          = true;
                PrevShowVA           = true;
                PrevShowVABox        = false;
                PrevShowOverlay      = true;
                PrevShowSessionLabel = true;
                PrevResolution       = 30;
                PrevValueAreaPct     = 70;
                PrevBarMode          = BarModeEnum.Mode2;

                PrevBullColor    = Brushes.Blue;
                PrevBearColor    = Brushes.Purple;
                PrevVABoxColor   = Brushes.CornflowerBlue;
                PrevPocColor     = Brushes.Red;
                PrevPocWidth     = 1;
                PrevVahColor     = Brushes.Aqua;
                PrevVahWidth     = 1;
                PrevValColor     = Brushes.Aqua;
                PrevValWidth     = 1;

                PrevBullOpacity  = 75;
                PrevBearOpacity  = 75;
                PrevVABoxOpacity = 90;
                PrevPocOpacity   = 20;
                PrevVahOpacity   = 20;
                PrevValOpacity   = 20;

                PrevShowLevelLabels = true;
                PrevPocLabel     = "POC";
                PrevVahLabel     = "VAH";
                PrevValLabel     = "VAL";
                PrevLabelColor   = Brushes.Gray;

                // Forex
                ShowForexBoxes = false;

                // Open Levels
                Show6pmOpen     = true;
                Pm6Color        = Brushes.DimGray;
                Pm6Width        = 1;
                Pm6Style        = LineStyleEnum.Dashed;
                Show6pmLabel    = true;

                ShowWeeklyOpen  = true;
                WeeklyOpenColor = Brushes.DimGray;
                WeeklyOpenWidth = 2;
                WeeklyOpenStyle = LineStyleEnum.Dashed;
                ShowWeeklyLabel = true;
            }
            else if (State == State.Configure)
            {
                ClearOutputWindow();
            }
            else if (State == State.DataLoaded)
            {
                InitializeArrays();
            }
            else if (State == State.Terminated)
            {
                DisposeResources();
            }
        }

        private void InitializeArrays()
        {
            curVpGreen    = new double[CurResolution];
            curVpRed      = new double[CurResolution];
            curZoneBounds = new double[CurResolution];
            curLtfO = new List<double>();
            curLtfC = new List<double>();
            curLtfH = new List<double>();
            curLtfL = new List<double>();
            curLtfV = new List<double>();

            prevVpGreen    = new double[PrevResolution];
            prevVpRed      = new double[PrevResolution];
            prevZoneBounds = new double[PrevResolution];
            prevLtfO = new List<double>();
            prevLtfC = new List<double>();
            prevLtfH = new List<double>();
            prevLtfL = new List<double>();
            prevLtfV = new List<double>();

            snapGreen  = new double[PrevResolution];
            snapRed    = new double[PrevResolution];
            snapBounds = new double[PrevResolution];
            snapHigh   = double.NaN;
            snapLow    = double.NaN;
            snapPoc    = double.NaN;
            snapVah    = double.NaN;
            snapVal    = double.NaN;
            snapReady  = false;

            completedSessions  = new List<SessionDrawData>();
            completedForexBoxes = new List<ForexBoxData>();
            open6pmLevels      = new List<OpenLevelData>();
            weeklyOpenLevels   = new List<OpenLevelData>();

            curZoneStart   = 0;
            curActiveZone  = false;
            prevZoneStart  = 0;
            prevActiveZone = false;

            tokyoStart  = 0;
            londonStart = 0;
            nyStart     = 0;

            open6pm      = double.NaN;
            openWeekly   = double.NaN;
            open6pmSet   = false;
            openWeeklySet = false;

            lastUtcHour   = -1;
            lastDayOfWeek = -1;
            lastWeekOfYear = -1;
            lastMonth     = -1;
            lastYear      = -1;
            lastDayOfMonth = -1;

            drawTagCounter = 0;
            prevOverlay    = null;
        }

        private void DisposeResources()
        {
            if (dxCurBullBrush != null) { dxCurBullBrush.Dispose(); dxCurBullBrush = null; }
            if (dxCurBearBrush != null) { dxCurBearBrush.Dispose(); dxCurBearBrush = null; }
            if (dxPrevBullBrush != null) { dxPrevBullBrush.Dispose(); dxPrevBullBrush = null; }
            if (dxPrevBearBrush != null) { dxPrevBearBrush.Dispose(); dxPrevBearBrush = null; }
            if (dxCurPocBrush != null) { dxCurPocBrush.Dispose(); dxCurPocBrush = null; }
            if (dxCurVahBrush != null) { dxCurVahBrush.Dispose(); dxCurVahBrush = null; }
            if (dxCurValBrush != null) { dxCurValBrush.Dispose(); dxCurValBrush = null; }
            if (dxPrevPocBrush != null) { dxPrevPocBrush.Dispose(); dxPrevPocBrush = null; }
            if (dxPrevVahBrush != null) { dxPrevVahBrush.Dispose(); dxPrevVahBrush = null; }
            if (dxPrevValBrush != null) { dxPrevValBrush.Dispose(); dxPrevValBrush = null; }
            if (dxCurBoxBrush != null) { dxCurBoxBrush.Dispose(); dxCurBoxBrush = null; }
            if (dxCurBoxBgBrush != null) { dxCurBoxBgBrush.Dispose(); dxCurBoxBgBrush = null; }
            if (dxCurVaBrush != null) { dxCurVaBrush.Dispose(); dxCurVaBrush = null; }
            if (dxPrevVaBrush != null) { dxPrevVaBrush.Dispose(); dxPrevVaBrush = null; }
            if (dxPm6Brush != null) { dxPm6Brush.Dispose(); dxPm6Brush = null; }
            if (dxWeeklyBrush != null) { dxWeeklyBrush.Dispose(); dxWeeklyBrush = null; }
            if (dxForexBoxBrush != null) { dxForexBoxBrush.Dispose(); dxForexBoxBrush = null; }
            if (dxTextBrush != null) { dxTextBrush.Dispose(); dxTextBrush = null; }
            if (dxCurLabelBrush != null) { dxCurLabelBrush.Dispose(); dxCurLabelBrush = null; }
            if (dxPrevLabelBrush != null) { dxPrevLabelBrush.Dispose(); dxPrevLabelBrush = null; }
            if (textFormatSmall != null) { textFormatSmall.Dispose(); textFormatSmall = null; }
            if (textFormatLabel != null) { textFormatLabel.Dispose(); textFormatLabel = null; }
            brushesValid = false;
        }

        #region Session Detection

        private int GetUtcHour(DateTime time)
        {
            return time.ToUniversalTime().Hour;
        }

        private int GetWeekOfYear(DateTime time)
        {
            return System.Globalization.CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                time, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        private bool DetectNewSession(SessionTypeEnum sessionType, DateTime barTime, DateTime prevBarTime)
        {
            int curUtcHour = GetUtcHour(barTime);
            int prevUtcHour = GetUtcHour(prevBarTime);

            switch (sessionType)
            {
                case SessionTypeEnum.Tokyo:
                    // New Tokyo: UTC hour discontinuity (session starts at 0:00 UTC)
                    return curUtcHour != prevUtcHour + 1 && curUtcHour != prevUtcHour && barTime.Date != prevBarTime.Date;
                case SessionTypeEnum.London:
                    return curUtcHour >= 7 && prevUtcHour < 7;
                case SessionTypeEnum.NewYork:
                    return curUtcHour >= 13 && prevUtcHour < 13;
                case SessionTypeEnum.Daily:
                    return barTime.DayOfWeek != prevBarTime.DayOfWeek;
                case SessionTypeEnum.Weekly:
                    return GetWeekOfYear(barTime) != GetWeekOfYear(prevBarTime);
                case SessionTypeEnum.Monthly:
                    return barTime.Month != prevBarTime.Month;
                case SessionTypeEnum.Quarterly:
                    return barTime.Month != prevBarTime.Month && (barTime.Month - 1) % 3 == 0;
                case SessionTypeEnum.Yearly:
                    return barTime.Year != prevBarTime.Year;
                default:
                    return barTime.DayOfWeek != prevBarTime.DayOfWeek;
            }
        }

        private bool DetectEndSession(SessionTypeEnum sessionType, DateTime barTime, DateTime prevBarTime)
        {
            int curUtcHour = GetUtcHour(barTime);
            int prevUtcHour = GetUtcHour(prevBarTime);

            switch (sessionType)
            {
                case SessionTypeEnum.Tokyo:
                    return curUtcHour >= 9 && prevUtcHour < 9;
                case SessionTypeEnum.London:
                    return curUtcHour >= 16 && prevUtcHour < 16;
                case SessionTypeEnum.NewYork:
                    return curUtcHour >= 22 && prevUtcHour < 22;
                case SessionTypeEnum.Daily:
                    return barTime.DayOfWeek != prevBarTime.DayOfWeek;
                case SessionTypeEnum.Weekly:
                    return GetWeekOfYear(barTime) != GetWeekOfYear(prevBarTime);
                case SessionTypeEnum.Monthly:
                    return barTime.Month != prevBarTime.Month;
                case SessionTypeEnum.Quarterly:
                    return barTime.Month != prevBarTime.Month && (barTime.Month - 1) % 3 == 0;
                case SessionTypeEnum.Yearly:
                    return barTime.Year != prevBarTime.Year;
                default:
                    return barTime.DayOfWeek != prevBarTime.DayOfWeek;
            }
        }

        // Forex session detection
        private bool DetectNewTokyo(DateTime barTime, DateTime prevBarTime)
        {
            int curUtcHour = GetUtcHour(barTime);
            int prevUtcHour = GetUtcHour(prevBarTime);
            return curUtcHour != prevUtcHour + 1 && curUtcHour != prevUtcHour && barTime.Date != prevBarTime.Date;
        }

        private bool DetectEndTokyo(DateTime barTime, DateTime prevBarTime)
        {
            return GetUtcHour(barTime) >= 9 && GetUtcHour(prevBarTime) < 9;
        }

        private bool DetectNewLondon(DateTime barTime, DateTime prevBarTime)
        {
            return GetUtcHour(barTime) >= 7 && GetUtcHour(prevBarTime) < 7;
        }

        private bool DetectEndLondon(DateTime barTime, DateTime prevBarTime)
        {
            return GetUtcHour(barTime) >= 16 && GetUtcHour(prevBarTime) < 16;
        }

        private bool DetectNewNY(DateTime barTime, DateTime prevBarTime)
        {
            return GetUtcHour(barTime) >= 13 && GetUtcHour(prevBarTime) < 13;
        }

        private bool DetectEndNY(DateTime barTime, DateTime prevBarTime)
        {
            return GetUtcHour(barTime) >= 22 && GetUtcHour(prevBarTime) < 22;
        }

        #endregion

        #region Volume Profile Calculation

        private double GetVolOverlap(double y11, double y12, double y21, double y22, double height, double v)
        {
            if (height <= 0) return 0;
            double overlap = Math.Min(Math.Max(y11, y12), Math.Max(y21, y22))
                           - Math.Max(Math.Min(y11, y12), Math.Min(y21, y22));
            return Math.Max(overlap, 0) * v / height;
        }

        private void ProfileAdd(double o, double h, double l, double c, double v,
            double gap, double[] vpGreen, double[] vpRed, double[] zBounds, int res)
        {
            double bodyTop = Math.Max(c, o);
            double bodyBot = Math.Min(c, o);
            bool isGreen = c >= o;
            double topWick = h - bodyTop;
            double botWick = bodyBot - l;
            double body = bodyTop - bodyBot;
            double denom = 2 * topWick + 2 * botWick + body;

            double bodyVol = denom > 0 ? body * v / denom : 0;
            double topWVol = denom > 0 ? 2 * topWick * v / denom : 0;
            double botWVol = denom > 0 ? 2 * botWick * v / denom : 0;

            for (int i = 0; i < res; i++)
            {
                double zTop = zBounds[i];
                double zBot = zTop - gap;

                double gAdd = (isGreen ? GetVolOverlap(zBot, zTop, bodyBot, bodyTop, body, bodyVol) : 0)
                            + GetVolOverlap(zBot, zTop, bodyTop, h, topWick, topWVol) / 2.0
                            + GetVolOverlap(zBot, zTop, bodyBot, l, botWick, botWVol) / 2.0;

                double rAdd = (!isGreen ? GetVolOverlap(zBot, zTop, bodyBot, bodyTop, body, bodyVol) : 0)
                            + GetVolOverlap(zBot, zTop, bodyTop, h, topWick, topWVol) / 2.0
                            + GetVolOverlap(zBot, zTop, bodyBot, l, botWick, botWVol) / 2.0;

                vpGreen[i] += gAdd;
                vpRed[i]   += rAdd;
            }
        }

        private void CalcSession(double pH, double pL, int res,
            List<double> ltfO, List<double> ltfC, List<double> ltfH, List<double> ltfL, List<double> ltfV,
            double[] vpGreen, double[] vpRed, double[] zBounds)
        {
            // Clear
            Array.Clear(vpGreen, 0, res);
            Array.Clear(vpRed, 0, res);

            if (double.IsNaN(pH) || double.IsNaN(pL) || pH == pL || ltfO.Count == 0)
                return;

            double gap = (pH - pL) / res;
            for (int i = 0; i < res; i++)
                zBounds[i] = pH - gap * i;

            for (int j = 0; j < ltfO.Count; j++)
            {
                ProfileAdd(ltfO[j], ltfH[j], ltfL[j], ltfC[j], ltfV[j],
                    gap, vpGreen, vpRed, zBounds, res);
            }
        }

        private double CalcPocLevel(double[] vpGreen, double[] vpRed, double[] zBounds, int res)
        {
            double maxVol = 0;
            int ind = 0;
            for (int i = 0; i < res; i++)
            {
                double tot = vpGreen[i] + vpRed[i];
                if (tot > maxVol)
                {
                    maxVol = tot;
                    ind = i;
                }
            }
            if (ind < res - 1)
                return zBounds[ind] - (zBounds[ind] - zBounds[ind + 1]) / 2.0;
            return double.NaN;
        }

        private void CalcValueArea(double poc, double pH, double pL, double[] vpGreen, double[] vpRed,
            double[] zBounds, int res, int vaWidPct, out double val, out double vah)
        {
            double gap = (pH - pL) / res;
            double volSum = vpGreen.Sum() + vpRed.Sum();
            double volCnt = 0;
            vah = pH;
            val = pL;

            int pocInd = 0;
            for (int i = 0; i < res - 1; i++)
            {
                if (zBounds[i] >= poc && zBounds[i + 1] < poc)
                {
                    pocInd = i;
                    break;
                }
            }

            volCnt += vpRed[pocInd] + vpGreen[pocInd];

            for (int i = 1; i < res; i++)
            {
                if (pocInd + i < res)
                {
                    volCnt += vpRed[pocInd + i] + vpGreen[pocInd + i];
                    if (volCnt >= volSum * (vaWidPct / 100.0))
                        break;
                    else
                        val = zBounds[pocInd + i] - gap;
                }
                if (pocInd - i >= 0)
                {
                    volCnt += vpRed[pocInd - i] + vpGreen[pocInd - i];
                    if (volCnt >= volSum * (vaWidPct / 100.0))
                        break;
                    else
                        vah = zBounds[pocInd - i];
                }
            }
        }

        private void ResetProfile(double[] vpGreen, double[] vpRed,
            List<double> ltfO, List<double> ltfH, List<double> ltfL, List<double> ltfC, List<double> ltfV)
        {
            Array.Clear(vpGreen, 0, vpGreen.Length);
            Array.Clear(vpRed, 0, vpRed.Length);
            ltfO.Clear();
            ltfH.Clear();
            ltfL.Clear();
            ltfC.Clear();
            ltfV.Clear();
        }

        private double GetHighestHigh(int startBar, int endBar)
        {
            double highest = double.MinValue;
            for (int i = startBar; i <= endBar; i++)
            {
                int barsAgo = CurrentBar - i;
                if (barsAgo >= 0 && barsAgo < CurrentBar)
                {
                    double h = High.GetValueAt(i);
                    if (h > highest) highest = h;
                }
            }
            return highest == double.MinValue ? double.NaN : highest;
        }

        private double GetLowestLow(int startBar, int endBar)
        {
            double lowest = double.MaxValue;
            for (int i = startBar; i <= endBar; i++)
            {
                int barsAgo = CurrentBar - i;
                if (barsAgo >= 0 && barsAgo < CurrentBar)
                {
                    double l = Low.GetValueAt(i);
                    if (l < lowest) lowest = l;
                }
            }
            return lowest == double.MaxValue ? double.NaN : lowest;
        }

        #endregion

        #region OnBarUpdate

        protected override void OnBarUpdate()
        {
            if (CurrentBar < 2) return;

            DateTime barTime = Time[0];
            DateTime prevBarTime = Time[1];

            // ============ Session Detection ============
            bool curNew  = DetectNewSession(CurSessionType, barTime, prevBarTime);
            bool curEnd  = DetectEndSession(CurSessionType, barTime, prevBarTime);
            bool prevNew = DetectNewSession(PrevSessionType, barTime, prevBarTime);
            bool prevEnd = DetectEndSession(PrevSessionType, barTime, prevBarTime);

            // Forex session detection
            bool newTokyo  = DetectNewTokyo(barTime, prevBarTime);
            bool endTokyo  = DetectEndTokyo(barTime, prevBarTime);
            bool newLondon = DetectNewLondon(barTime, prevBarTime);
            bool endLondon = DetectEndLondon(barTime, prevBarTime);
            bool newNY     = DetectNewNY(barTime, prevBarTime);
            bool endNY     = DetectEndNY(barTime, prevBarTime);

            // ============ Open Levels (6 PM / Weekly) ============
            // Convert bar time to Eastern
            TimeZoneInfo eastern = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime barTimeET = TimeZoneInfo.ConvertTimeFromUtc(barTime.ToUniversalTime(), eastern);
            DateTime prevBarTimeET = TimeZoneInfo.ConvertTimeFromUtc(prevBarTime.ToUniversalTime(), eastern);

            // 6 PM open: first bar at or after 6 PM ET
            if (barTimeET.Hour >= 18 && prevBarTimeET.Hour < 18)
            {
                if (!double.IsNaN(open6pm) && open6pmSet)
                {
                    // Save previous level
                    open6pmLevels.Add(new OpenLevelData { StartBar = open6pmBar, Price = open6pm, Label = "6pm open" });
                    // Keep only recent
                    if (open6pmLevels.Count > 5) open6pmLevels.RemoveAt(0);
                }
                open6pm = Open[0];
                open6pmSet = true;
                open6pmBar = CurrentBar;
            }

            // Weekly open: first bar at or after 6 PM ET on Sunday
            if (barTimeET.DayOfWeek == DayOfWeek.Sunday && barTimeET.Hour >= 18
                && !(prevBarTimeET.DayOfWeek == DayOfWeek.Sunday && prevBarTimeET.Hour >= 18))
            {
                if (!double.IsNaN(openWeekly) && openWeeklySet)
                {
                    weeklyOpenLevels.Add(new OpenLevelData { StartBar = openWeeklyBar, Price = openWeekly, Label = "Weekly open" });
                    if (weeklyOpenLevels.Count > 3) weeklyOpenLevels.RemoveAt(0);
                }
                openWeekly = Open[0];
                openWeeklySet = true;
                openWeeklyBar = CurrentBar;
            }

            // ============ Forex Tracking ============
            if (newTokyo)  { tokyoStart  = CurrentBar; tokyoActive  = true; }
            if (newLondon) { londonStart = CurrentBar; londonActive = true; }
            if (newNY)     { nyStart     = CurrentBar; nyActive     = true; }

            if (endTokyo && tokyoActive && ShowForexBoxes)
            {
                double fxH = GetHighestHigh(tokyoStart, CurrentBar - 1);
                double fxL = GetLowestLow(tokyoStart, CurrentBar - 1);
                completedForexBoxes.Add(new ForexBoxData { LeftBar = tokyoStart, RightBar = CurrentBar - 1, High = fxH, Low = fxL, Label = "Tokyo" });
                tokyoActive = false;
            }
            if (endLondon && londonActive && ShowForexBoxes)
            {
                double fxH = GetHighestHigh(londonStart, CurrentBar - 1);
                double fxL = GetLowestLow(londonStart, CurrentBar - 1);
                completedForexBoxes.Add(new ForexBoxData { LeftBar = londonStart, RightBar = CurrentBar - 1, High = fxH, Low = fxL, Label = "London" });
                londonActive = false;
            }
            if (endNY && nyActive && ShowForexBoxes)
            {
                double fxH = GetHighestHigh(nyStart, CurrentBar - 1);
                double fxL = GetLowestLow(nyStart, CurrentBar - 1);
                completedForexBoxes.Add(new ForexBoxData { LeftBar = nyStart, RightBar = CurrentBar - 1, High = fxH, Low = fxL, Label = "New York" });
                nyActive = false;
            }

            // ============ Current Session End — Finalize Profile ============
            if (curEnd && curActiveZone)
            {
                curProfHigh = GetHighestHigh(curZoneStart, CurrentBar - 1);
                curProfLow  = GetLowestLow(curZoneStart, CurrentBar - 1);

                CalcSession(curProfHigh, curProfLow, CurResolution,
                    curLtfO, curLtfC, curLtfH, curLtfL, curLtfV,
                    curVpGreen, curVpRed, curZoneBounds);

                double totalVol = curVpGreen.Sum() + curVpRed.Sum();
                if (totalVol > 0)
                {
                    double poc = CalcPocLevel(curVpGreen, curVpRed, curZoneBounds, CurResolution);
                    CalcValueArea(poc, curProfHigh, curProfLow, curVpGreen, curVpRed, curZoneBounds, CurResolution, CurValueAreaPct, out double val, out double vah);

                    completedSessions.Add(new SessionDrawData
                    {
                        LeftBar     = curZoneStart,
                        RightBar    = CurrentBar - 1,
                        High        = curProfHigh,
                        Low         = curProfLow,
                        Poc         = poc,
                        Vah         = vah,
                        Val         = val,
                        VpGreen     = (double[])curVpGreen.Clone(),
                        VpRed       = (double[])curVpRed.Clone(),
                        ZoneBounds  = (double[])curZoneBounds.Clone(),
                        Resolution  = CurResolution,
                        IsCurrent   = false,
                        SessionType = CurSessionType.ToString()
                    });

                    // Keep max 20 historical sessions to avoid memory bloat
                    if (completedSessions.Count > 20)
                        completedSessions.RemoveAt(0);
                }

                curActiveZone = false;
            }

            // ============ Previous Session End — Snapshot ============
            if (prevEnd && prevActiveZone)
            {
                prevProfHigh = GetHighestHigh(prevZoneStart, CurrentBar - 1);
                prevProfLow  = GetLowestLow(prevZoneStart, CurrentBar - 1);

                CalcSession(prevProfHigh, prevProfLow, PrevResolution,
                    prevLtfO, prevLtfC, prevLtfH, prevLtfL, prevLtfV,
                    prevVpGreen, prevVpRed, prevZoneBounds);

                double totalVol = prevVpGreen.Sum() + prevVpRed.Sum();
                if (totalVol > 0)
                {
                    // Snapshot for overlay
                    Array.Copy(prevVpGreen, snapGreen, PrevResolution);
                    Array.Copy(prevVpRed, snapRed, PrevResolution);
                    Array.Copy(prevZoneBounds, snapBounds, PrevResolution);
                    snapHigh = prevProfHigh;
                    snapLow  = prevProfLow;
                    snapPoc  = CalcPocLevel(prevVpGreen, prevVpRed, prevZoneBounds, PrevResolution);
                    CalcValueArea(snapPoc, prevProfHigh, prevProfLow, prevVpGreen, prevVpRed, prevZoneBounds,
                        PrevResolution, PrevValueAreaPct, out double sv, out double svh);
                    snapVal   = sv;
                    snapVah   = svh;
                    snapReady = true;
                }

                prevActiveZone = false;
            }

            // ============ New Session — Reset ============
            if (curNew)
            {
                ResetProfile(curVpGreen, curVpRed, curLtfO, curLtfH, curLtfL, curLtfC, curLtfV);
                curZoneStart  = CurrentBar;
                curActiveZone = true;
            }

            if (prevNew)
            {
                ResetProfile(prevVpGreen, prevVpRed, prevLtfO, prevLtfH, prevLtfL, prevLtfC, prevLtfV);
                prevZoneStart  = CurrentBar;
                prevActiveZone = true;
            }

            // ============ Accumulate bar data ============
            if (curActiveZone)
            {
                curLtfO.Add(Open[0]);
                curLtfH.Add(High[0]);
                curLtfL.Add(Low[0]);
                curLtfC.Add(Close[0]);
                curLtfV.Add(Volume[0]);
            }

            if (prevActiveZone)
            {
                prevLtfO.Add(Open[0]);
                prevLtfH.Add(High[0]);
                prevLtfL.Add(Low[0]);
                prevLtfC.Add(Close[0]);
                prevLtfV.Add(Volume[0]);
            }

            // ============ Update current session live profile ============
            if (curActiveZone)
            {
                curProfHigh = GetHighestHigh(curZoneStart, CurrentBar);
                curProfLow  = GetLowestLow(curZoneStart, CurrentBar);

                CalcSession(curProfHigh, curProfLow, CurResolution,
                    curLtfO, curLtfC, curLtfH, curLtfL, curLtfV,
                    curVpGreen, curVpRed, curZoneBounds);
            }

            // ============ Build previous overlay data ============
            if (PrevShowOverlay && snapReady && curActiveZone)
            {
                prevOverlay = new PrevOverlayData
                {
                    LeftBar     = curZoneStart,
                    RightBar    = CurrentBar,
                    High        = snapHigh,
                    Low         = snapLow,
                    Poc         = snapPoc,
                    Vah         = snapVah,
                    Val         = snapVal,
                    VpGreen     = (double[])snapGreen.Clone(),
                    VpRed       = (double[])snapRed.Clone(),
                    ZoneBounds  = (double[])snapBounds.Clone(),
                    Resolution  = PrevResolution,
                    SessionType = PrevSessionType.ToString()
                };
            }
        }

        #endregion

        #region OnRender (SharpDX)

        public override void OnRenderTargetChanged()
        {
            DisposeResources();
            brushesValid = false;
        }

        private void EnsureBrushes(RenderTarget rt)
        {
            if (brushesValid) return;

            dxCurBullBrush  = CreateBrush(rt, CurBullColor, CurBullOpacity);
            dxCurBearBrush  = CreateBrush(rt, CurBearColor, CurBearOpacity);
            dxPrevBullBrush = CreateBrush(rt, PrevBullColor, PrevBullOpacity);
            dxPrevBearBrush = CreateBrush(rt, PrevBearColor, PrevBearOpacity);
            dxCurPocBrush   = CreateBrush(rt, CurPocColor, 0);
            dxCurVahBrush   = CreateBrush(rt, CurVahColor, 0);
            dxCurValBrush   = CreateBrush(rt, CurValColor, 0);
            dxPrevPocBrush  = CreateBrush(rt, PrevPocColor, PrevPocOpacity);
            dxPrevVahBrush  = CreateBrush(rt, PrevVahColor, PrevVahOpacity);
            dxPrevValBrush  = CreateBrush(rt, PrevValColor, PrevValOpacity);
            dxCurBoxBrush   = CreateBrush(rt, CurBoxColor, 0);
            dxCurBoxBgBrush = CreateBrush(rt, CurBoxColor, CurBoxBgOpacity);
            dxCurVaBrush    = CreateBrush(rt, CurVABoxColor, CurVABoxOpacity);
            dxPrevVaBrush   = CreateBrush(rt, PrevVABoxColor, PrevVABoxOpacity);
            dxPm6Brush      = CreateBrush(rt, Pm6Color, 0);
            dxWeeklyBrush   = CreateBrush(rt, WeeklyOpenColor, 0);
            dxForexBoxBrush = CreateBrush(rt, CurBoxColor, CurBoxBgOpacity);
            dxTextBrush     = new SharpDX.Direct2D1.SolidColorBrush(rt, SharpDX.Color.White);
            dxCurLabelBrush = CreateBrush(rt, CurLabelColor, 0);
            dxPrevLabelBrush = CreateBrush(rt, PrevLabelColor, 0);

            textFormatSmall = new SharpDX.DirectWrite.TextFormat(
                NinjaTrader.Core.Globals.DirectWriteFactory, "Arial", 10);
            textFormatLabel = new SharpDX.DirectWrite.TextFormat(
                NinjaTrader.Core.Globals.DirectWriteFactory, "Arial", 11);

            brushesValid = true;
        }

        private SharpDX.Direct2D1.Brush CreateBrush(RenderTarget rt, System.Windows.Media.Brush wpfBrush, int opacityPct)
        {
            var scb = wpfBrush as System.Windows.Media.SolidColorBrush;
            if (scb == null) scb = System.Windows.Media.Brushes.White as System.Windows.Media.SolidColorBrush;
            var c = scb.Color;
            float alpha = (100 - opacityPct) / 100f; // Pine opacity: 0=opaque, 100=transparent
            return new SharpDX.Direct2D1.SolidColorBrush(rt,
                new SharpDX.Color(c.R, c.G, c.B, (byte)(alpha * 255)));
        }

        protected override void OnRender(ChartControl chartControl, ChartScale chartScale)
        {
            if (chartControl == null || chartScale == null) return;
            var rt = RenderTarget;
            if (rt == null) return;

            EnsureBrushes(rt);

            // Draw completed sessions
            foreach (var s in completedSessions)
            {
                DrawVolumeProfile(rt, chartControl, chartScale, s.LeftBar, s.RightBar,
                    s.High, s.Low, s.Poc, s.Vah, s.Val,
                    s.VpGreen, s.VpRed, s.ZoneBounds, s.Resolution,
                    dxCurBullBrush, dxCurBearBrush, dxCurPocBrush, CurPocWidth,
                    dxCurVahBrush, CurVahWidth, dxCurValBrush, CurValWidth,
                    dxCurBoxBrush, dxCurBoxBgBrush, CurBoxWidth, dxCurVaBrush,
                    CurShowProfile, CurShowPoc, CurShowVA, CurShowVABox,
                    CurShowSessionBox, CurShowSessionLabel, CurBarMode,
                    s.SessionType, false,
                    CurShowLevelLabels, "C: " + CurPocLabel, "C: " + CurVahLabel, "C: " + CurValLabel, dxCurLabelBrush);
            }

            // Draw current live session
            if (curActiveZone && CurShowLiveZone)
            {
                double totalVol = curVpGreen.Sum() + curVpRed.Sum();
                if (totalVol > 0)
                {
                    double poc = CalcPocLevel(curVpGreen, curVpRed, curZoneBounds, CurResolution);
                    CalcValueArea(poc, curProfHigh, curProfLow, curVpGreen, curVpRed, curZoneBounds,
                        CurResolution, CurValueAreaPct, out double val, out double vah);

                    DrawVolumeProfile(rt, chartControl, chartScale,
                        curZoneStart, CurrentBar,
                        curProfHigh, curProfLow, poc, vah, val,
                        curVpGreen, curVpRed, curZoneBounds, CurResolution,
                        dxCurBullBrush, dxCurBearBrush, dxCurPocBrush, CurPocWidth,
                        dxCurVahBrush, CurVahWidth, dxCurValBrush, CurValWidth,
                        dxCurBoxBrush, dxCurBoxBgBrush, CurBoxWidth, dxCurVaBrush,
                        CurShowProfile, CurShowPoc, CurShowVA, CurShowVABox,
                        CurShowSessionBox, CurShowSessionLabel, CurBarMode,
                        CurSessionType.ToString(), true,
                        CurShowLevelLabels, "C: " + CurPocLabel, "C: " + CurVahLabel, "C: " + CurValLabel, dxCurLabelBrush);
                }
            }

            // Draw previous session overlay
            if (prevOverlay != null && PrevShowOverlay && curActiveZone)
            {
                DrawPrevSessionOverlay(rt, chartControl, chartScale);
            }

            // Draw forex boxes
            if (ShowForexBoxes)
            {
                foreach (var fb in completedForexBoxes)
                {
                    DrawForexBox(rt, chartControl, chartScale, fb);
                }
            }

            // Draw open levels
            if (Show6pmOpen && open6pmSet)
            {
                DrawOpenLevel(rt, chartControl, chartScale, open6pmBar, open6pm, Pm6Width,
                    dxPm6Brush, Pm6Style, Show6pmLabel ? "6pm open" : null);
            }
            if (ShowWeeklyOpen && openWeeklySet)
            {
                DrawOpenLevel(rt, chartControl, chartScale, openWeeklyBar, openWeekly, WeeklyOpenWidth,
                    dxWeeklyBrush, WeeklyOpenStyle, ShowWeeklyLabel ? "Weekly open" : null);
            }
        }

        private void DrawVolumeProfile(RenderTarget rt, ChartControl chartControl, ChartScale chartScale,
            int leftBar, int rightBar, double pH, double pL, double poc, double vah, double val,
            double[] vpGreen, double[] vpRed, double[] zBounds, int res,
            SharpDX.Direct2D1.Brush bullBrush, SharpDX.Direct2D1.Brush bearBrush,
            SharpDX.Direct2D1.Brush pocBrush, int pocWidth,
            SharpDX.Direct2D1.Brush vahBrush, int vahWidth,
            SharpDX.Direct2D1.Brush valBrush, int valWidth,
            SharpDX.Direct2D1.Brush boxBrush, SharpDX.Direct2D1.Brush boxBgBrush, int boxWidth,
            SharpDX.Direct2D1.Brush vaBrush,
            bool showProf, bool showPoc, bool showVA, bool showVABox,
            bool showBox, bool showLabel, BarModeEnum barMode,
            string sessionType, bool isLive,
            bool showLevelLabels, string pocText, string vahText, string valText,
            SharpDX.Direct2D1.Brush labelBrush)
        {
            float xLeft  = chartControl.GetXByBarIndex(chartControl.BarsArray[0], leftBar);
            float xRight = chartControl.GetXByBarIndex(chartControl.BarsArray[0], rightBar);
            float xMid   = xLeft + (xRight - xLeft) * 0.6f; // Profile extends 60% of session width

            double maxV = 0;
            for (int i = 0; i < res; i++)
            {
                double tot = vpGreen[i] + vpRed[i];
                if (tot > maxV) maxV = tot;
            }

            if (double.IsNaN(pH) || double.IsNaN(pL)) return;
            double gap = (pH - pL) / res;
            double buf = gap / 10.0;

            // Session box
            if (showBox)
            {
                float yTop = chartScale.GetYByValue(pH);
                float yBot = chartScale.GetYByValue(pL);
                rt.FillRectangle(new SharpDX.RectangleF(xLeft, yTop, xRight - xLeft, yBot - yTop), boxBgBrush);
                rt.DrawRectangle(new SharpDX.RectangleF(xLeft, yTop, xRight - xLeft, yBot - yTop), boxBrush, boxWidth,
                    new StrokeStyle(rt.Factory, new StrokeStyleProperties { DashStyle = SharpDX.Direct2D1.DashStyle.Dash }));
            }

            // Session label
            if (showLabel)
            {
                float yTop = chartScale.GetYByValue(pH);
                float labelX = (xLeft + xRight) / 2f;
                var layout = new SharpDX.DirectWrite.TextLayout(
                    NinjaTrader.Core.Globals.DirectWriteFactory, sessionType, textFormatLabel, 200, 20);
                rt.DrawTextLayout(new SharpDX.Vector2(labelX - layout.Metrics.Width / 2f, yTop - 16), layout, dxTextBrush);
                layout.Dispose();
            }

            // Volume profile bars
            if (showProf && maxV > 0)
            {
                for (int i = 0; i < res; i++)
                {
                    double zTop = zBounds[i];
                    double zBot = zTop - gap;
                    float yTop = chartScale.GetYByValue(zTop - buf);
                    float yBot = chartScale.GetYByValue(zBot + buf);

                    double gVol = vpGreen[i];
                    double rVol = vpRed[i];

                    float gWidth = (float)((xMid - xLeft) * (gVol / maxV));
                    float rWidth = (float)((xMid - xLeft) * (rVol / maxV));

                    switch (barMode)
                    {
                        case BarModeEnum.Mode2:
                            // Green from left, red continues from green end
                            rt.FillRectangle(new SharpDX.RectangleF(xLeft, yTop, gWidth, yBot - yTop), bullBrush);
                            rt.FillRectangle(new SharpDX.RectangleF(xLeft + gWidth, yTop, rWidth, yBot - yTop), bearBrush);
                            break;
                        case BarModeEnum.Mode1:
                            // Green only
                            rt.FillRectangle(new SharpDX.RectangleF(xLeft, yTop, gWidth, yBot - yTop), bullBrush);
                            break;
                        case BarModeEnum.Mode3:
                            // Green right, red left of anchor
                            rt.FillRectangle(new SharpDX.RectangleF(xLeft, yTop, gWidth, yBot - yTop), bullBrush);
                            rt.FillRectangle(new SharpDX.RectangleF(xLeft - rWidth, yTop, rWidth, yBot - yTop), bearBrush);
                            break;
                    }
                }
            }

            // POC line
            if (showPoc && !double.IsNaN(poc))
            {
                float yPoc = chartScale.GetYByValue(poc);
                rt.DrawLine(new SharpDX.Vector2(xLeft, yPoc), new SharpDX.Vector2(xRight, yPoc), pocBrush, pocWidth);
                if (showLevelLabels)
                {
                    var layout = new SharpDX.DirectWrite.TextLayout(
                        NinjaTrader.Core.Globals.DirectWriteFactory, pocText, textFormatSmall, 100, 14);
                    rt.DrawTextLayout(new SharpDX.Vector2(xRight + 2, yPoc - 7), layout, labelBrush);
                    layout.Dispose();
                }
            }

            // VAH / VAL lines
            if (showVA)
            {
                if (!double.IsNaN(vah))
                {
                    float yVah = chartScale.GetYByValue(vah);
                    rt.DrawLine(new SharpDX.Vector2(xLeft, yVah), new SharpDX.Vector2(xRight, yVah), vahBrush, vahWidth);
                    if (showLevelLabels)
                    {
                        var layout = new SharpDX.DirectWrite.TextLayout(
                            NinjaTrader.Core.Globals.DirectWriteFactory, vahText, textFormatSmall, 100, 14);
                        rt.DrawTextLayout(new SharpDX.Vector2(xRight + 2, yVah - 7), layout, labelBrush);
                        layout.Dispose();
                    }
                }
                if (!double.IsNaN(val))
                {
                    float yVal = chartScale.GetYByValue(val);
                    rt.DrawLine(new SharpDX.Vector2(xLeft, yVal), new SharpDX.Vector2(xRight, yVal), valBrush, valWidth);
                    if (showLevelLabels)
                    {
                        var layout = new SharpDX.DirectWrite.TextLayout(
                            NinjaTrader.Core.Globals.DirectWriteFactory, valText, textFormatSmall, 100, 14);
                        rt.DrawTextLayout(new SharpDX.Vector2(xRight + 2, yVal - 7), layout, labelBrush);
                        layout.Dispose();
                    }
                }
            }

            // Value Area box
            if (showVABox && !double.IsNaN(vah) && !double.IsNaN(val))
            {
                float yVah = chartScale.GetYByValue(vah);
                float yVal = chartScale.GetYByValue(val);
                rt.FillRectangle(new SharpDX.RectangleF(xLeft, yVah, xRight - xLeft, yVal - yVah), vaBrush);
            }
        }

        private void DrawPrevSessionOverlay(RenderTarget rt, ChartControl chartControl, ChartScale chartScale)
        {
            var p = prevOverlay;
            if (p == null || !snapReady) return;

            int res = p.Resolution;
            double pH = p.High;
            double pL = p.Low;
            if (double.IsNaN(pH) || double.IsNaN(pL) || pH == pL) return;

            float xLeft  = chartControl.GetXByBarIndex(chartControl.BarsArray[0], p.LeftBar);
            float xRight = chartControl.GetXByBarIndex(chartControl.BarsArray[0], p.RightBar);
            float xMid   = xLeft + (xRight - xLeft) * 0.7f;

            double gap = (pH - pL) / res;
            double buf = gap / 10.0;

            double maxV = 0;
            for (int i = 0; i < res; i++)
            {
                double tot = p.VpGreen[i] + p.VpRed[i];
                if (tot > maxV) maxV = tot;
            }

            // Profile bars
            if (PrevShowProfile && maxV > 0)
            {
                for (int i = 0; i < res; i++)
                {
                    double zTop = p.ZoneBounds[i];
                    double zBot = zTop - gap;
                    float yTop = chartScale.GetYByValue(zTop - buf);
                    float yBot = chartScale.GetYByValue(zBot + buf);

                    float gWidth = (float)((xMid - xLeft) * (p.VpGreen[i] / maxV));
                    float rWidth = (float)((xMid - xLeft) * (p.VpRed[i] / maxV));

                    switch (PrevBarMode)
                    {
                        case BarModeEnum.Mode2:
                            rt.FillRectangle(new SharpDX.RectangleF(xLeft, yTop, gWidth, yBot - yTop), dxPrevBullBrush);
                            rt.FillRectangle(new SharpDX.RectangleF(xLeft + gWidth, yTop, rWidth, yBot - yTop), dxPrevBearBrush);
                            break;
                        case BarModeEnum.Mode1:
                            rt.FillRectangle(new SharpDX.RectangleF(xLeft, yTop, gWidth, yBot - yTop), dxPrevBullBrush);
                            break;
                        case BarModeEnum.Mode3:
                            rt.FillRectangle(new SharpDX.RectangleF(xLeft, yTop, gWidth, yBot - yTop), dxPrevBullBrush);
                            rt.FillRectangle(new SharpDX.RectangleF(xLeft - rWidth, yTop, rWidth, yBot - yTop), dxPrevBearBrush);
                            break;
                    }
                }
            }

            // Session label
            if (PrevShowSessionLabel)
            {
                float yTop = chartScale.GetYByValue(pH);
                float labelX = (xLeft + xMid) / 2f;
                string txt = "Prev " + p.SessionType;
                var layout = new SharpDX.DirectWrite.TextLayout(
                    NinjaTrader.Core.Globals.DirectWriteFactory, txt, textFormatSmall, 200, 14);
                rt.DrawTextLayout(new SharpDX.Vector2(labelX - layout.Metrics.Width / 2f, yTop - 14), layout, dxTextBrush);
                layout.Dispose();
            }

            // POC line (dashed, extends to current bar)
            if (PrevShowPoc && !double.IsNaN(p.Poc))
            {
                float yPoc = chartScale.GetYByValue(p.Poc);
                float xEnd = chartControl.GetXByBarIndex(chartControl.BarsArray[0], CurrentBar);
                using (var dashStyle = new StrokeStyle(rt.Factory, new StrokeStyleProperties { DashStyle = SharpDX.Direct2D1.DashStyle.Dash }))
                {
                    rt.DrawLine(new SharpDX.Vector2(xLeft, yPoc), new SharpDX.Vector2(xEnd, yPoc),
                        dxPrevPocBrush, PrevPocWidth, dashStyle);
                }
                if (PrevShowLevelLabels)
                {
                    var layout = new SharpDX.DirectWrite.TextLayout(
                        NinjaTrader.Core.Globals.DirectWriteFactory, "P: " + PrevPocLabel, textFormatSmall, 100, 14);
                    rt.DrawTextLayout(new SharpDX.Vector2(xEnd + 2, yPoc - 7), layout, dxPrevLabelBrush);
                    layout.Dispose();
                }
            }

            // VAH / VAL lines (dashed)
            if (PrevShowVA)
            {
                float xEnd = chartControl.GetXByBarIndex(chartControl.BarsArray[0], CurrentBar);
                using (var dashStyle = new StrokeStyle(rt.Factory, new StrokeStyleProperties { DashStyle = SharpDX.Direct2D1.DashStyle.Dash }))
                {
                    if (!double.IsNaN(p.Vah))
                    {
                        float yVah = chartScale.GetYByValue(p.Vah);
                        rt.DrawLine(new SharpDX.Vector2(xLeft, yVah), new SharpDX.Vector2(xEnd, yVah),
                            dxPrevVahBrush, PrevVahWidth, dashStyle);
                        if (PrevShowLevelLabels)
                        {
                            var layout = new SharpDX.DirectWrite.TextLayout(
                                NinjaTrader.Core.Globals.DirectWriteFactory, "P: " + PrevVahLabel, textFormatSmall, 100, 14);
                            rt.DrawTextLayout(new SharpDX.Vector2(xEnd + 2, yVah - 7), layout, dxPrevLabelBrush);
                            layout.Dispose();
                        }
                    }
                    if (!double.IsNaN(p.Val))
                    {
                        float yVal = chartScale.GetYByValue(p.Val);
                        rt.DrawLine(new SharpDX.Vector2(xLeft, yVal), new SharpDX.Vector2(xEnd, yVal),
                            dxPrevValBrush, PrevValWidth, dashStyle);
                        if (PrevShowLevelLabels)
                        {
                            var layout = new SharpDX.DirectWrite.TextLayout(
                                NinjaTrader.Core.Globals.DirectWriteFactory, "P: " + PrevValLabel, textFormatSmall, 100, 14);
                            rt.DrawTextLayout(new SharpDX.Vector2(xEnd + 2, yVal - 7), layout, dxPrevLabelBrush);
                            layout.Dispose();
                        }
                    }
                }
            }

            // VA Box
            if (PrevShowVABox && !double.IsNaN(p.Vah) && !double.IsNaN(p.Val))
            {
                float xEnd = chartControl.GetXByBarIndex(chartControl.BarsArray[0], CurrentBar);
                float yVah = chartScale.GetYByValue(p.Vah);
                float yVal = chartScale.GetYByValue(p.Val);
                rt.FillRectangle(new SharpDX.RectangleF(xLeft, yVah, xEnd - xLeft, yVal - yVah), dxPrevVaBrush);
            }
        }

        private void DrawForexBox(RenderTarget rt, ChartControl chartControl, ChartScale chartScale, ForexBoxData fb)
        {
            float xLeft  = chartControl.GetXByBarIndex(chartControl.BarsArray[0], fb.LeftBar);
            float xRight = chartControl.GetXByBarIndex(chartControl.BarsArray[0], fb.RightBar);
            float yTop   = chartScale.GetYByValue(fb.High);
            float yBot   = chartScale.GetYByValue(fb.Low);

            rt.FillRectangle(new SharpDX.RectangleF(xLeft, yTop, xRight - xLeft, yBot - yTop), dxForexBoxBrush);
            using (var dashStyle = new StrokeStyle(rt.Factory, new StrokeStyleProperties { DashStyle = SharpDX.Direct2D1.DashStyle.Dash }))
            {
                rt.DrawRectangle(new SharpDX.RectangleF(xLeft, yTop, xRight - xLeft, yBot - yTop),
                    dxCurBoxBrush, CurBoxWidth, dashStyle);
            }

            var layout = new SharpDX.DirectWrite.TextLayout(
                NinjaTrader.Core.Globals.DirectWriteFactory, fb.Label, textFormatLabel, 200, 20);
            float labelX = (xLeft + xRight) / 2f - layout.Metrics.Width / 2f;
            rt.DrawTextLayout(new SharpDX.Vector2(labelX, yTop - 16), layout, dxTextBrush);
            layout.Dispose();
        }

        private void DrawOpenLevel(RenderTarget rt, ChartControl chartControl, ChartScale chartScale,
            int startBar, double price, int width, SharpDX.Direct2D1.Brush brush, LineStyleEnum style, string label)
        {
            if (double.IsNaN(price)) return;

            float xLeft = chartControl.GetXByBarIndex(chartControl.BarsArray[0], startBar);
            float xRight = chartControl.GetXByBarIndex(chartControl.BarsArray[0], CurrentBar);
            float y = chartScale.GetYByValue(price);

            SharpDX.Direct2D1.DashStyle ds = style == LineStyleEnum.Dashed ? SharpDX.Direct2D1.DashStyle.Dash
                         : style == LineStyleEnum.Dotted ? SharpDX.Direct2D1.DashStyle.Dot
                         : SharpDX.Direct2D1.DashStyle.Solid;

            using (var strokeStyle = new StrokeStyle(rt.Factory, new StrokeStyleProperties { DashStyle = ds }))
            {
                rt.DrawLine(new SharpDX.Vector2(xLeft, y), new SharpDX.Vector2(xRight, y), brush, width, strokeStyle);
            }

            if (label != null)
            {
                var layout = new SharpDX.DirectWrite.TextLayout(
                    NinjaTrader.Core.Globals.DirectWriteFactory, label, textFormatSmall, 200, 14);
                rt.DrawTextLayout(new SharpDX.Vector2(xRight + 4, y - 7), layout, brush);
                layout.Dispose();
            }
        }

        #endregion

        #region Properties — Current Session

        [NinjaScriptProperty]
        [Display(Name = "Session Type", Order = 1, GroupName = "1. Current Session")]
        public SessionTypeEnum CurSessionType { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Show Volume Profile", Order = 2, GroupName = "1. Current Session")]
        public bool CurShowProfile { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Show POC", Order = 3, GroupName = "1. Current Session")]
        public bool CurShowPoc { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Show VAH and VAL", Order = 4, GroupName = "1. Current Session")]
        public bool CurShowVA { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Show Value Area Box", Order = 5, GroupName = "1. Current Session")]
        public bool CurShowVABox { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Show Live Zone", Order = 6, GroupName = "1. Current Session")]
        public bool CurShowLiveZone { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Show Session Box", Order = 7, GroupName = "1. Current Session")]
        public bool CurShowSessionBox { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Show Session Label", Order = 8, GroupName = "1. Current Session")]
        public bool CurShowSessionLabel { get; set; }

        [NinjaScriptProperty]
        [Range(5, 100)]
        [Display(Name = "Resolution", Order = 9, GroupName = "1. Current Session")]
        public int CurResolution { get; set; }

        [NinjaScriptProperty]
        [Range(1, 100)]
        [Display(Name = "Value Area %", Order = 10, GroupName = "1. Current Session")]
        public int CurValueAreaPct { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Bar Mode", Order = 11, GroupName = "1. Current Session")]
        public BarModeEnum CurBarMode { get; set; }

        #endregion

        #region Properties — Current Session Appearance

        [XmlIgnore]
        [Display(Name = "Up Volume Color", Order = 1, GroupName = "2. Current Appearance")]
        public System.Windows.Media.Brush CurBullColor { get; set; }
        [Browsable(false)]
        public string CurBullColorSerialize { get { return Serialize.BrushToString(CurBullColor); } set { CurBullColor = Serialize.StringToBrush(value); } }

        [Range(0, 100)]
        [Display(Name = "Up Volume Opacity %", Order = 2, GroupName = "2. Current Appearance")]
        public int CurBullOpacity { get; set; }

        [XmlIgnore]
        [Display(Name = "Down Volume Color", Order = 3, GroupName = "2. Current Appearance")]
        public System.Windows.Media.Brush CurBearColor { get; set; }
        [Browsable(false)]
        public string CurBearColorSerialize { get { return Serialize.BrushToString(CurBearColor); } set { CurBearColor = Serialize.StringToBrush(value); } }

        [Range(0, 100)]
        [Display(Name = "Down Volume Opacity %", Order = 4, GroupName = "2. Current Appearance")]
        public int CurBearOpacity { get; set; }

        [XmlIgnore]
        [Display(Name = "VA Box Color", Order = 5, GroupName = "2. Current Appearance")]
        public System.Windows.Media.Brush CurVABoxColor { get; set; }
        [Browsable(false)]
        public string CurVABoxColorSerialize { get { return Serialize.BrushToString(CurVABoxColor); } set { CurVABoxColor = Serialize.StringToBrush(value); } }

        [Range(0, 100)]
        [Display(Name = "VA Box Opacity %", Order = 6, GroupName = "2. Current Appearance")]
        public int CurVABoxOpacity { get; set; }

        [XmlIgnore]
        [Display(Name = "POC Color", Order = 7, GroupName = "2. Current Appearance")]
        public System.Windows.Media.Brush CurPocColor { get; set; }
        [Browsable(false)]
        public string CurPocColorSerialize { get { return Serialize.BrushToString(CurPocColor); } set { CurPocColor = Serialize.StringToBrush(value); } }

        [Range(1, 5)]
        [Display(Name = "POC Width", Order = 8, GroupName = "2. Current Appearance")]
        public int CurPocWidth { get; set; }

        [XmlIgnore]
        [Display(Name = "VAH Color", Order = 9, GroupName = "2. Current Appearance")]
        public System.Windows.Media.Brush CurVahColor { get; set; }
        [Browsable(false)]
        public string CurVahColorSerialize { get { return Serialize.BrushToString(CurVahColor); } set { CurVahColor = Serialize.StringToBrush(value); } }

        [Range(1, 5)]
        [Display(Name = "VAH Width", Order = 10, GroupName = "2. Current Appearance")]
        public int CurVahWidth { get; set; }

        [XmlIgnore]
        [Display(Name = "VAL Color", Order = 11, GroupName = "2. Current Appearance")]
        public System.Windows.Media.Brush CurValColor { get; set; }
        [Browsable(false)]
        public string CurValColorSerialize { get { return Serialize.BrushToString(CurValColor); } set { CurValColor = Serialize.StringToBrush(value); } }

        [Range(1, 5)]
        [Display(Name = "VAL Width", Order = 12, GroupName = "2. Current Appearance")]
        public int CurValWidth { get; set; }

        [XmlIgnore]
        [Display(Name = "Session Box Color", Order = 13, GroupName = "2. Current Appearance")]
        public System.Windows.Media.Brush CurBoxColor { get; set; }
        [Browsable(false)]
        public string CurBoxColorSerialize { get { return Serialize.BrushToString(CurBoxColor); } set { CurBoxColor = Serialize.StringToBrush(value); } }

        [Range(1, 5)]
        [Display(Name = "Session Box Width", Order = 14, GroupName = "2. Current Appearance")]
        public int CurBoxWidth { get; set; }

        [Range(0, 100)]
        [Display(Name = "Session Box BG Opacity %", Order = 15, GroupName = "2. Current Appearance")]
        public int CurBoxBgOpacity { get; set; }

        #endregion

        #region Properties — Current Level Labels

        [NinjaScriptProperty]
        [Display(Name = "Show Level Labels", Order = 1, GroupName = "3. Current Level Labels")]
        public bool CurShowLevelLabels { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "POC Label", Order = 2, GroupName = "3. Current Level Labels")]
        public string CurPocLabel { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "VAH Label", Order = 3, GroupName = "3. Current Level Labels")]
        public string CurVahLabel { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "VAL Label", Order = 4, GroupName = "3. Current Level Labels")]
        public string CurValLabel { get; set; }

        [XmlIgnore]
        [Display(Name = "Label Color", Order = 5, GroupName = "3. Current Level Labels")]
        public System.Windows.Media.Brush CurLabelColor { get; set; }
        [Browsable(false)]
        public string CurLabelColorSerialize { get { return Serialize.BrushToString(CurLabelColor); } set { CurLabelColor = Serialize.StringToBrush(value); } }

        #endregion

        #region Properties — Previous Session

        [NinjaScriptProperty]
        [Display(Name = "Session Type", Order = 1, GroupName = "4. Previous Session")]
        public SessionTypeEnum PrevSessionType { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Show Volume Profile", Order = 2, GroupName = "4. Previous Session")]
        public bool PrevShowProfile { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Show POC", Order = 3, GroupName = "4. Previous Session")]
        public bool PrevShowPoc { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Show VAH and VAL", Order = 4, GroupName = "4. Previous Session")]
        public bool PrevShowVA { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Show Value Area Box", Order = 5, GroupName = "4. Previous Session")]
        public bool PrevShowVABox { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Show Overlay", Order = 6, GroupName = "4. Previous Session")]
        public bool PrevShowOverlay { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Show Session Label", Order = 7, GroupName = "4. Previous Session")]
        public bool PrevShowSessionLabel { get; set; }

        [NinjaScriptProperty]
        [Range(5, 100)]
        [Display(Name = "Resolution", Order = 8, GroupName = "4. Previous Session")]
        public int PrevResolution { get; set; }

        [NinjaScriptProperty]
        [Range(1, 100)]
        [Display(Name = "Value Area %", Order = 9, GroupName = "4. Previous Session")]
        public int PrevValueAreaPct { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Bar Mode", Order = 10, GroupName = "4. Previous Session")]
        public BarModeEnum PrevBarMode { get; set; }

        #endregion

        #region Properties — Previous Session Appearance

        [XmlIgnore]
        [Display(Name = "Up Volume Color", Order = 1, GroupName = "5. Previous Appearance")]
        public System.Windows.Media.Brush PrevBullColor { get; set; }
        [Browsable(false)]
        public string PrevBullColorSerialize { get { return Serialize.BrushToString(PrevBullColor); } set { PrevBullColor = Serialize.StringToBrush(value); } }

        [Range(0, 100)]
        [Display(Name = "Up Volume Opacity %", Order = 2, GroupName = "5. Previous Appearance")]
        public int PrevBullOpacity { get; set; }

        [XmlIgnore]
        [Display(Name = "Down Volume Color", Order = 3, GroupName = "5. Previous Appearance")]
        public System.Windows.Media.Brush PrevBearColor { get; set; }
        [Browsable(false)]
        public string PrevBearColorSerialize { get { return Serialize.BrushToString(PrevBearColor); } set { PrevBearColor = Serialize.StringToBrush(value); } }

        [Range(0, 100)]
        [Display(Name = "Down Volume Opacity %", Order = 4, GroupName = "5. Previous Appearance")]
        public int PrevBearOpacity { get; set; }

        [XmlIgnore]
        [Display(Name = "VA Box Color", Order = 5, GroupName = "5. Previous Appearance")]
        public System.Windows.Media.Brush PrevVABoxColor { get; set; }
        [Browsable(false)]
        public string PrevVABoxColorSerialize { get { return Serialize.BrushToString(PrevVABoxColor); } set { PrevVABoxColor = Serialize.StringToBrush(value); } }

        [Range(0, 100)]
        [Display(Name = "VA Box Opacity %", Order = 6, GroupName = "5. Previous Appearance")]
        public int PrevVABoxOpacity { get; set; }

        [XmlIgnore]
        [Display(Name = "POC Color", Order = 7, GroupName = "5. Previous Appearance")]
        public System.Windows.Media.Brush PrevPocColor { get; set; }
        [Browsable(false)]
        public string PrevPocColorSerialize { get { return Serialize.BrushToString(PrevPocColor); } set { PrevPocColor = Serialize.StringToBrush(value); } }

        [Range(1, 5)]
        [Display(Name = "POC Width", Order = 8, GroupName = "5. Previous Appearance")]
        public int PrevPocWidth { get; set; }

        [Range(0, 100)]
        [Display(Name = "POC Opacity %", Order = 9, GroupName = "5. Previous Appearance")]
        public int PrevPocOpacity { get; set; }

        [XmlIgnore]
        [Display(Name = "VAH Color", Order = 10, GroupName = "5. Previous Appearance")]
        public System.Windows.Media.Brush PrevVahColor { get; set; }
        [Browsable(false)]
        public string PrevVahColorSerialize { get { return Serialize.BrushToString(PrevVahColor); } set { PrevVahColor = Serialize.StringToBrush(value); } }

        [Range(1, 5)]
        [Display(Name = "VAH Width", Order = 11, GroupName = "5. Previous Appearance")]
        public int PrevVahWidth { get; set; }

        [Range(0, 100)]
        [Display(Name = "VAH Opacity %", Order = 12, GroupName = "5. Previous Appearance")]
        public int PrevVahOpacity { get; set; }

        [XmlIgnore]
        [Display(Name = "VAL Color", Order = 13, GroupName = "5. Previous Appearance")]
        public System.Windows.Media.Brush PrevValColor { get; set; }
        [Browsable(false)]
        public string PrevValColorSerialize { get { return Serialize.BrushToString(PrevValColor); } set { PrevValColor = Serialize.StringToBrush(value); } }

        [Range(1, 5)]
        [Display(Name = "VAL Width", Order = 14, GroupName = "5. Previous Appearance")]
        public int PrevValWidth { get; set; }

        [Range(0, 100)]
        [Display(Name = "VAL Opacity %", Order = 15, GroupName = "5. Previous Appearance")]
        public int PrevValOpacity { get; set; }

        #endregion

        #region Properties — Previous Level Labels

        [NinjaScriptProperty]
        [Display(Name = "Show Level Labels", Order = 1, GroupName = "6. Previous Level Labels")]
        public bool PrevShowLevelLabels { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "POC Label", Order = 2, GroupName = "6. Previous Level Labels")]
        public string PrevPocLabel { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "VAH Label", Order = 3, GroupName = "6. Previous Level Labels")]
        public string PrevVahLabel { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "VAL Label", Order = 4, GroupName = "6. Previous Level Labels")]
        public string PrevValLabel { get; set; }

        [XmlIgnore]
        [Display(Name = "Label Color", Order = 5, GroupName = "6. Previous Level Labels")]
        public System.Windows.Media.Brush PrevLabelColor { get; set; }
        [Browsable(false)]
        public string PrevLabelColorSerialize { get { return Serialize.BrushToString(PrevLabelColor); } set { PrevLabelColor = Serialize.StringToBrush(value); } }

        #endregion

        #region Properties — Forex Sessions

        [NinjaScriptProperty]
        [Display(Name = "Show Forex Session Boxes", Order = 1, GroupName = "7. Forex Sessions")]
        public bool ShowForexBoxes { get; set; }

        #endregion

        #region Properties — Open Levels

        [NinjaScriptProperty]
        [Display(Name = "Show 6 PM Daily Open", Order = 1, GroupName = "8. Open Levels")]
        public bool Show6pmOpen { get; set; }

        [XmlIgnore]
        [Display(Name = "6 PM Color", Order = 2, GroupName = "8. Open Levels")]
        public System.Windows.Media.Brush Pm6Color { get; set; }
        [Browsable(false)]
        public string Pm6ColorSerialize { get { return Serialize.BrushToString(Pm6Color); } set { Pm6Color = Serialize.StringToBrush(value); } }

        [Range(1, 4)]
        [Display(Name = "6 PM Width", Order = 3, GroupName = "8. Open Levels")]
        public int Pm6Width { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "6 PM Style", Order = 4, GroupName = "8. Open Levels")]
        public LineStyleEnum Pm6Style { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Show 6 PM Label", Order = 5, GroupName = "8. Open Levels")]
        public bool Show6pmLabel { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Show Weekly Open", Order = 6, GroupName = "8. Open Levels")]
        public bool ShowWeeklyOpen { get; set; }

        [XmlIgnore]
        [Display(Name = "Weekly Open Color", Order = 7, GroupName = "8. Open Levels")]
        public System.Windows.Media.Brush WeeklyOpenColor { get; set; }
        [Browsable(false)]
        public string WeeklyOpenColorSerialize { get { return Serialize.BrushToString(WeeklyOpenColor); } set { WeeklyOpenColor = Serialize.StringToBrush(value); } }

        [Range(1, 4)]
        [Display(Name = "Weekly Open Width", Order = 8, GroupName = "8. Open Levels")]
        public int WeeklyOpenWidth { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Weekly Open Style", Order = 9, GroupName = "8. Open Levels")]
        public LineStyleEnum WeeklyOpenStyle { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Show Weekly Label", Order = 10, GroupName = "8. Open Levels")]
        public bool ShowWeeklyLabel { get; set; }

        #endregion
    }
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private SessionsVPwithPrevSessionVPAndOpens[] cacheSessionsVPwithPrevSessionVPAndOpens;
		public SessionsVPwithPrevSessionVPAndOpens SessionsVPwithPrevSessionVPAndOpens(SessionTypeEnum curSessionType, bool curShowProfile, bool curShowPoc, bool curShowVA, bool curShowVABox, bool curShowLiveZone, bool curShowSessionBox, bool curShowSessionLabel, int curResolution, int curValueAreaPct, BarModeEnum curBarMode, bool curShowLevelLabels, string curPocLabel, string curVahLabel, string curValLabel, SessionTypeEnum prevSessionType, bool prevShowProfile, bool prevShowPoc, bool prevShowVA, bool prevShowVABox, bool prevShowOverlay, bool prevShowSessionLabel, int prevResolution, int prevValueAreaPct, BarModeEnum prevBarMode, bool prevShowLevelLabels, string prevPocLabel, string prevVahLabel, string prevValLabel, bool showForexBoxes, bool show6pmOpen, LineStyleEnum pm6Style, bool show6pmLabel, bool showWeeklyOpen, LineStyleEnum weeklyOpenStyle, bool showWeeklyLabel)
		{
			return SessionsVPwithPrevSessionVPAndOpens(Input, curSessionType, curShowProfile, curShowPoc, curShowVA, curShowVABox, curShowLiveZone, curShowSessionBox, curShowSessionLabel, curResolution, curValueAreaPct, curBarMode, curShowLevelLabels, curPocLabel, curVahLabel, curValLabel, prevSessionType, prevShowProfile, prevShowPoc, prevShowVA, prevShowVABox, prevShowOverlay, prevShowSessionLabel, prevResolution, prevValueAreaPct, prevBarMode, prevShowLevelLabels, prevPocLabel, prevVahLabel, prevValLabel, showForexBoxes, show6pmOpen, pm6Style, show6pmLabel, showWeeklyOpen, weeklyOpenStyle, showWeeklyLabel);
		}

		public SessionsVPwithPrevSessionVPAndOpens SessionsVPwithPrevSessionVPAndOpens(ISeries<double> input, SessionTypeEnum curSessionType, bool curShowProfile, bool curShowPoc, bool curShowVA, bool curShowVABox, bool curShowLiveZone, bool curShowSessionBox, bool curShowSessionLabel, int curResolution, int curValueAreaPct, BarModeEnum curBarMode, bool curShowLevelLabels, string curPocLabel, string curVahLabel, string curValLabel, SessionTypeEnum prevSessionType, bool prevShowProfile, bool prevShowPoc, bool prevShowVA, bool prevShowVABox, bool prevShowOverlay, bool prevShowSessionLabel, int prevResolution, int prevValueAreaPct, BarModeEnum prevBarMode, bool prevShowLevelLabels, string prevPocLabel, string prevVahLabel, string prevValLabel, bool showForexBoxes, bool show6pmOpen, LineStyleEnum pm6Style, bool show6pmLabel, bool showWeeklyOpen, LineStyleEnum weeklyOpenStyle, bool showWeeklyLabel)
		{
			if (cacheSessionsVPwithPrevSessionVPAndOpens != null)
				for (int idx = 0; idx < cacheSessionsVPwithPrevSessionVPAndOpens.Length; idx++)
					if (cacheSessionsVPwithPrevSessionVPAndOpens[idx] != null && cacheSessionsVPwithPrevSessionVPAndOpens[idx].CurSessionType == curSessionType && cacheSessionsVPwithPrevSessionVPAndOpens[idx].CurShowProfile == curShowProfile && cacheSessionsVPwithPrevSessionVPAndOpens[idx].CurShowPoc == curShowPoc && cacheSessionsVPwithPrevSessionVPAndOpens[idx].CurShowVA == curShowVA && cacheSessionsVPwithPrevSessionVPAndOpens[idx].CurShowVABox == curShowVABox && cacheSessionsVPwithPrevSessionVPAndOpens[idx].CurShowLiveZone == curShowLiveZone && cacheSessionsVPwithPrevSessionVPAndOpens[idx].CurShowSessionBox == curShowSessionBox && cacheSessionsVPwithPrevSessionVPAndOpens[idx].CurShowSessionLabel == curShowSessionLabel && cacheSessionsVPwithPrevSessionVPAndOpens[idx].CurResolution == curResolution && cacheSessionsVPwithPrevSessionVPAndOpens[idx].CurValueAreaPct == curValueAreaPct && cacheSessionsVPwithPrevSessionVPAndOpens[idx].CurBarMode == curBarMode && cacheSessionsVPwithPrevSessionVPAndOpens[idx].CurShowLevelLabels == curShowLevelLabels && cacheSessionsVPwithPrevSessionVPAndOpens[idx].CurPocLabel == curPocLabel && cacheSessionsVPwithPrevSessionVPAndOpens[idx].CurVahLabel == curVahLabel && cacheSessionsVPwithPrevSessionVPAndOpens[idx].CurValLabel == curValLabel && cacheSessionsVPwithPrevSessionVPAndOpens[idx].PrevSessionType == prevSessionType && cacheSessionsVPwithPrevSessionVPAndOpens[idx].PrevShowProfile == prevShowProfile && cacheSessionsVPwithPrevSessionVPAndOpens[idx].PrevShowPoc == prevShowPoc && cacheSessionsVPwithPrevSessionVPAndOpens[idx].PrevShowVA == prevShowVA && cacheSessionsVPwithPrevSessionVPAndOpens[idx].PrevShowVABox == prevShowVABox && cacheSessionsVPwithPrevSessionVPAndOpens[idx].PrevShowOverlay == prevShowOverlay && cacheSessionsVPwithPrevSessionVPAndOpens[idx].PrevShowSessionLabel == prevShowSessionLabel && cacheSessionsVPwithPrevSessionVPAndOpens[idx].PrevResolution == prevResolution && cacheSessionsVPwithPrevSessionVPAndOpens[idx].PrevValueAreaPct == prevValueAreaPct && cacheSessionsVPwithPrevSessionVPAndOpens[idx].PrevBarMode == prevBarMode && cacheSessionsVPwithPrevSessionVPAndOpens[idx].PrevShowLevelLabels == prevShowLevelLabels && cacheSessionsVPwithPrevSessionVPAndOpens[idx].PrevPocLabel == prevPocLabel && cacheSessionsVPwithPrevSessionVPAndOpens[idx].PrevVahLabel == prevVahLabel && cacheSessionsVPwithPrevSessionVPAndOpens[idx].PrevValLabel == prevValLabel && cacheSessionsVPwithPrevSessionVPAndOpens[idx].ShowForexBoxes == showForexBoxes && cacheSessionsVPwithPrevSessionVPAndOpens[idx].Show6pmOpen == show6pmOpen && cacheSessionsVPwithPrevSessionVPAndOpens[idx].Pm6Style == pm6Style && cacheSessionsVPwithPrevSessionVPAndOpens[idx].Show6pmLabel == show6pmLabel && cacheSessionsVPwithPrevSessionVPAndOpens[idx].ShowWeeklyOpen == showWeeklyOpen && cacheSessionsVPwithPrevSessionVPAndOpens[idx].WeeklyOpenStyle == weeklyOpenStyle && cacheSessionsVPwithPrevSessionVPAndOpens[idx].ShowWeeklyLabel == showWeeklyLabel && cacheSessionsVPwithPrevSessionVPAndOpens[idx].EqualsInput(input))
						return cacheSessionsVPwithPrevSessionVPAndOpens[idx];
			return CacheIndicator<SessionsVPwithPrevSessionVPAndOpens>(new SessionsVPwithPrevSessionVPAndOpens(){ CurSessionType = curSessionType, CurShowProfile = curShowProfile, CurShowPoc = curShowPoc, CurShowVA = curShowVA, CurShowVABox = curShowVABox, CurShowLiveZone = curShowLiveZone, CurShowSessionBox = curShowSessionBox, CurShowSessionLabel = curShowSessionLabel, CurResolution = curResolution, CurValueAreaPct = curValueAreaPct, CurBarMode = curBarMode, CurShowLevelLabels = curShowLevelLabels, CurPocLabel = curPocLabel, CurVahLabel = curVahLabel, CurValLabel = curValLabel, PrevSessionType = prevSessionType, PrevShowProfile = prevShowProfile, PrevShowPoc = prevShowPoc, PrevShowVA = prevShowVA, PrevShowVABox = prevShowVABox, PrevShowOverlay = prevShowOverlay, PrevShowSessionLabel = prevShowSessionLabel, PrevResolution = prevResolution, PrevValueAreaPct = prevValueAreaPct, PrevBarMode = prevBarMode, PrevShowLevelLabels = prevShowLevelLabels, PrevPocLabel = prevPocLabel, PrevVahLabel = prevVahLabel, PrevValLabel = prevValLabel, ShowForexBoxes = showForexBoxes, Show6pmOpen = show6pmOpen, Pm6Style = pm6Style, Show6pmLabel = show6pmLabel, ShowWeeklyOpen = showWeeklyOpen, WeeklyOpenStyle = weeklyOpenStyle, ShowWeeklyLabel = showWeeklyLabel }, input, ref cacheSessionsVPwithPrevSessionVPAndOpens);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.SessionsVPwithPrevSessionVPAndOpens SessionsVPwithPrevSessionVPAndOpens(SessionTypeEnum curSessionType, bool curShowProfile, bool curShowPoc, bool curShowVA, bool curShowVABox, bool curShowLiveZone, bool curShowSessionBox, bool curShowSessionLabel, int curResolution, int curValueAreaPct, BarModeEnum curBarMode, bool curShowLevelLabels, string curPocLabel, string curVahLabel, string curValLabel, SessionTypeEnum prevSessionType, bool prevShowProfile, bool prevShowPoc, bool prevShowVA, bool prevShowVABox, bool prevShowOverlay, bool prevShowSessionLabel, int prevResolution, int prevValueAreaPct, BarModeEnum prevBarMode, bool prevShowLevelLabels, string prevPocLabel, string prevVahLabel, string prevValLabel, bool showForexBoxes, bool show6pmOpen, LineStyleEnum pm6Style, bool show6pmLabel, bool showWeeklyOpen, LineStyleEnum weeklyOpenStyle, bool showWeeklyLabel)
		{
			return indicator.SessionsVPwithPrevSessionVPAndOpens(Input, curSessionType, curShowProfile, curShowPoc, curShowVA, curShowVABox, curShowLiveZone, curShowSessionBox, curShowSessionLabel, curResolution, curValueAreaPct, curBarMode, curShowLevelLabels, curPocLabel, curVahLabel, curValLabel, prevSessionType, prevShowProfile, prevShowPoc, prevShowVA, prevShowVABox, prevShowOverlay, prevShowSessionLabel, prevResolution, prevValueAreaPct, prevBarMode, prevShowLevelLabels, prevPocLabel, prevVahLabel, prevValLabel, showForexBoxes, show6pmOpen, pm6Style, show6pmLabel, showWeeklyOpen, weeklyOpenStyle, showWeeklyLabel);
		}

		public Indicators.SessionsVPwithPrevSessionVPAndOpens SessionsVPwithPrevSessionVPAndOpens(ISeries<double> input , SessionTypeEnum curSessionType, bool curShowProfile, bool curShowPoc, bool curShowVA, bool curShowVABox, bool curShowLiveZone, bool curShowSessionBox, bool curShowSessionLabel, int curResolution, int curValueAreaPct, BarModeEnum curBarMode, bool curShowLevelLabels, string curPocLabel, string curVahLabel, string curValLabel, SessionTypeEnum prevSessionType, bool prevShowProfile, bool prevShowPoc, bool prevShowVA, bool prevShowVABox, bool prevShowOverlay, bool prevShowSessionLabel, int prevResolution, int prevValueAreaPct, BarModeEnum prevBarMode, bool prevShowLevelLabels, string prevPocLabel, string prevVahLabel, string prevValLabel, bool showForexBoxes, bool show6pmOpen, LineStyleEnum pm6Style, bool show6pmLabel, bool showWeeklyOpen, LineStyleEnum weeklyOpenStyle, bool showWeeklyLabel)
		{
			return indicator.SessionsVPwithPrevSessionVPAndOpens(input, curSessionType, curShowProfile, curShowPoc, curShowVA, curShowVABox, curShowLiveZone, curShowSessionBox, curShowSessionLabel, curResolution, curValueAreaPct, curBarMode, curShowLevelLabels, curPocLabel, curVahLabel, curValLabel, prevSessionType, prevShowProfile, prevShowPoc, prevShowVA, prevShowVABox, prevShowOverlay, prevShowSessionLabel, prevResolution, prevValueAreaPct, prevBarMode, prevShowLevelLabels, prevPocLabel, prevVahLabel, prevValLabel, showForexBoxes, show6pmOpen, pm6Style, show6pmLabel, showWeeklyOpen, weeklyOpenStyle, showWeeklyLabel);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.SessionsVPwithPrevSessionVPAndOpens SessionsVPwithPrevSessionVPAndOpens(SessionTypeEnum curSessionType, bool curShowProfile, bool curShowPoc, bool curShowVA, bool curShowVABox, bool curShowLiveZone, bool curShowSessionBox, bool curShowSessionLabel, int curResolution, int curValueAreaPct, BarModeEnum curBarMode, bool curShowLevelLabels, string curPocLabel, string curVahLabel, string curValLabel, SessionTypeEnum prevSessionType, bool prevShowProfile, bool prevShowPoc, bool prevShowVA, bool prevShowVABox, bool prevShowOverlay, bool prevShowSessionLabel, int prevResolution, int prevValueAreaPct, BarModeEnum prevBarMode, bool prevShowLevelLabels, string prevPocLabel, string prevVahLabel, string prevValLabel, bool showForexBoxes, bool show6pmOpen, LineStyleEnum pm6Style, bool show6pmLabel, bool showWeeklyOpen, LineStyleEnum weeklyOpenStyle, bool showWeeklyLabel)
		{
			return indicator.SessionsVPwithPrevSessionVPAndOpens(Input, curSessionType, curShowProfile, curShowPoc, curShowVA, curShowVABox, curShowLiveZone, curShowSessionBox, curShowSessionLabel, curResolution, curValueAreaPct, curBarMode, curShowLevelLabels, curPocLabel, curVahLabel, curValLabel, prevSessionType, prevShowProfile, prevShowPoc, prevShowVA, prevShowVABox, prevShowOverlay, prevShowSessionLabel, prevResolution, prevValueAreaPct, prevBarMode, prevShowLevelLabels, prevPocLabel, prevVahLabel, prevValLabel, showForexBoxes, show6pmOpen, pm6Style, show6pmLabel, showWeeklyOpen, weeklyOpenStyle, showWeeklyLabel);
		}

		public Indicators.SessionsVPwithPrevSessionVPAndOpens SessionsVPwithPrevSessionVPAndOpens(ISeries<double> input , SessionTypeEnum curSessionType, bool curShowProfile, bool curShowPoc, bool curShowVA, bool curShowVABox, bool curShowLiveZone, bool curShowSessionBox, bool curShowSessionLabel, int curResolution, int curValueAreaPct, BarModeEnum curBarMode, bool curShowLevelLabels, string curPocLabel, string curVahLabel, string curValLabel, SessionTypeEnum prevSessionType, bool prevShowProfile, bool prevShowPoc, bool prevShowVA, bool prevShowVABox, bool prevShowOverlay, bool prevShowSessionLabel, int prevResolution, int prevValueAreaPct, BarModeEnum prevBarMode, bool prevShowLevelLabels, string prevPocLabel, string prevVahLabel, string prevValLabel, bool showForexBoxes, bool show6pmOpen, LineStyleEnum pm6Style, bool show6pmLabel, bool showWeeklyOpen, LineStyleEnum weeklyOpenStyle, bool showWeeklyLabel)
		{
			return indicator.SessionsVPwithPrevSessionVPAndOpens(input, curSessionType, curShowProfile, curShowPoc, curShowVA, curShowVABox, curShowLiveZone, curShowSessionBox, curShowSessionLabel, curResolution, curValueAreaPct, curBarMode, curShowLevelLabels, curPocLabel, curVahLabel, curValLabel, prevSessionType, prevShowProfile, prevShowPoc, prevShowVA, prevShowVABox, prevShowOverlay, prevShowSessionLabel, prevResolution, prevValueAreaPct, prevBarMode, prevShowLevelLabels, prevPocLabel, prevVahLabel, prevValLabel, showForexBoxes, show6pmOpen, pm6Style, show6pmLabel, showWeeklyOpen, weeklyOpenStyle, showWeeklyLabel);
		}
	}
}

#endregion
