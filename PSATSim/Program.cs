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
					Arguments=@" D:\Calc\SOAC\Project\input.xml D:\Calc\SOAC\Project\out.xml -g",
					WorkingDirectory=@"C:\Program Files (x86)\PSATSim",
					UseShellExecute = true,
					CreateNoWindow = true
				}
			};

			
			List<Configuration> c = new List<Configuration>();
			ConfigurationGenerator cg = new ConfigurationGenerator();
			for(int i=0;i<8;i++)
				c.Add(cg.RandomConfig());
			FileBuilder.WriteXML(c, new List<string> { "compress.tra" }, "..\\..\\..\\input.xml");
			p.Start();
			Thread.Sleep(10000);
			if (!p.HasExited)
				p.Kill();
		}
	}
}
