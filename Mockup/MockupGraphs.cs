using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

public class ScatterPlot
{
	private List<Point> data;
	private string xLabel = "Avg Meters";
	private int xMin = 0;
	private int xMax = 90;
	//private string yLabel = "Room Temperature (F)";
	private int yMin = 20;
	private int yMax = 120;
	
	public ScatterPlot()
	{
		data = DataSource.RandomHighXHighY(2, 50, xMin, yMin, xMax, yMax);
	}
	
	public void Draw(Graphics g, int width, int height)
	{
		int borderPadding = 10;
		int yAxisPadding = 40;
		int xAxisPadding = 40;
		g.Clear(Color.White);
		g.SmoothingMode = SmoothingMode.AntiAlias;
		Pen pen = new Pen(Color.Black, 2);
		
		//axes
		int xAxisY = height - borderPadding - xAxisPadding;
		int xAxisWidth = width - (borderPadding*2) - yAxisPadding;
		int yAxisX = borderPadding + yAxisPadding;
		int yAxisHeight = height - (borderPadding*2) - xAxisPadding;
		g.DrawLine(pen, yAxisX, xAxisY, yAxisX + xAxisWidth, xAxisY);
		g.DrawLine(pen, yAxisX, xAxisY, yAxisX, xAxisY - yAxisHeight);
		
		//axis ticks and points
		Pen tickPen = new Pen(Color.Black, 1);
		int tickLength = 5;
		int pointRadius = 1;
		foreach(Point point in data)
		{
			int x = yAxisX + (xAxisWidth * (point.X - xMin) / (xMax - xMin));
			int y = xAxisY - (yAxisHeight * (point.Y - yMin) / (yMax - yMin));
			g.DrawLine(tickPen, x, xAxisY, x, xAxisY + tickLength);
			g.DrawLine(tickPen, yAxisX, y, yAxisX - tickLength, y);
			g.DrawArc(pen, x-pointRadius, y-pointRadius, pointRadius*2, pointRadius*2, 0, 360);
		}
		
		//x axis numbers
		int xNumbersY = xAxisY + tickLength + 1;
		Font font = new Font(new FontFamily("Arial"), 10);
		TextMeasure textMeasure = new TextMeasure(g, font, xMin.ToString());
		textMeasure.DrawCenterX(Brushes.Black, yAxisX, xNumbersY);
		textMeasure = new TextMeasure(g, font, ((int)(xMax/3)).ToString());
		textMeasure.DrawCenterX(Brushes.Black, yAxisX + (int)(xAxisWidth/3), xNumbersY);
		textMeasure = new TextMeasure(g, font, ((int)(2*xMax/3)).ToString());
		textMeasure.DrawCenterX(Brushes.Black, yAxisX + (int)(2*xAxisWidth/3), xNumbersY);
		textMeasure = new TextMeasure(g, font, xMax.ToString());
		textMeasure.DrawCenterX(Brushes.Black, yAxisX + xAxisWidth, xNumbersY);
		
		//x axis label
		int xLabelY = xNumbersY + textMeasure.Height + 2;
		textMeasure = new TextMeasure(g, font, xLabel);
		textMeasure.DrawCenterX(Brushes.Black, yAxisX + ((int)(xAxisWidth/2)), xLabelY);
		
		//y axis numbers
		int yNumbersX = yAxisX - tickLength - 1;
		textMeasure = new TextMeasure(g, font, yMin+"˚F");
		textMeasure.DrawRightAlign(Brushes.Black, yNumbersX, xAxisY);
		textMeasure = new TextMeasure(g, font, (yMin+((yMax-yMin)/5))+"˚F");
		textMeasure.DrawRightAlign(Brushes.Black, yNumbersX, xAxisY-(int)(yAxisHeight/5));
		textMeasure = new TextMeasure(g, font, (yMin+(2*(yMax-yMin)/5))+"˚F");
		textMeasure.DrawRightAlign(Brushes.Black, yNumbersX, xAxisY-(int)(2*yAxisHeight/5));
		textMeasure = new TextMeasure(g, font, (yMin+(3*(yMax-yMin)/5))+"˚F");
		textMeasure.DrawRightAlign(Brushes.Black, yNumbersX, xAxisY-(int)(3*yAxisHeight/5));
		textMeasure = new TextMeasure(g, font, (yMin+(4*(yMax-yMin)/5))+"˚F");
		textMeasure.DrawRightAlign(Brushes.Black, yNumbersX, xAxisY-(int)(4*yAxisHeight/5));
		textMeasure = new TextMeasure(g, font, yMax+"˚F");
		textMeasure.DrawRightAlign(Brushes.Black, yNumbersX, xAxisY - yAxisHeight);
	}
}

