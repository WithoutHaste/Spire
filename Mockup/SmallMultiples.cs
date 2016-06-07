using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

public class SmallMultiples
{
	private static int graphWidthPx = 800;
	private static int graphHeightPx = 600;
	private static int border = 50;
	private static int internalPadding = 30;
	private static int colCount = 4;
	private static int rowCount = 2;
	private int colWidth = (graphWidthPx - border*2 - internalPadding*(colCount-1)) / colCount;
	private int rowHeight = (graphHeightPx - border*2 - internalPadding*(rowCount-1)) / rowCount;
	private Color beige = Color.FromArgb(255,250,250,231);
	private Color darkBrown = Color.FromArgb(255,44,37,31);
	private Color medBrown = Color.FromArgb(255,116,89,63);
	private Color brown = Color.FromArgb(255,181,149,117);
	private Color lightBrown = Color.FromArgb(255,221,207,193);
	
	private Random random = new Random(4);
	private int yMax = 50;
	
	public static void Main(string[] args)
	{
		new SmallMultiples();
	}
	
	public SmallMultiples()
	{
		List<List<Point>> data = RandomData(7, 24);
		List<Point> summary = SummarizeData(data);
		Bitmap image = InitGraph();
		int row = 0;
		int col = 0;
		for(int i=0; i<data.Count; i++)
		{
			DrawSmallGraph(image, data[i], row, col);
			col++;
			if(col >= colCount)
			{
				col = 0;
				row++;
			}
		}	
		DrawSmallGraph(image, summary, row, col);
		image.Save("sampleSmallMultiples.bmp", ImageFormat.Bmp);
	}
	
	private Bitmap InitGraph()
	{
		Bitmap image = new Bitmap(graphWidthPx, graphHeightPx);
		using(Graphics g = Graphics.FromImage(image))
		{
			g.Clear(beige);
		}
		return image;
	}
	
	private void DrawSmallGraph(Bitmap image, List<Point> data, int row, int col)
	{	
		Point graphTopLeft = new Point(border + (colWidth+internalPadding)*col, border + (rowHeight+internalPadding)*row);
		int barPadding = 0;
		int barWidth = (colWidth - 23*barPadding) / 24;
//		Brush barBrush = new SolidBrush(brown);
		using(Graphics g = Graphics.FromImage(image))
		{
			int x = graphTopLeft.X;
			int y = graphTopLeft.Y + rowHeight;
			Brush barBrush = new LinearGradientBrush(new Point(x,y+150), new Point(x,y-rowHeight), brown, darkBrown);
			for(int i=0; i<data.Count; i++)
			{
				int barHeight = data[i].Y * (rowHeight/yMax);
				g.FillRectangle(barBrush, x, y-barHeight, barWidth, barHeight);
				x += barPadding + barWidth;
			}
		}
	}
	
	private List<Point> SummarizeData(List<List<Point>> data)
	{
		List<Point> summary = new List<Point>();
		for(int i=0; i<data[0].Count; i++)
		{
			int count = 0;
			int total = 0;
			for(int j=0; j<data.Count; j++)
			{
				count++;
				total += data[j][i].Y;
			}
			summary.Add(new Point(data[0][i].X, (int)(total/count)));
		}
		return summary;
	}
	
	private List<List<Point>> RandomData(int countLists, int countPoints)
	{
		List<List<Point>> lists = new List<List<Point>>();
		for(int i=0; i<countLists; i++)
		{
			lists.Add(RandomData(countPoints));
		}
		return lists;
	}
	
	private List<Point> RandomData(int count)
	{	
		List<Point> data = new List<Point>();
		int variance = 6;
		int prevRandom = random.Next(0, yMax+1);
		for(int i=1; i<=count; i++)
		{
			int newRandom = random.Next(prevRandom-variance, prevRandom+variance);
			if(newRandom < 0) newRandom = 0;
			if(newRandom > yMax) newRandom = yMax;
			data.Add(new Point(i, newRandom));
			prevRandom = newRandom;
		}
		return data;
	}
	
}
