using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PSATSim
{
	public class PsatSim
	{
        

        private string WorkingDirectory
        {
            get {return  Directory.GetCurrentDirectory(); }
        }
        private string Input
        {
            get { return WorkingDirectory + "\\input.xml"; }
        }
        private string Output
        {
            get { return WorkingDirectory + "\\out.xml"; }
        }
        private string Arguments
        {
            get { return Input + " " + Output + " -g"; }

        }
        public List<SimulatedConfiguration> Run(List<Configuration> configurations,List<string> selectedTraces)
		{
			Process p = new Process
			{
				StartInfo =
				{
					FileName="psatsim_con.exe",
					Arguments=Arguments,
					WorkingDirectory=@"C:\Program Files (x86)\PSATSim",
					UseShellExecute = true,
					CreateNoWindow = true,
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
				}
			};
		    FileBuilder.WriteXML(configurations, selectedTraces, Input);
            Console.WriteLine("Starting simulator...");
            p.Start();
            while (!p.HasExited) { }
            Thread.Sleep(100);
            Console.WriteLine("Exited with code:{0} ", p.ExitCode);
            p = null;
            return FileBuilder.ReadXML(configurations, Output);
		}
	}
}