//todo use images of vehicles instead of words
public class PieChartSmallMultiples
{
	private string[] vehicles = new string[] { "Car", "Truck", "Motorcycle", "Bus" };
	private Color[] colors = new Color[] {
		Color.FromArgb(255,202,202) //red
		, Color.FromArgb(255,211,168) //red orange
		, Color.FromArgb(255,234,185) //orange
		, Color.FromArgb(255,244,183) //yellow orange
		, Color.FromArgb(253,255,185) //yellow
		, Color.FromArgb(241,255,191) //yellow green
		, Color.FromArgb(187,255,230) //turquoise
		, Color.FromArgb(191,255,255) //cyan
		, Color.FromArgb(183,232,255) //blue
		, Color.FromArgb(204,215,255) //darkblue
		, Color.FromArgb(234,217,255) //blueviolet
		, Color.FromArgb(247,215,255) //redviolet
		, Color.FromArgb(255,210,247) //magenta
		, Color.FromArgb(255,213,231) //pinky red
	};
	private Dictionary<string, Color> vehicleColors;
	private List<Point> rawData;
	private List<TrafficData> summaryData;
	private int hourMin = 6;
	private int hourMax = 18;
	
	public PieChartSmallMultiples()
	{
		rawData = DataSource.Random(5, 1000, hourMin, 0, hourMax, vehicles.Length-1);
		SummarizeData();
		vehicleColors = new Dictionary<string, Color> {
			{ vehicles[0], colors[0] }
			, { vehicles[1], colors[3] }
			, { vehicles[2], colors[6] }
			, { vehicles[3], colors[8] }
		};
	}
	
	private void SummarizeData()
	{
		summaryData = new List<TrafficData>() {
			new TrafficData(vehicles) { HourMin=6, HourMax=8 }
			, new TrafficData(vehicles) { HourMin=8, HourMax=10 }
			, new TrafficData(vehicles) { HourMin=10, HourMax=12 }
			, new TrafficData(vehicles) { HourMin=12, HourMax=14 }
			, new TrafficData(vehicles) { HourMin=14, HourMax=16 }
			, new TrafficData(vehicles) { HourMin=16, HourMax=18 }
		};
		foreach(Point point in rawData)
		{
			foreach(TrafficData trafficData in summaryData)
			{
				if(trafficData.HourInRange(point.X))
					trafficData.VehicleCounts[vehicles[point.Y]]++;
			}
		}
	}

	public void Draw(Graphics g, int width, int height)
	{
		int borderPadding = 4;
		int titlePadding = 40;
		int rowCount = 2;
		int colCount = 3;
		int pieHeight = (int)((height - (borderPadding*2) - titlePadding) / rowCount);
		int pieWidth = (int)((width - (borderPadding*2)) / colCount);
		int piePadding = 15;
		
		g.Clear(Color.White);
		g.SmoothingMode = SmoothingMode.AntiAlias;
		Pen pen = new Pen(Color.Black, 2);

		//title
		Font font = new Font(new FontFamily("Times New Roman"), 10);
		TextMeasure textMeasure = new TextMeasure(g, font, "Glenville Weekday Traffic");
		textMeasure.DrawCenterX(Brushes.Black, (int)(width/2), titlePadding - borderPadding*3);
		
		//pies
		Cell cell = new Cell(rowCount, colCount);
		foreach(TrafficData summary in summaryData)
		{
			int vehicleCount = summary.TotalCount;
			int outerX = borderPadding + (cell.Col * pieWidth);
			int outerY = borderPadding + titlePadding + (cell.Row * pieHeight);
			int squareX = outerX + piePadding;
			int squareY = outerY + piePadding;
			int centerX = squareX + (int)(pieWidth/2) - piePadding;
			int centerY = squareY + (int)(pieHeight/2) - piePadding;
			int radius = Math.Min(centerX-squareX, centerY-squareY);
			int degrees = 0;

//			pen = new Pen(Color.Black, 1);
//			g.DrawArc(pen, centerX-radius-1, centerY-radius-1, radius*2+2, radius*2+2, 0, 360);

			//hour labels
			textMeasure = new TextMeasure(g, font, FormatHour(summary.HourMin));
			if(cell.Col == 0)
				textMeasure.Draw(Brushes.Black, squareX, squareY);
			else
				textMeasure.DrawCenterX(Brushes.Black, squareX, squareY);
			textMeasure = new TextMeasure(g, font, FormatHour(summary.HourMax));
			if(cell.Col < colCount-1)
				textMeasure.DrawCenterX(Brushes.Black, squareX+pieWidth, squareY);
			else
				textMeasure.DrawRightAlign(Brushes.Black, squareX+pieWidth, squareY);
			
			//pie slices
			foreach(KeyValuePair<string, int> pair in summary.VehicleCounts)
			{
				int plusDegrees = (int)(360*pair.Value/vehicleCount);
				Pen colorPen = new Pen(vehicleColors[pair.Key], 2);
				//SolidBrush colorBrush = new SolidBrush(VehicleColor(pair.Key));
				g.DrawArc(colorPen, centerX-radius, centerY-radius, radius*2, radius*2, degrees, plusDegrees);
				//g.FillArc(colorBrush, xStart, yStart, xEnd, yEnd, 0, 360);
				degrees += plusDegrees;
			}
			cell.Increment();
		}
	}

