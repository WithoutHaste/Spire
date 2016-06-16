using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

public class DoubleBufferedPanel : Panel
{
	public DoubleBufferedPanel()
	{
		// Set the value of the double-buffering style bits to true.
		this.SetStyle(ControlStyles.DoubleBuffer |
		ControlStyles.UserPaint |
		ControlStyles.AllPaintingInWmPaint,
		true);

		this.UpdateStyles();
	}
}