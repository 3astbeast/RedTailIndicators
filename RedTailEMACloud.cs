#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.DrawingTools;
using SharpDX;
using SharpDX.Direct2D1;
#endregion

namespace NinjaTrader.NinjaScript
{
	public enum MATypeOption
	{
		EMA,
		SMA
	}
}

namespace NinjaTrader.NinjaScript.Indicators
{
	public class RedTailEMACloud : Indicator
	{
		private EMA ema1Short, ema1Long;
		private EMA ema2Short, ema2Long;
		private EMA ema3Short, ema3Long;
		private EMA ema4Short, ema4Long;
		private EMA ema5Short, ema5Long;

		private SMA sma1Short, sma1Long;
		private SMA sma2Short, sma2Long;
		private SMA sma3Short, sma3Long;
		private SMA sma4Short, sma4Long;
		private SMA sma5Short, sma5Long;

		private Series<double> ma1ShortSeries, ma1LongSeries;
		private Series<double> ma2ShortSeries, ma2LongSeries;
		private Series<double> ma3ShortSeries, ma3LongSeries;
		private Series<double> ma4ShortSeries, ma4LongSeries;
		private Series<double> ma5ShortSeries, ma5LongSeries;

		private SharpDX.Direct2D1.Brush dxCloud1Bull, dxCloud1Bear;
		private SharpDX.Direct2D1.Brush dxCloud2Bull, dxCloud2Bear;
		private SharpDX.Direct2D1.Brush dxCloud3Bull, dxCloud3Bear;
		private SharpDX.Direct2D1.Brush dxCloud4Bull, dxCloud4Bear;
		private SharpDX.Direct2D1.Brush dxCloud5Bull, dxCloud5Bear;
		private SharpDX.Direct2D1.Brush dxShortRising, dxShortFalling;
		private SharpDX.Direct2D1.Brush dxLongRising, dxLongFalling;
		private bool brushesNeedUpdate = true;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"RedTail EMA Cloud - Multiple EMA clouds for trend identification";
				Name										= "RedTail EMA Cloud";
				Calculate									= Calculate.OnBarClose;
				IsOverlay									= true;
				DisplayInDataBox							= true;
				DrawOnPricePanel							= true;
				DrawHorizontalGridLines						= true;
				DrawVerticalGridLines						= true;
				PaintPriceMarkers							= false;
				ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				IsSuspendedWhileInactive					= true;

				MAType										= MATypeOption.EMA;

				ShortEMA1Length								= 8;
				LongEMA1Length								= 9;
				ShortEMA2Length								= 5;
				LongEMA2Length								= 12;
				ShortEMA3Length								= 34;
				LongEMA3Length								= 50;
				ShortEMA4Length								= 72;
				LongEMA4Length								= 89;
				ShortEMA5Length								= 180;
				LongEMA5Length								= 200;

				ShowLongAlerts								= false;
				ShowShortAlerts								= false;
				ShowLine									= false;
				ShowEMACloud1								= true;
				ShowEMACloud2								= true;
				ShowEMACloud3								= true;
				ShowEMACloud4								= false;
				ShowEMACloud5								= false;
				EMACloudLeading								= 0;

				Cloud1BullColor								= Brushes.DarkGreen;
				Cloud1BearColor								= Brushes.DarkMagenta;
				Cloud1Opacity								= 45;

				Cloud2BullColor								= Brushes.LimeGreen;
				Cloud2BearColor								= Brushes.Red;
				Cloud2Opacity								= 65;

				Cloud3BullColor								= Brushes.DodgerBlue;
				Cloud3BearColor								= Brushes.Orange;
				Cloud3Opacity								= 70;

				Cloud4BullColor								= Brushes.Teal;
				Cloud4BearColor								= Brushes.HotPink;
				Cloud4Opacity								= 65;

				Cloud5BullColor								= Brushes.Cyan;
				Cloud5BearColor								= Brushes.OrangeRed;
				Cloud5Opacity								= 65;

