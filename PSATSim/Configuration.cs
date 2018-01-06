using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSATSim
{
	public class Configuration:ICloneable
	{
		public string Name { get; set; }
		public Dictionary<string, string> Parameters { get; set; }
		public Configuration() { }
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
			if (!Parameters.ContainsKey(attribute))
				Parameters.Add(attribute, value);
			else
				Parameters[attribute] = value;
		}

        public object Clone()
        {
           return this.MemberwiseClone();
        }
    }
}