	private string FormatHour(int hour)
	{
		if(hour < 12) 
			return String.Format("{0} AM", hour);
		if(hour == 12)
			return "12 Noon";
		return String.Format("{0} PM", hour-12);
	}
	
	private Color VehicleColor(string vehicle)
	{
		if(vehicle == vehicles[0])
			return colors[0];
		else if(vehicle == vehicles[1])
			return colors[1];
		else if(vehicle == vehicles[2])
			return colors[2];
		else if(vehicle == vehicles[3])
			return colors[3];
		else
			return colors[8];
	}
}

public struct Cell
{
	public int Row;
	public int Col;
	private int rows;
	private int cols;
	
	public Cell(int r, int c)
	{
		rows = r;
		cols = c;
		Row = 0;
		Col = 0;
	}
	
	public void Reset()
	{
		Row = 0;
		Col = 0;
	}
	
	public void Increment()
	{
		Col++;
		if(Col > cols-1)
		{
			Row++;
			Col = 0;
		}
	}
}

public class TrafficData
{
	public Dictionary<string, int> VehicleCounts { get; set; }
	public int HourMin { get; set; }
	public int HourMax { get; set; }
	public int TotalCount {
		get {
			int count = 0;
			foreach(KeyValuePair<string,int> pair in VehicleCounts)
			{
				count += pair.Value;
			}
			return count;
		}
	}
	
	public TrafficData(string[] vehicles)
	{
		VehicleCounts = new Dictionary<string, int>();
		foreach(string v in vehicles)
		{
			VehicleCounts[v] = 0;
		}
	}
	
	public bool HourInRange(int hour)
	{
		return (HourMin <= hour && hour <= HourMax);
	}
}

public class TextMeasure
{
	public string Text;
	private Graphics g;
	public Font Font;
	private SizeF size;
	
	public int Width { get { return (int)size.Width; } }
	public int Height { get { return (int)size.Height; } }
	
	public TextMeasure(Graphics g, Font font, string text)
	{
		this.g = g;
		this.Font = font;
		this.Text = text;
		size = g.MeasureString(Text, Font);
	}
	
	public void Draw(Brush brush, int x, int y)
	{
		g.DrawString(Text, Font, brush, x, y);
	}
	
	public void DrawCenterX(Brush brush, int x, int y)
	{
		g.DrawString(Text, Font, brush, CenterX(x), y);
	}
	
	public void DrawRightAlign(Brush brush, int x, int y)
	{
		g.DrawString(Text, Font, brush, RightAlign(x), CenterY(y));
	}
	
	public int RightAlign(int x)
	{
		return x - (int)size.Width;
	}
	
	public int CenterX(int x)
	{
		return x - (int)(size.Width / 2);
	}
	
	public int CenterY (int y)
	{
		return y - (int)(size.Height / 2);
	}
}

public class DataSource
{
	public static List<Point> Random(int seed, int count, int xMin, int yMin, int xMax, int yMax)
	{
		Random random = new Random(seed);
		List<Point> points = new List<Point>();
		for(int i=0; i<count; i++)
		{
			points.Add(new Point(
				random.Next(xMin, xMax+1)
				, random.Next(yMin, yMax+1)
			));
		}
		return points;
	}

	public static List<Point> RandomHighXHighY(int seed, int count, int xMin, int yMin, int xMax, int yMax)
	{
		Random random = new Random(seed);
		List<Point> points = new List<Point>();
		for(int i=0; i<count; i++)
		{
			points.Add(new Point(
				TrendHigh(random, xMin, xMax)
				, TrendHigh(random, yMin, yMax)
			));
		}
		return points;
	}
	
	private static int TrendHigh(Random random, int min, int max)
	{
		if(random.Next(0,2) == 1)
		{
			return random.Next(min + (int)((max-min)/2), max+1);
		}
		return random.Next(min, max+1);
	}
}