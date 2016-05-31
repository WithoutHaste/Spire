using System;
using System.Collections.Generic;
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
	
	public static int Below(System.Windows.Forms.Control.ControlCollection controls, int buffer)
	{
		int maxY = 0;
		foreach(System.Windows.Forms.Control control in controls)
		{
			maxY = Math.Max(maxY, control.Top + control.Height);
		}
		return maxY + buffer;
	}

}
