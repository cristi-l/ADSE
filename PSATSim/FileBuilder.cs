using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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
					configNode.SetAttribute("name", trace + "_sim_" + configuration.Name);
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
					general.SetAttribute("seed", configuration.GetValue("seed"));
					general.SetAttribute("trace", trace);
					general.SetAttribute("output", "");
					general.SetAttribute("vdd", configuration.GetValue("vdd"));
					general.SetAttribute("frequency", configuration.GetValue("frequency"));

					configNode.AppendChild(general);

					XmlElement execution = doc.CreateElement("execution");
					execution.SetAttribute("architecture", configuration.GetValue("architecture"));
					execution.SetAttribute("integer", configuration.GetValue("integer"));
					execution.SetAttribute("floating", configuration.GetValue("floating"));
					execution.SetAttribute("branch", configuration.GetValue("branch"));
					execution.SetAttribute("memory", configuration.GetValue("memory"));

					configNode.AppendChild(execution);

					XmlElement memory = doc.CreateElement("memory");
					memory.SetAttribute("architecture", configuration.GetValue("mem_architecture"));

					configNode.AppendChild(memory);

					XmlElement l1_code = doc.CreateElement("l1_code");
					l1_code.SetAttribute("hitrate", configuration.GetValue("l1Code_hitrate"));
					l1_code.SetAttribute("latency", configuration.GetValue("l1_latency"));

					memory.AppendChild(l1_code);

					XmlElement l1_data = doc.CreateElement("l1_data");
					l1_data.SetAttribute("hitrate", configuration.GetValue("l1Data_hitrate"));
					l1_data.SetAttribute("latency", configuration.GetValue("l1_latency"));

					memory.AppendChild(l1_data);

					XmlElement l2 = doc.CreateElement("l2");
					l2.SetAttribute("hitrate", configuration.GetValue("l2_hitrate"));
					l2.SetAttribute("latency", configuration.GetValue("l2_latency"));

					memory.AppendChild(l2);

					XmlElement system = doc.CreateElement("system");
					system.SetAttribute("latency", configuration.GetValue("sys_latency"));

					memory.AppendChild(system);


				}
			}
		}
	}
}
