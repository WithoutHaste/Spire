using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

public class Cosmograph
{
	private static int graphWidthPx = 800;
	private static int graphHeightPx = 600;
	private static int padding = 100;
	private int height = graphHeightPx - padding - padding;
	private Color beige = Color.FromArgb(255,250,250,231);
	private Color darkBrown = Color.FromArgb(255,44,37,31);
	private Color medBrown = Color.FromArgb(255,116,89,63);
	private Color brown = Color.FromArgb(255,181,149,117);
	private Color lightBrown = Color.FromArgb(255,221,207,193);
	
	public static void Main(string[] args)
	{
		new Cosmograph();
	}
	
	public Cosmograph()
	{
		List<Cosmo> data = LoadData();
		Bitmap image = MakeGraph(data);
		image.Save("sampleCosmograph.bmp", ImageFormat.Bmp);
	}
	
	private Bitmap MakeGraph(List<Cosmo> data)
	{
		Bitmap image = new Bitmap(graphWidthPx, graphHeightPx);
		using(Graphics g = Graphics.FromImage(image))
		{
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.Clear(beige);

			int leftHeight = 0;
			int rightHeight = 0;
			foreach(Cosmo datum in data)
			{
				leftHeight += datum.From;
				rightHeight += datum.To;
			}
			int leftSpacing = (height - leftHeight) / 4;
			int rightSpacing = (height - rightHeight) / 3;
			
			Dictionary<int, int> itemHeight = SumItemHeights(data);
			Dictionary<int, int> itemY = DetermineItemYs(itemHeight, leftSpacing, rightSpacing);
			
			int leftX = padding;
			int rightX = graphWidthPx - padding;
			int middleX = (int)(leftX + ((rightX-leftX) / 2));
			foreach(Cosmo datum in data)
			{
				Color color = darkBrown;
				switch(datum.To)
				{
					case 6: color = darkBrown; break;
					case 7: color = medBrown; break;
					case 8: color = brown; break;
					case 9: color = lightBrown; break;
				}
				Pen pen = new Pen(color, 2);
				Brush brush = new SolidBrush(color);
				
				Point leftTop = new Point(leftX, itemY[datum.From]);
				Point leftBottom = new Point(leftX, itemY[datum.From]+datum.Size);
				Point rightTop = new Point(rightX, itemY[datum.To]);
				Point rightBottom = new Point(rightX, itemY[datum.To]+datum.Size);

				/* straight lines
				GraphicsPath path = new GraphicsPath();
				path.AddLine(leftTop, rightTop);
				path.AddLine(rightTop, rightBottom);
				path.AddLine(rightBottom, leftBottom);
				path.CloseFigure();
				g.DrawPath(pen, path);
				*/
	
				/* outlines
				g.DrawBeziers(pen, new Point[] {
					leftTop,
					new Point(middleX, leftTop.Y),
					new Point(middleX, rightTop.Y),
					rightTop
				});
				g.DrawBeziers(pen, new Point[] {
					leftBottom,
					new Point(middleX, leftBottom.Y),
					new Point(middleX, rightBottom.Y),
					rightBottom
				});
				g.DrawLine(pen, leftTop, leftBottom);
				g.DrawLine(pen, rightTop, rightBottom);
				*/
				
				GraphicsPath path = new GraphicsPath();
				path.AddBezier(
					leftTop,
					new Point(middleX, leftTop.Y),
					new Point(middleX, rightTop.Y),
					rightTop
				);
				path.AddLine(rightTop, rightBottom);
				path.AddBezier(
					rightBottom,
					new Point(middleX, rightBottom.Y),
					new Point(middleX, leftBottom.Y),
					leftBottom
				);
				path.CloseFigure();
				//g.DrawPath(pen, path);
				g.FillPath(brush, path);
				
				itemY[datum.From] += datum.Size;
				itemY[datum.To] += datum.Size;
			}
		}
		return image;
	}
	
	private Dictionary<int, int> SumItemHeights(List<Cosmo> data)
	{
		Dictionary<int, int> heights = new Dictionary<int, int>();
		foreach(Cosmo datum in data)
		{
			if(!heights.ContainsKey(datum.From))
				heights[datum.From] = 0;
			if(!heights.ContainsKey(datum.To))
				heights[datum.To] = 0;
			heights[datum.From] += datum.Size;
			heights[datum.To] += datum.Size;
		}
		return heights;
	}
	
	private Dictionary<int, int> DetermineItemYs(Dictionary<int, int> heights, int leftSpacing, int rightSpacing)
	{
		int leftY = padding;
		int rightY = padding;
		Dictionary<int, int> ys = new Dictionary<int, int>();
		foreach(KeyValuePair<int, int> height in heights)
		{
			if(height.Key <= 5)
			{
				ys[height.Key] = leftY;
				leftY += height.Value + (leftSpacing/2);
			}
			else
			{
				ys[height.Key] = rightY;
				rightY += height.Value + (rightSpacing/2);
			}
		}
		return ys;
	}
		
	private List<Cosmo> LoadData()
	{
		List<Cosmo> data = new List<Cosmo>();
		data.Add(new Cosmo() { From=1, To=6, Size=22 });
		data.Add(new Cosmo() { From=1, To=8, Size=13 });
		data.Add(new Cosmo() { From=2, To=6, Size=46 });
		data.Add(new Cosmo() { From=2, To=7, Size=22 });
		data.Add(new Cosmo() { From=2, To=9, Size=5 });
		data.Add(new Cosmo() { From=3, To=6, Size=30 });
		data.Add(new Cosmo() { From=4, To=6, Size=14 });
		data.Add(new Cosmo() { From=4, To=7, Size=8 });
		data.Add(new Cosmo() { From=4, To=9, Size=3 });
		data.Add(new Cosmo() { From=5, To=6, Size=7 });
		data.Add(new Cosmo() { From=5, To=7, Size=11 });
		data.Add(new Cosmo() { From=5, To=8, Size=2 });
		data.Add(new Cosmo() { From=5, To=9, Size=31 });
		return data;
	}
	
}

public struct Cosmo
{
	public int From;
	public int To;
	public int Size;
}