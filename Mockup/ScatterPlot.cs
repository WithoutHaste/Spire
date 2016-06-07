using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

public class ScatterPlot
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
	private Color darkBrownTransparent = Color.FromArgb(100,44,37,31);
	
	public static void Main(string[] args)
	{
		new ScatterPlot();
	}
	
	public ScatterPlot()
	{
		List<Point> points = RandomData(1000);
		Bitmap image = MakeGraph(points);
		image.Save("sampleScatterPlot.bmp", ImageFormat.Bmp);
	}
	
	private Bitmap MakeGraph(List<Point> data)
	{
		Bitmap image = new Bitmap(graphWidthPx, graphHeightPx);
		using(Graphics g = Graphics.FromImage(image))
		{
			g.Clear(beige);
			DrawPoints(g, data);
			DrawTics(g, data);
		}
		return image;
	}
	
	private void DrawPoints(Graphics g, List<Point> data)
	{
		Brush brush = new SolidBrush(darkBrown);
		foreach(Point datum in data)
		{
			g.FillRectangle(brush, minX+datum.X, padding+datum.Y, 4, 4);
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
	
	//??
	/*
	private Point GaussianRandom(Random r)
	{
		double x1 = r.NextDouble();
		double x2 = r.NextDouble();	
		double y1 = Math.Sqrt( -2 * Math.Log(x1)) * Math.Cos( 2 * Math.PI * x2 );
        double y2 = Math.Sqrt( -2 * Math.Log(x1)) * Math.Sin( 2 * Math.PI * x2 );
		
		y1 = (y1 + 1) * (maxX - minX);
		y2 = (y2 + 1) * (maxY - minY);
		
		return new Point((int)y1, (int)y2);
	}
	*/
}
