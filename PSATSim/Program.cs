using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace PSATSim
{
	public class Program
	{
		static void Main(string[] args)
		{
			Process p = new Process
			{
				StartInfo =
				{
					FileName="psatsim_con.exe",
					Arguments=" config.xml first.xml -g",
					WorkingDirectory=@"C:\Program Files (x86)\PSATSim",
					UseShellExecute = true,
					CreateNoWindow = true
				}
			};

			p.Start();
			Thread.Sleep(100000);
			if (!p.HasExited)
				p.Kill();
			Configuration c = new Configuration();
		}
	}
}
