using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

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