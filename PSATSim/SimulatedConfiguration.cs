using System.Collections.Generic;

namespace PSATSim
{
	public class SimulatedConfiguration
	{
		public Dictionary<string, string> Values = new Dictionary<string, string>();
		public Configuration cfg;
		/*public int cycles { get; set; }
		public int instructions { get; set; }
		public int fetches { get; set; }
		public double ipc { get; set; }
		public double energy { get; set; }
		public double power { get; set; }
		*/
		public string Name { get; set; }
	}
}