				ShortEMARisingColor							= Brushes.Olive;
				ShortEMAFallingColor						= Brushes.Maroon;
				LongEMARisingColor							= Brushes.Green;
				LongEMAFallingColor							= Brushes.Red;
			}
			else if (State == State.DataLoaded)
			{
				// Use MaxValue as default so unset bars are easily detected
				ma1ShortSeries = new Series<double>(this, MaximumBarsLookBack.Infinite);
				ma1LongSeries  = new Series<double>(this, MaximumBarsLookBack.Infinite);
				ma2ShortSeries = new Series<double>(this, MaximumBarsLookBack.Infinite);
				ma2LongSeries  = new Series<double>(this, MaximumBarsLookBack.Infinite);
				ma3ShortSeries = new Series<double>(this, MaximumBarsLookBack.Infinite);
				ma3LongSeries  = new Series<double>(this, MaximumBarsLookBack.Infinite);
				ma4ShortSeries = new Series<double>(this, MaximumBarsLookBack.Infinite);
				ma4LongSeries  = new Series<double>(this, MaximumBarsLookBack.Infinite);
				ma5ShortSeries = new Series<double>(this, MaximumBarsLookBack.Infinite);
				ma5LongSeries  = new Series<double>(this, MaximumBarsLookBack.Infinite);

				if (MAType == MATypeOption.EMA)
				{
					ema1Short = EMA(ShortEMA1Length); ema1Long = EMA(LongEMA1Length);
					ema2Short = EMA(ShortEMA2Length); ema2Long = EMA(LongEMA2Length);
					ema3Short = EMA(ShortEMA3Length); ema3Long = EMA(LongEMA3Length);
					ema4Short = EMA(ShortEMA4Length); ema4Long = EMA(LongEMA4Length);
					ema5Short = EMA(ShortEMA5Length); ema5Long = EMA(LongEMA5Length);
				}
				else
				{
					sma1Short = SMA(ShortEMA1Length); sma1Long = SMA(LongEMA1Length);
					sma2Short = SMA(ShortEMA2Length); sma2Long = SMA(LongEMA2Length);
					sma3Short = SMA(ShortEMA3Length); sma3Long = SMA(LongEMA3Length);
					sma4Short = SMA(ShortEMA4Length); sma4Long = SMA(LongEMA4Length);
					sma5Short = SMA(ShortEMA5Length); sma5Long = SMA(LongEMA5Length);
				}
			}
			else if (State == State.Terminated)
			{
				DisposeBrushes();
			}
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar < Math.Max(Math.Max(Math.Max(LongEMA1Length, LongEMA2Length), Math.Max(LongEMA3Length, LongEMA4Length)), LongEMA5Length))
				return;

			bool isEMA = MAType == MATypeOption.EMA;

			ma1ShortSeries[0] = isEMA ? ema1Short[0] : sma1Short[0];
			ma1LongSeries[0]  = isEMA ? ema1Long[0]  : sma1Long[0];
			ma2ShortSeries[0] = isEMA ? ema2Short[0] : sma2Short[0];
			ma2LongSeries[0]  = isEMA ? ema2Long[0]  : sma2Long[0];
			ma3ShortSeries[0] = isEMA ? ema3Short[0] : sma3Short[0];
			ma3LongSeries[0]  = isEMA ? ema3Long[0]  : sma3Long[0];
			ma4ShortSeries[0] = isEMA ? ema4Short[0] : sma4Short[0];
			ma4LongSeries[0]  = isEMA ? ema4Long[0]  : sma4Long[0];
			ma5ShortSeries[0] = isEMA ? ema5Short[0] : sma5Short[0];
			ma5LongSeries[0]  = isEMA ? ema5Long[0]  : sma5Long[0];
		}

		private bool IsValidValue(double v)
		{
			return v != 0 && !double.IsNaN(v) && !double.IsInfinity(v);
		}

		#region SharpDX Rendering

		private SharpDX.Direct2D1.Brush CreateDXBrush(SharpDX.Direct2D1.RenderTarget rt, System.Windows.Media.Brush wmBrush, float alpha)
		{
			System.Windows.Media.SolidColorBrush scb = wmBrush as System.Windows.Media.SolidColorBrush;
			if (scb == null) return new SharpDX.Direct2D1.SolidColorBrush(rt, new SharpDX.Color(128, 128, 128, 128));
			return new SharpDX.Direct2D1.SolidColorBrush(rt, new SharpDX.Color(scb.Color.R, scb.Color.G, scb.Color.B, (byte)(alpha * 255)));
		}

		private void DisposeBrushes()
		{
			if (dxCloud1Bull != null) { dxCloud1Bull.Dispose(); dxCloud1Bull = null; }
			if (dxCloud1Bear != null) { dxCloud1Bear.Dispose(); dxCloud1Bear = null; }
			if (dxCloud2Bull != null) { dxCloud2Bull.Dispose(); dxCloud2Bull = null; }
			if (dxCloud2Bear != null) { dxCloud2Bear.Dispose(); dxCloud2Bear = null; }
			if (dxCloud3Bull != null) { dxCloud3Bull.Dispose(); dxCloud3Bull = null; }
			if (dxCloud3Bear != null) { dxCloud3Bear.Dispose(); dxCloud3Bear = null; }
			if (dxCloud4Bull != null) { dxCloud4Bull.Dispose(); dxCloud4Bull = null; }
			if (dxCloud4Bear != null) { dxCloud4Bear.Dispose(); dxCloud4Bear = null; }
			if (dxCloud5Bull != null) { dxCloud5Bull.Dispose(); dxCloud5Bull = null; }
			if (dxCloud5Bear != null) { dxCloud5Bear.Dispose(); dxCloud5Bear = null; }
			if (dxShortRising  != null) { dxShortRising.Dispose();  dxShortRising  = null; }
			if (dxShortFalling != null) { dxShortFalling.Dispose(); dxShortFalling = null; }
			if (dxLongRising   != null) { dxLongRising.Dispose();   dxLongRising   = null; }
			if (dxLongFalling  != null) { dxLongFalling.Dispose();  dxLongFalling  = null; }
		}

		public override void OnRenderTargetChanged()
		{
			DisposeBrushes();
			brushesNeedUpdate = true;
		}

		private void EnsureBrushes(SharpDX.Direct2D1.RenderTarget rt)
		{
			if (!brushesNeedUpdate) return;
			DisposeBrushes();

			dxCloud1Bull = CreateDXBrush(rt, Cloud1BullColor, (100 - Cloud1Opacity) / 100f);
			dxCloud1Bear = CreateDXBrush(rt, Cloud1BearColor, (100 - Cloud1Opacity) / 100f);
			dxCloud2Bull = CreateDXBrush(rt, Cloud2BullColor, (100 - Cloud2Opacity) / 100f);
			dxCloud2Bear = CreateDXBrush(rt, Cloud2BearColor, (100 - Cloud2Opacity) / 100f);
			dxCloud3Bull = CreateDXBrush(rt, Cloud3BullColor, (100 - Cloud3Opacity) / 100f);
			dxCloud3Bear = CreateDXBrush(rt, Cloud3BearColor, (100 - Cloud3Opacity) / 100f);
			dxCloud4Bull = CreateDXBrush(rt, Cloud4BullColor, (100 - Cloud4Opacity) / 100f);
			dxCloud4Bear = CreateDXBrush(rt, Cloud4BearColor, (100 - Cloud4Opacity) / 100f);
			dxCloud5Bull = CreateDXBrush(rt, Cloud5BullColor, (100 - Cloud5Opacity) / 100f);
			dxCloud5Bear = CreateDXBrush(rt, Cloud5BearColor, (100 - Cloud5Opacity) / 100f);

			dxShortRising  = CreateDXBrush(rt, ShortEMARisingColor,  1f);
			dxShortFalling = CreateDXBrush(rt, ShortEMAFallingColor, 1f);
			dxLongRising   = CreateDXBrush(rt, LongEMARisingColor,   1f);
			dxLongFalling  = CreateDXBrush(rt, LongEMAFallingColor,  1f);

			brushesNeedUpdate = false;
		}

		protected override void OnRender(ChartControl chartControl, ChartScale chartScale)
		{
			if (Bars == null || ChartBars == null) return;

			SharpDX.Direct2D1.RenderTarget rt = RenderTarget;
			if (rt == null) return;

			EnsureBrushes(rt);

			int firstIdx = ChartBars.FromIndex;
			int lastIdx  = ChartBars.ToIndex;

			if (ShowEMACloud1) RenderCloud(rt, chartControl, chartScale, ma1ShortSeries, ma1LongSeries, dxCloud1Bull, dxCloud1Bear, firstIdx, lastIdx);
			if (ShowEMACloud2) RenderCloud(rt, chartControl, chartScale, ma2ShortSeries, ma2LongSeries, dxCloud2Bull, dxCloud2Bear, firstIdx, lastIdx);
			if (ShowEMACloud3) RenderCloud(rt, chartControl, chartScale, ma3ShortSeries, ma3LongSeries, dxCloud3Bull, dxCloud3Bear, firstIdx, lastIdx);
			if (ShowEMACloud4) RenderCloud(rt, chartControl, chartScale, ma4ShortSeries, ma4LongSeries, dxCloud4Bull, dxCloud4Bear, firstIdx, lastIdx);
			if (ShowEMACloud5) RenderCloud(rt, chartControl, chartScale, ma5ShortSeries, ma5LongSeries, dxCloud5Bull, dxCloud5Bear, firstIdx, lastIdx);

			if (ShowLine)
			{
				if (ShowEMACloud1) RenderMALines(rt, chartControl, chartScale, ma1ShortSeries, ma1LongSeries, firstIdx, lastIdx);
				if (ShowEMACloud2) RenderMALines(rt, chartControl, chartScale, ma2ShortSeries, ma2LongSeries, firstIdx, lastIdx);
				if (ShowEMACloud3) RenderMALines(rt, chartControl, chartScale, ma3ShortSeries, ma3LongSeries, firstIdx, lastIdx);
				if (ShowEMACloud4) RenderMALines(rt, chartControl, chartScale, ma4ShortSeries, ma4LongSeries, firstIdx, lastIdx);
				if (ShowEMACloud5) RenderMALines(rt, chartControl, chartScale, ma5ShortSeries, ma5LongSeries, firstIdx, lastIdx);
			}
		}

		private void RenderCloud(SharpDX.Direct2D1.RenderTarget rt, ChartControl chartControl, ChartScale chartScale,
			Series<double> shortSeries, Series<double> longSeries,
			SharpDX.Direct2D1.Brush bullBrush, SharpDX.Direct2D1.Brush bearBrush,
			int firstIdx, int lastIdx)
		{
			int barCount = Bars.Count;
			if (barCount < 2) return;

			// Collect all valid bar data points for the visible range
			// Then batch consecutive same-direction segments into single path geometries
			List<float> xCoords = new List<float>(lastIdx - firstIdx + 2);
			List<float> yShort  = new List<float>(lastIdx - firstIdx + 2);
			List<float> yLong   = new List<float>(lastIdx - firstIdx + 2);
			List<bool>  isBull  = new List<bool>(lastIdx - firstIdx + 2);

			for (int idx = firstIdx; idx <= lastIdx; idx++)
			{
				if (idx < 0 || idx >= barCount) continue;

				double sVal = shortSeries.GetValueAt(idx);
				double lVal = longSeries.GetValueAt(idx);

				if (!IsValidValue(sVal) || !IsValidValue(lVal)) continue;

				xCoords.Add(chartControl.GetXByBarIndex(ChartBars, idx));
				yShort.Add(chartScale.GetYByValue(sVal));
				yLong.Add(chartScale.GetYByValue(lVal));
				isBull.Add(sVal >= lVal);
			}

			if (xCoords.Count < 2) return;

			// Batch consecutive same-direction segments into single geometries
			int startRun = 0;
			while (startRun < xCoords.Count - 1)
			{
				bool currentBull = isBull[startRun];
				int endRun = startRun + 1;

				// Extend run while direction stays the same
				while (endRun < xCoords.Count && isBull[endRun] == currentBull)
					endRun++;

				// Draw this run as a single filled polygon
				using (SharpDX.Direct2D1.PathGeometry geo = new SharpDX.Direct2D1.PathGeometry(rt.Factory))
				{
					using (GeometrySink sink = geo.Open())
					{
						// Forward along the short MA
						sink.BeginFigure(new Vector2(xCoords[startRun], yShort[startRun]), FigureBegin.Filled);
						for (int i = startRun + 1; i < endRun; i++)
							sink.AddLine(new Vector2(xCoords[i], yShort[i]));

						// Backward along the long MA
						for (int i = endRun - 1; i >= startRun; i--)
							sink.AddLine(new Vector2(xCoords[i], yLong[i]));

						sink.EndFigure(FigureEnd.Closed);
						sink.Close();
					}
					rt.FillGeometry(geo, currentBull ? bullBrush : bearBrush);
				}

				startRun = endRun;
			}
		}

		private void RenderMALines(SharpDX.Direct2D1.RenderTarget rt, ChartControl chartControl, ChartScale chartScale,
			Series<double> shortSeries, Series<double> longSeries, int firstIdx, int lastIdx)
		{
			int barCount = Bars.Count;

			float prevX = 0, prevYS = 0, prevYL = 0;
			double prevSVal = 0, prevLVal = 0;
			bool hasPrev = false;

			for (int idx = firstIdx; idx <= lastIdx; idx++)
			{
				if (idx < 0 || idx >= barCount) continue;

				double sVal = shortSeries.GetValueAt(idx);
				double lVal = longSeries.GetValueAt(idx);

				if (!IsValidValue(sVal) || !IsValidValue(lVal))
				{
					hasPrev = false;
					continue;
				}

				float x  = chartControl.GetXByBarIndex(ChartBars, idx);
				float yS = chartScale.GetYByValue(sVal);
				float yL = chartScale.GetYByValue(lVal);

				if (hasPrev)
				{
					// Short MA line
					rt.DrawLine(new Vector2(prevX, prevYS), new Vector2(x, yS),
						sVal >= prevSVal ? dxShortRising : dxShortFalling, 1f);

					// Long MA line
					rt.DrawLine(new Vector2(prevX, prevYL), new Vector2(x, yL),
						lVal >= prevLVal ? dxLongRising : dxLongFalling, 3f);
				}

				prevX = x; prevYS = yS; prevYL = yL;
				prevSVal = sVal; prevLVal = lVal;
				hasPrev = true;
			}
		}

		#endregion

		#region Properties

		[NinjaScriptProperty]
		[Display(Name="MA Type", Description="Moving Average Type", Order=1, GroupName="Parameters")]
		public MATypeOption MAType
		{ get; set; }

		[Range(1, int.MaxValue)]
		[NinjaScriptProperty]
		[Display(Name="Short EMA1 Length", Description="Short EMA1 Length", Order=2, GroupName="Parameters")]
		public int ShortEMA1Length
		{ get; set; }

		[Range(1, int.MaxValue)]
		[NinjaScriptProperty]
		[Display(Name="Long EMA1 Length", Description="Long EMA1 Length", Order=3, GroupName="Parameters")]
		public int LongEMA1Length
		{ get; set; }

		[Range(1, int.MaxValue)]
		[NinjaScriptProperty]
		[Display(Name="Short EMA2 Length", Description="Short EMA2 Length", Order=4, GroupName="Parameters")]
		public int ShortEMA2Length
		{ get; set; }

		[Range(1, int.MaxValue)]
		[NinjaScriptProperty]
		[Display(Name="Long EMA2 Length", Description="Long EMA2 Length", Order=5, GroupName="Parameters")]
		public int LongEMA2Length
		{ get; set; }

		[Range(1, int.MaxValue)]
		[NinjaScriptProperty]
		[Display(Name="Short EMA3 Length", Description="Short EMA3 Length", Order=6, GroupName="Parameters")]
		public int ShortEMA3Length
		{ get; set; }

		[Range(1, int.MaxValue)]
		[NinjaScriptProperty]
		[Display(Name="Long EMA3 Length", Description="Long EMA3 Length", Order=7, GroupName="Parameters")]
		public int LongEMA3Length
		{ get; set; }

		[Range(1, int.MaxValue)]
		[NinjaScriptProperty]
		[Display(Name="Short EMA4 Length", Description="Short EMA4 Length", Order=8, GroupName="Parameters")]
		public int ShortEMA4Length
		{ get; set; }

		[Range(1, int.MaxValue)]
		[NinjaScriptProperty]
		[Display(Name="Long EMA4 Length", Description="Long EMA4 Length", Order=9, GroupName="Parameters")]
		public int LongEMA4Length
		{ get; set; }

		[Range(1, int.MaxValue)]
		[NinjaScriptProperty]
		[Display(Name="Short EMA5 Length", Description="Short EMA5 Length", Order=10, GroupName="Parameters")]
		public int ShortEMA5Length
		{ get; set; }

		[Range(1, int.MaxValue)]
		[NinjaScriptProperty]
		[Display(Name="Long EMA5 Length", Description="Long EMA5 Length", Order=11, GroupName="Parameters")]
		public int LongEMA5Length
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="Show Long Alerts", Description="Show Long Alerts", Order=1, GroupName="Display Options")]
		public bool ShowLongAlerts
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="Show Short Alerts", Description="Show Short Alerts", Order=2, GroupName="Display Options")]
		public bool ShowShortAlerts
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="Display EMA Line", Description="Display EMA Line", Order=3, GroupName="Display Options")]
		public bool ShowLine
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="Show EMA Cloud-1", Description="Show EMA Cloud-1", Order=4, GroupName="Display Options")]
		public bool ShowEMACloud1
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="Show EMA Cloud-2", Description="Show EMA Cloud-2", Order=5, GroupName="Display Options")]
		public bool ShowEMACloud2
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="Show EMA Cloud-3", Description="Show EMA Cloud-3", Order=6, GroupName="Display Options")]
		public bool ShowEMACloud3
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="Show EMA Cloud-4", Description="Show EMA Cloud-4", Order=7, GroupName="Display Options")]
		public bool ShowEMACloud4
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="Show EMA Cloud-5", Description="Show EMA Cloud-5", Order=8, GroupName="Display Options")]
		public bool ShowEMACloud5
		{ get; set; }

		[Range(0, int.MaxValue)]
		[NinjaScriptProperty]
		[Display(Name="Leading Period For EMA Cloud", Description="Leading Period For EMA Cloud", Order=9, GroupName="Display Options")]
		public int EMACloudLeading
		{ get; set; }

		[XmlIgnore]
		[Display(Name="Cloud 1 - Bullish Color", Description="Cloud 1 Bullish Color", Order=1, GroupName="Cloud 1 Colors")]
		public System.Windows.Media.Brush Cloud1BullColor
		{ get; set; }

		[Browsable(false)]
		public string Cloud1BullColorSerializable
		{
			get { return Serialize.BrushToString(Cloud1BullColor); }
			set { Cloud1BullColor = Serialize.StringToBrush(value); }
		}

		[XmlIgnore]
		[Display(Name="Cloud 1 - Bearish Color", Description="Cloud 1 Bearish Color", Order=2, GroupName="Cloud 1 Colors")]
		public System.Windows.Media.Brush Cloud1BearColor
		{ get; set; }

		[Browsable(false)]
		public string Cloud1BearColorSerializable
		{
			get { return Serialize.BrushToString(Cloud1BearColor); }
			set { Cloud1BearColor = Serialize.StringToBrush(value); }
		}

		[Range(0, 100)]
		[NinjaScriptProperty]
		[Display(Name="Cloud 1 - Opacity", Description="Cloud 1 Opacity (0-100)", Order=3, GroupName="Cloud 1 Colors")]
		public int Cloud1Opacity
		{ get; set; }

		[XmlIgnore]
		[Display(Name="Cloud 2 - Bullish Color", Description="Cloud 2 Bullish Color", Order=1, GroupName="Cloud 2 Colors")]
		public System.Windows.Media.Brush Cloud2BullColor
		{ get; set; }

		[Browsable(false)]
		public string Cloud2BullColorSerializable
		{
			get { return Serialize.BrushToString(Cloud2BullColor); }
			set { Cloud2BullColor = Serialize.StringToBrush(value); }
		}

		[XmlIgnore]
		[Display(Name="Cloud 2 - Bearish Color", Description="Cloud 2 Bearish Color", Order=2, GroupName="Cloud 2 Colors")]
		public System.Windows.Media.Brush Cloud2BearColor
		{ get; set; }

		[Browsable(false)]
		public string Cloud2BearColorSerializable
		{
			get { return Serialize.BrushToString(Cloud2BearColor); }
			set { Cloud2BearColor = Serialize.StringToBrush(value); }
		}

		[Range(0, 100)]
		[NinjaScriptProperty]
		[Display(Name="Cloud 2 - Opacity", Description="Cloud 2 Opacity (0-100)", Order=3, GroupName="Cloud 2 Colors")]
		public int Cloud2Opacity
		{ get; set; }

		[XmlIgnore]
		[Display(Name="Cloud 3 - Bullish Color", Description="Cloud 3 Bullish Color", Order=1, GroupName="Cloud 3 Colors")]
		public System.Windows.Media.Brush Cloud3BullColor
		{ get; set; }

		[Browsable(false)]
		public string Cloud3BullColorSerializable
		{
			get { return Serialize.BrushToString(Cloud3BullColor); }
			set { Cloud3BullColor = Serialize.StringToBrush(value); }
		}

		[XmlIgnore]
		[Display(Name="Cloud 3 - Bearish Color", Description="Cloud 3 Bearish Color", Order=2, GroupName="Cloud 3 Colors")]
		public System.Windows.Media.Brush Cloud3BearColor
		{ get; set; }

		[Browsable(false)]
		public string Cloud3BearColorSerializable
		{
			get { return Serialize.BrushToString(Cloud3BearColor); }
			set { Cloud3BearColor = Serialize.StringToBrush(value); }
		}

		[Range(0, 100)]
		[NinjaScriptProperty]
		[Display(Name="Cloud 3 - Opacity", Description="Cloud 3 Opacity (0-100)", Order=3, GroupName="Cloud 3 Colors")]
		public int Cloud3Opacity
		{ get; set; }

		[XmlIgnore]
		[Display(Name="Cloud 4 - Bullish Color", Description="Cloud 4 Bullish Color", Order=1, GroupName="Cloud 4 Colors")]
		public System.Windows.Media.Brush Cloud4BullColor
		{ get; set; }

		[Browsable(false)]
		public string Cloud4BullColorSerializable
		{
			get { return Serialize.BrushToString(Cloud4BullColor); }
			set { Cloud4BullColor = Serialize.StringToBrush(value); }
		}

		[XmlIgnore]
		[Display(Name="Cloud 4 - Bearish Color", Description="Cloud 4 Bearish Color", Order=2, GroupName="Cloud 4 Colors")]
		public System.Windows.Media.Brush Cloud4BearColor
		{ get; set; }

		[Browsable(false)]
		public string Cloud4BearColorSerializable
		{
			get { return Serialize.BrushToString(Cloud4BearColor); }
			set { Cloud4BearColor = Serialize.StringToBrush(value); }
		}

		[Range(0, 100)]
		[NinjaScriptProperty]
		[Display(Name="Cloud 4 - Opacity", Description="Cloud 4 Opacity (0-100)", Order=3, GroupName="Cloud 4 Colors")]
		public int Cloud4Opacity
		{ get; set; }

		[XmlIgnore]
		[Display(Name="Cloud 5 - Bullish Color", Description="Cloud 5 Bullish Color", Order=1, GroupName="Cloud 5 Colors")]
		public System.Windows.Media.Brush Cloud5BullColor
		{ get; set; }

		[Browsable(false)]
		public string Cloud5BullColorSerializable
		{
			get { return Serialize.BrushToString(Cloud5BullColor); }
			set { Cloud5BullColor = Serialize.StringToBrush(value); }
		}

		[XmlIgnore]
		[Display(Name="Cloud 5 - Bearish Color", Description="Cloud 5 Bearish Color", Order=2, GroupName="Cloud 5 Colors")]
		public System.Windows.Media.Brush Cloud5BearColor
		{ get; set; }

		[Browsable(false)]
		public string Cloud5BearColorSerializable
		{
			get { return Serialize.BrushToString(Cloud5BearColor); }
			set { Cloud5BearColor = Serialize.StringToBrush(value); }
		}

		[Range(0, 100)]
		[NinjaScriptProperty]
		[Display(Name="Cloud 5 - Opacity", Description="Cloud 5 Opacity (0-100)", Order=3, GroupName="Cloud 5 Colors")]
		public int Cloud5Opacity
		{ get; set; }

		[XmlIgnore]
		[Display(Name="Short EMA - Rising Color", Description="Short EMA Rising Color", Order=1, GroupName="EMA Line Colors")]
		public System.Windows.Media.Brush ShortEMARisingColor
		{ get; set; }

		[Browsable(false)]
		public string ShortEMARisingColorSerializable
		{
			get { return Serialize.BrushToString(ShortEMARisingColor); }
			set { ShortEMARisingColor = Serialize.StringToBrush(value); }
		}

		[XmlIgnore]
		[Display(Name="Short EMA - Falling Color", Description="Short EMA Falling Color", Order=2, GroupName="EMA Line Colors")]
		public System.Windows.Media.Brush ShortEMAFallingColor
		{ get; set; }

		[Browsable(false)]
		public string ShortEMAFallingColorSerializable
		{
			get { return Serialize.BrushToString(ShortEMAFallingColor); }
			set { ShortEMAFallingColor = Serialize.StringToBrush(value); }
		}

		[XmlIgnore]
		[Display(Name="Long EMA - Rising Color", Description="Long EMA Rising Color", Order=3, GroupName="EMA Line Colors")]
		public System.Windows.Media.Brush LongEMARisingColor
		{ get; set; }

		[Browsable(false)]
		public string LongEMARisingColorSerializable
		{
			get { return Serialize.BrushToString(LongEMARisingColor); }
			set { LongEMARisingColor = Serialize.StringToBrush(value); }
		}

		[XmlIgnore]
		[Display(Name="Long EMA - Falling Color", Description="Long EMA Falling Color", Order=4, GroupName="EMA Line Colors")]
		public System.Windows.Media.Brush LongEMAFallingColor
		{ get; set; }

		[Browsable(false)]
		public string LongEMAFallingColorSerializable
		{
			get { return Serialize.BrushToString(LongEMAFallingColor); }
			set { LongEMAFallingColor = Serialize.StringToBrush(value); }
		}

		#endregion
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private RedTailEMACloud[] cacheRedTailEMACloud;
		public RedTailEMACloud RedTailEMACloud(MATypeOption mAType, int shortEMA1Length, int longEMA1Length, int shortEMA2Length, int longEMA2Length, int shortEMA3Length, int longEMA3Length, int shortEMA4Length, int longEMA4Length, int shortEMA5Length, int longEMA5Length, bool showLongAlerts, bool showShortAlerts, bool showLine, bool showEMACloud1, bool showEMACloud2, bool showEMACloud3, bool showEMACloud4, bool showEMACloud5, int eMACloudLeading, int cloud1Opacity, int cloud2Opacity, int cloud3Opacity, int cloud4Opacity, int cloud5Opacity)
		{
			return RedTailEMACloud(Input, mAType, shortEMA1Length, longEMA1Length, shortEMA2Length, longEMA2Length, shortEMA3Length, longEMA3Length, shortEMA4Length, longEMA4Length, shortEMA5Length, longEMA5Length, showLongAlerts, showShortAlerts, showLine, showEMACloud1, showEMACloud2, showEMACloud3, showEMACloud4, showEMACloud5, eMACloudLeading, cloud1Opacity, cloud2Opacity, cloud3Opacity, cloud4Opacity, cloud5Opacity);
		}

		public RedTailEMACloud RedTailEMACloud(ISeries<double> input, MATypeOption mAType, int shortEMA1Length, int longEMA1Length, int shortEMA2Length, int longEMA2Length, int shortEMA3Length, int longEMA3Length, int shortEMA4Length, int longEMA4Length, int shortEMA5Length, int longEMA5Length, bool showLongAlerts, bool showShortAlerts, bool showLine, bool showEMACloud1, bool showEMACloud2, bool showEMACloud3, bool showEMACloud4, bool showEMACloud5, int eMACloudLeading, int cloud1Opacity, int cloud2Opacity, int cloud3Opacity, int cloud4Opacity, int cloud5Opacity)
		{
			if (cacheRedTailEMACloud != null)
				for (int idx = 0; idx < cacheRedTailEMACloud.Length; idx++)
					if (cacheRedTailEMACloud[idx] != null && cacheRedTailEMACloud[idx].MAType == mAType && cacheRedTailEMACloud[idx].ShortEMA1Length == shortEMA1Length && cacheRedTailEMACloud[idx].LongEMA1Length == longEMA1Length && cacheRedTailEMACloud[idx].ShortEMA2Length == shortEMA2Length && cacheRedTailEMACloud[idx].LongEMA2Length == longEMA2Length && cacheRedTailEMACloud[idx].ShortEMA3Length == shortEMA3Length && cacheRedTailEMACloud[idx].LongEMA3Length == longEMA3Length && cacheRedTailEMACloud[idx].ShortEMA4Length == shortEMA4Length && cacheRedTailEMACloud[idx].LongEMA4Length == longEMA4Length && cacheRedTailEMACloud[idx].ShortEMA5Length == shortEMA5Length && cacheRedTailEMACloud[idx].LongEMA5Length == longEMA5Length && cacheRedTailEMACloud[idx].ShowLongAlerts == showLongAlerts && cacheRedTailEMACloud[idx].ShowShortAlerts == showShortAlerts && cacheRedTailEMACloud[idx].ShowLine == showLine && cacheRedTailEMACloud[idx].ShowEMACloud1 == showEMACloud1 && cacheRedTailEMACloud[idx].ShowEMACloud2 == showEMACloud2 && cacheRedTailEMACloud[idx].ShowEMACloud3 == showEMACloud3 && cacheRedTailEMACloud[idx].ShowEMACloud4 == showEMACloud4 && cacheRedTailEMACloud[idx].ShowEMACloud5 == showEMACloud5 && cacheRedTailEMACloud[idx].EMACloudLeading == eMACloudLeading && cacheRedTailEMACloud[idx].Cloud1Opacity == cloud1Opacity && cacheRedTailEMACloud[idx].Cloud2Opacity == cloud2Opacity && cacheRedTailEMACloud[idx].Cloud3Opacity == cloud3Opacity && cacheRedTailEMACloud[idx].Cloud4Opacity == cloud4Opacity && cacheRedTailEMACloud[idx].Cloud5Opacity == cloud5Opacity && cacheRedTailEMACloud[idx].EqualsInput(input))
						return cacheRedTailEMACloud[idx];
			return CacheIndicator<RedTailEMACloud>(new RedTailEMACloud(){ MAType = mAType, ShortEMA1Length = shortEMA1Length, LongEMA1Length = longEMA1Length, ShortEMA2Length = shortEMA2Length, LongEMA2Length = longEMA2Length, ShortEMA3Length = shortEMA3Length, LongEMA3Length = longEMA3Length, ShortEMA4Length = shortEMA4Length, LongEMA4Length = longEMA4Length, ShortEMA5Length = shortEMA5Length, LongEMA5Length = longEMA5Length, ShowLongAlerts = showLongAlerts, ShowShortAlerts = showShortAlerts, ShowLine = showLine, ShowEMACloud1 = showEMACloud1, ShowEMACloud2 = showEMACloud2, ShowEMACloud3 = showEMACloud3, ShowEMACloud4 = showEMACloud4, ShowEMACloud5 = showEMACloud5, EMACloudLeading = eMACloudLeading, Cloud1Opacity = cloud1Opacity, Cloud2Opacity = cloud2Opacity, Cloud3Opacity = cloud3Opacity, Cloud4Opacity = cloud4Opacity, Cloud5Opacity = cloud5Opacity }, input, ref cacheRedTailEMACloud);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.RedTailEMACloud RedTailEMACloud(MATypeOption mAType, int shortEMA1Length, int longEMA1Length, int shortEMA2Length, int longEMA2Length, int shortEMA3Length, int longEMA3Length, int shortEMA4Length, int longEMA4Length, int shortEMA5Length, int longEMA5Length, bool showLongAlerts, bool showShortAlerts, bool showLine, bool showEMACloud1, bool showEMACloud2, bool showEMACloud3, bool showEMACloud4, bool showEMACloud5, int eMACloudLeading, int cloud1Opacity, int cloud2Opacity, int cloud3Opacity, int cloud4Opacity, int cloud5Opacity)
		{
			return indicator.RedTailEMACloud(Input, mAType, shortEMA1Length, longEMA1Length, shortEMA2Length, longEMA2Length, shortEMA3Length, longEMA3Length, shortEMA4Length, longEMA4Length, shortEMA5Length, longEMA5Length, showLongAlerts, showShortAlerts, showLine, showEMACloud1, showEMACloud2, showEMACloud3, showEMACloud4, showEMACloud5, eMACloudLeading, cloud1Opacity, cloud2Opacity, cloud3Opacity, cloud4Opacity, cloud5Opacity);
		}

		public Indicators.RedTailEMACloud RedTailEMACloud(ISeries<double> input , MATypeOption mAType, int shortEMA1Length, int longEMA1Length, int shortEMA2Length, int longEMA2Length, int shortEMA3Length, int longEMA3Length, int shortEMA4Length, int longEMA4Length, int shortEMA5Length, int longEMA5Length, bool showLongAlerts, bool showShortAlerts, bool showLine, bool showEMACloud1, bool showEMACloud2, bool showEMACloud3, bool showEMACloud4, bool showEMACloud5, int eMACloudLeading, int cloud1Opacity, int cloud2Opacity, int cloud3Opacity, int cloud4Opacity, int cloud5Opacity)
		{
			return indicator.RedTailEMACloud(input, mAType, shortEMA1Length, longEMA1Length, shortEMA2Length, longEMA2Length, shortEMA3Length, longEMA3Length, shortEMA4Length, longEMA4Length, shortEMA5Length, longEMA5Length, showLongAlerts, showShortAlerts, showLine, showEMACloud1, showEMACloud2, showEMACloud3, showEMACloud4, showEMACloud5, eMACloudLeading, cloud1Opacity, cloud2Opacity, cloud3Opacity, cloud4Opacity, cloud5Opacity);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.RedTailEMACloud RedTailEMACloud(MATypeOption mAType, int shortEMA1Length, int longEMA1Length, int shortEMA2Length, int longEMA2Length, int shortEMA3Length, int longEMA3Length, int shortEMA4Length, int longEMA4Length, int shortEMA5Length, int longEMA5Length, bool showLongAlerts, bool showShortAlerts, bool showLine, bool showEMACloud1, bool showEMACloud2, bool showEMACloud3, bool showEMACloud4, bool showEMACloud5, int eMACloudLeading, int cloud1Opacity, int cloud2Opacity, int cloud3Opacity, int cloud4Opacity, int cloud5Opacity)
		{
			return indicator.RedTailEMACloud(Input, mAType, shortEMA1Length, longEMA1Length, shortEMA2Length, longEMA2Length, shortEMA3Length, longEMA3Length, shortEMA4Length, longEMA4Length, shortEMA5Length, longEMA5Length, showLongAlerts, showShortAlerts, showLine, showEMACloud1, showEMACloud2, showEMACloud3, showEMACloud4, showEMACloud5, eMACloudLeading, cloud1Opacity, cloud2Opacity, cloud3Opacity, cloud4Opacity, cloud5Opacity);
		}

		public Indicators.RedTailEMACloud RedTailEMACloud(ISeries<double> input , MATypeOption mAType, int shortEMA1Length, int longEMA1Length, int shortEMA2Length, int longEMA2Length, int shortEMA3Length, int longEMA3Length, int shortEMA4Length, int longEMA4Length, int shortEMA5Length, int longEMA5Length, bool showLongAlerts, bool showShortAlerts, bool showLine, bool showEMACloud1, bool showEMACloud2, bool showEMACloud3, bool showEMACloud4, bool showEMACloud5, int eMACloudLeading, int cloud1Opacity, int cloud2Opacity, int cloud3Opacity, int cloud4Opacity, int cloud5Opacity)
		{
			return indicator.RedTailEMACloud(input, mAType, shortEMA1Length, longEMA1Length, shortEMA2Length, longEMA2Length, shortEMA3Length, longEMA3Length, shortEMA4Length, longEMA4Length, shortEMA5Length, longEMA5Length, showLongAlerts, showShortAlerts, showLine, showEMACloud1, showEMACloud2, showEMACloud3, showEMACloud4, showEMACloud5, eMACloudLeading, cloud1Opacity, cloud2Opacity, cloud3Opacity, cloud4Opacity, cloud5Opacity);
		}
	}
}

#endregion
