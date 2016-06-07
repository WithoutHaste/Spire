using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

public class BoxPlot
{
	private static int graphWidthPx = 800;
	private static int graphHeightPx = 600;
	private static int padding = 30;
	private static int axisThickness = 50;
	private int minX = padding + axisThickness;
	private int maxX = graphWidthPx - padding;
	private int minY = padding + axisThickness;
	private int maxY = graphHeightPx - padding;
	private Color beige = Color.FromArgb(255,250,250,231);
	private Color darkBrown = Color.FromArgb(255,44,37,31);
	private Color medBrown = Color.FromArgb(255,116,89,63);
	private Color brown = Color.FromArgb(255,181,149,117);
	private Color lightBrown = Color.FromArgb(255,221,207,193);
	
	public static void Main(string[] args)
	{
		new BoxPlot();
	}
	
	public BoxPlot()
	{
		List<Point> points = RandomData(1000);
		List<Quartile> summary = SummarizeData(points);
		Bitmap image = MakeGraph(summary);
		image.Save("sampleBoxPlot.bmp", ImageFormat.Bmp);
	}
	
	private Bitmap MakeGraph(List<Quartile> data)
	{
		Bitmap image = new Bitmap(graphWidthPx, graphHeightPx);
		using(Graphics g = Graphics.FromImage(image))
		{
			g.Clear(beige);
			DrawBoxes(g, data);
			//DrawTics(g, data);
		}
		return image;
	}
	
	private void DrawBoxes(Graphics g, List<Quartile> data)
	{	
		Pen medianPen = new Pen(darkBrown, 4);
		Brush quartileBrush = new SolidBrush(brown);
		Brush rangeBrush = new SolidBrush(lightBrown);
		foreach(Quartile datum in data)
		{
			g.FillRectangle(rangeBrush, minX+datum.Min, padding+datum.GetMinY(), datum.Max-datum.Min, datum.GetMaxY()-datum.GetMinY());
			g.FillRectangle(quartileBrush, minX+datum.Min, padding+datum.GetLowerQuartile(), datum.Max-datum.Min, datum.GetUpperQuartile()-datum.GetLowerQuartile());
			g.DrawLine(medianPen, minX+datum.Min, padding+datum.GetMedian(), minX+datum.Max, padding+datum.GetMedian());
		}
	}
	
	private void DrawTics(Graphics g, List<Point> data)
	{
		Pen pen = new Pen(darkBrown, 0.75F);
		foreach(Point datum in data)
		{
			g.DrawLine(pen, minX+datum.X, graphHeightPx-minY+5, minX+datum.X, graphHeightPx-minY+20);
			g.DrawLine(pen, minX-5, padding+datum.Y, minX-20, padding+datum.Y);
		}
	}
	
	private List<Quartile> SummarizeData(List<Point> data)
	{
		int minX = 1000;
		int maxX = 0;
		foreach(Point datum in data)
		{
			minX = Math.Min(minX, datum.X);
			maxX = Math.Max(maxX, datum.X);
		}
		int interval = (maxX-minX) / 30;
		
		List<Quartile> quartiles = new List<Quartile>();
		for(int i=minX; i<maxX; i+=interval)
		{
			Quartile quartile = new Quartile(i, i+interval-1);
			foreach(Point datum in data)
			{
				if(datum.X < quartile.Min || datum.X > quartile.Max) continue;
				quartile.Ys.Add(datum.Y);
			}
			quartiles.Add(quartile);
		}
		
		//massauge data
		for(int i=0; i<5; i++)
		{
			quartiles.RemoveAt(quartiles.Count-1);
		}
		
		return quartiles;
	}
	
	private List<double> LoadGaussianNumbers()
	{
		List<double> numbers = new List<double>();
		using(StreamReader reader = new StreamReader("gaussianDistribution2.csv"))
		{
			string line;
			while((line = reader.ReadLine()) != null)
			{
				line = line.Trim();
				numbers.Add(Double.Parse(line));
			}
		}
		return numbers;
	}
	
	private List<Point> RandomData(int count)
	{	
		List<double> gaussianNumbers = LoadGaussianNumbers();
		Random random = new Random(1);
		List<Point> data = new List<Point>();
		int xRange = maxX - minX;
		for(int i=0; i<gaussianNumbers.Count/2; i++)
		{
			int x = (int)(gaussianNumbers[i] * xRange);
			if(x < 0) continue;
			if(x > xRange) continue;
			int y = random.Next(0, maxY-minY);
			data.Add(new Point(x, y));
		}
		return data;
	}
	
}

public struct Quartile
{
	public int Min;
	public int Max;
	public List<int> Ys;
	
	private bool calculated;
	private int median;
	private int lowerQuartile;
	private int upperQuartile;
	
	public Quartile(int min, int max)
	{
		Min = min;
		Max = max;
		Ys = new List<int>();
		calculated = false;
		median = 0;
		lowerQuartile = 0;
		upperQuartile = 0;
	}
		
	private void Calculate()
	{
		if(calculated) return;
		
		Ys.Sort();
		median = Ys[(int)(Ys.Count/2)];
		lowerQuartile = Ys[(int)(Ys.Count/4)];
		upperQuartile = Ys[(int)(Ys.Count*3/4)];
		
		calculated = true;
	}
	
	public int GetMedian()
	{
		Calculate();
		return median;
	}
	
	public int GetLowerQuartile()
	{
		Calculate();
		return lowerQuartile;
	}
	
	public int GetUpperQuartile()
	{
		Calculate();
		return upperQuartile;
	}
	
	public int GetMinY()
	{
		Calculate();
		return Ys[0];
	}
	
	public int GetMaxY()
	{
		Calculate();
		return Ys[Ys.Count-1];
	}
}