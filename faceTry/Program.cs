﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace faceTry
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			//Application.Run(new Form1());
			Start s = new Start();
			Prog.StartRun();
			while (true)
			{
				// run
				System.Threading.Thread.Sleep(3600000);
			}
		}
	}
}
