using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSATSim
{
	public class Configuration
	{
		public string Name { get; set; }
		public Dictionary<string, string> Parameters { get; set; }
		public string GetValue(string attribute)
		{
			string s = "";
			if (Parameters.TryGetValue(attribute, out s))
				return s;
			else
				throw new ArgumentException("key not found");
		}
	}
}
