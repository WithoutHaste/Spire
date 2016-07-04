using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

public class Sparkline
{
	private static int graphWidthPx = 1024;
	private static int graphHeightPx = 200;
	private static int padding = 30;
	private int minX = padding;
	private int maxX = graphWidthPx - padding;
	private int minY = padding;
	private int maxY = graphHeightPx - padding;
	private Color beige = Color.FromArgb(255,250,250,231);
	private Color darkBrown = Color.FromArgb(255,44,37,31);
	private Color medBrown = Color.FromArgb(255,116,89,63);
	private Color brown = Color.FromArgb(255,181,149,117);
	private Color lightBrown = Color.FromArgb(255,221,207,193);
	
	public static void Main(string[] args)
	{
		new Sparkline();
	}
	
	public Sparkline()
	{
		List<Point> points = RandomData(1000);
		Bitmap image = MakeGraph(points);
		Bitmap transparentImage = SetTransparency(image);
		transparentImage.Save("sampleSparkline.bmp", ImageFormat.Bmp);
	}
	
	private Bitmap MakeGraph(List<Point> data)
	{
		int count = Math.Min(250, data.Count);
		float xUnit = graphWidthPx / count;
		Bitmap image = new Bitmap(graphWidthPx, graphHeightPx, PixelFormat.Format32bppArgb);
		using(Graphics g = Graphics.FromImage(image))
		{
			g.SmoothingMode = SmoothingMode.AntiAlias;
			//g.Clear(Color.Transparent);
			Pen pen = new Pen(Color.Red, 2F);
			for(int i=1; i<count; i++)
			{
				g.DrawLine(pen, (int)((i-1)*xUnit), data[i-1].Y, (int)(i*xUnit), data[i].Y);
			}
		}
		return image;
	}
	
	private Bitmap SetTransparency(Bitmap originalImage)
	{
		ImageAttributes attr = new ImageAttributes();
//		attr.SetColorKey(originalImage.GetPixel(0, 0), originalImage.GetPixel(0, 0));
		attr.SetColorKey(Color.Black, Color.Black, ColorAdjustType.Bitmap);
		Bitmap newImage = new Bitmap(originalImage.Width, originalImage.Height, PixelFormat.Format32bppArgb);
		using(Graphics graphics = Graphics.FromImage(newImage))
		{
			Rectangle dstRect = new Rectangle(0, 0, originalImage.Width, originalImage.Height);
			graphics.DrawImage(originalImage, dstRect, 0, 0, originalImage.Width, originalImage.Height, GraphicsUnit.Pixel, attr);		
		}
		return newImage;
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
