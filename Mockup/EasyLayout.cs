using System;
using System.Drawing;

static class EasyLayout 
{
	public static int LeftOf(System.Windows.Forms.Control control)
	{
		return control.Left + control.Width;
	}

	public static int LeftOf(System.Windows.Forms.Control control, int buffer)
	{
		return control.Left + control.Width + buffer;
	}
	
	public static int Below(System.Windows.Forms.Control control)
	{
		return control.Top + control.Height;
	}

	public static int Below(System.Windows.Forms.Control control, int buffer)
	{
		return control.Top + control.Height + buffer;
	}

}
