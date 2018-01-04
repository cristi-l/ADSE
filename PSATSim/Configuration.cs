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
		public Configuration(string name)
		{
			Parameters = new Dictionary<string, string>();
			Name = name;
		}
		public string GetValue(string attribute)
		{
			string s = "";
			if (Parameters.TryGetValue(attribute, out s))
				return s;
			else
				throw new ArgumentException("key not found");
		}
		public void SetValue(string attribute, string value)
		{
			Parameters.Add(attribute, value);
		}
	}
}
