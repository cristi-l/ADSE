using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSATSim
{
	public class PSATSim
	{
		public List<SimulatedConfiguration> Run(List<Configuration> configurations,List<string> selectedTraces)
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
			FileBuilder.WriteXML(configurations, selectedTraces, @"D:\Calc\SOAC\Project\input.xml");
			p.Start();
			p.WaitForExit();
			return FileBuilder.ReadXML(configurations, @"D:\Calc\SOAC\Project\out.xml");
		}
	}
}
