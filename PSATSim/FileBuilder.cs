using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace PSATSim
{
	public class FileBuilder
	{
		public static void WriteXML(List<Configuration> configurations, List<string> traces, string fileName)
		{

			var doc = new XmlDocument();
			XmlElement mainNode = doc.CreateElement("psatsim");
			mainNode = (XmlElement)doc.AppendChild(mainNode);

			foreach (var trace in traces)
			{
				foreach (var configuration in configurations)
				{
					XmlElement configNode = doc.CreateElement("config");
					configNode.SetAttribute("name", trace + "_config_" + configuration.Name);
                    configNode.SetAttribute("number", 0.ToString("D"));
					mainNode.AppendChild(configNode);

					XmlElement general = doc.CreateElement("general");
					general.SetAttribute("superscalar", configuration.GetValue("superscalar"));
					general.SetAttribute("rename", configuration.GetValue("rename"));
					general.SetAttribute("reorder", configuration.GetValue("reorder"));
					general.SetAttribute("rsb_architecture", configuration.GetValue("rsb_architecture"));
					general.SetAttribute("rs_per_rsb", configuration.GetValue("rs_per_rsb"));
					general.SetAttribute("speculative", configuration.GetValue("speculative"));
					general.SetAttribute("speculation_accuracy", configuration.GetValue("speculation_accuracy"));
					general.SetAttribute("separate_dispatch", configuration.GetValue("separate_dispatch"));
					general.SetAttribute("seed", "");
					general.SetAttribute("trace", trace);
					general.SetAttribute("output", "");
					general.SetAttribute("vdd", configuration.GetValue("vdd"));
					general.SetAttribute("frequency", configuration.GetValue("frequency"));

					configNode.AppendChild(general);

					XmlElement execution = doc.CreateElement("execution");
					execution.SetAttribute("architecture", "standard");
					execution.SetAttribute("integer", configuration.GetValue("integer"));
					execution.SetAttribute("floating", configuration.GetValue("floating"));
					execution.SetAttribute("branch", configuration.GetValue("branch"));
					execution.SetAttribute("memory", configuration.GetValue("memory"));

					configNode.AppendChild(execution);

					XmlElement memory = doc.CreateElement("memory");
					memory.SetAttribute("architecture", "l2");

					configNode.AppendChild(memory);

					XmlElement l1_code = doc.CreateElement("l1_code");
					l1_code.SetAttribute("hitrate","0.990");
					l1_code.SetAttribute("latency", "1");

					memory.AppendChild(l1_code);

					XmlElement l1_data = doc.CreateElement("l1_data");
					l1_data.SetAttribute("hitrate", "0.970");
					l1_data.SetAttribute("latency", "1");

					memory.AppendChild(l1_data);

					XmlElement l2 = doc.CreateElement("l2");
					l2.SetAttribute("hitrate", "0.990");
					l2.SetAttribute("latency", "3");

					memory.AppendChild(l2);

					XmlElement system = doc.CreateElement("system");
					system.SetAttribute("latency", "20");

					memory.AppendChild(system);


				}
			}
			doc.Save(fileName);
		}
		public static List<SimulatedConfiguration> ReadXML(List<Configuration> configurations,  string fileName)
		{
			List<SimulatedConfiguration> results = new List<SimulatedConfiguration>();
			foreach (XElement level1 in XElement.Load(fileName).Elements())
			{
				Console.WriteLine(level1.FirstAttribute);
				SimulatedConfiguration sc = new SimulatedConfiguration();
				sc.Name = level1.FirstAttribute.Value.ToString();
                var name = sc.Name.Substring(sc.Name.LastIndexOf('_') + 1);
                sc.cfg = configurations.First(item => item.Name.Equals(name));
				foreach (var level2 in level1.Elements())
				{
					foreach (var attribute in level2.Attributes())
					{
						sc.Values.Add(attribute.Name.ToString(), attribute.Value.ToString());
					}
					results.Add(sc);
				}
			}
			return results;
		}
	}
}
