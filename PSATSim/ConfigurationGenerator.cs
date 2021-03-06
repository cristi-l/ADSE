﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSATSim
{
	public class ConfigurationGenerator
	{
		public static readonly Random random = new Random();
		public static List<string> rsb_architecture = new List<string>() { "centralized", "hybrid", "distributed" };
		public Configuration RandomConfig(string id)
		{
			Configuration c = new Configuration(id);


			var superscalar = random.Next(1, 16);
			c.SetValue("superscalar", superscalar.ToString());
			c.SetValue("rename", random.Next(superscalar + 1, 512).ToString());
			c.SetValue("reorder", random.Next(superscalar + 1, 512).ToString());

			c.SetValue("rsb_architecture", rsb_architecture[random.Next(rsb_architecture.Count)]);

			c.SetValue("rs_per_rsb", random.Next(1, 8).ToString());
			c.SetValue("speculative", (random.Next() % 2 == 0) ? "true" : "false");
			c.SetValue("speculation_accuracy", (0.9 + 0.1 * random.NextDouble()).ToString("F3"));
			c.SetValue("separate_dispatch", (random.Next() % 2 == 0) ? "true" : "false");
			c.SetValue("vdd", ((1.8 + 1.5 * random.NextDouble()).ToString("F1")));
			c.SetValue("frequency", ((int)(1.8 + 1.5 * random.NextDouble())).ToString());

			c.SetValue("integer", random.Next(1, 8).ToString());
			c.SetValue("floating", random.Next(1, 8).ToString());
			c.SetValue("branch", random.Next(1, 8).ToString());
			c.SetValue("memory", random.Next(1, 8).ToString());
			//caracteristici constante => le-am mutat in functia care genereaza fisierul
			/*c.SetValue("architecture", "standard");

			c.SetValue("mem_architecture", "l2");

			c.SetValue("l1Code_hitrate", "0.990");
			c.SetValue("l1Data_hitrate", "0.970");
			c.SetValue("l1_latency","1");

			c.SetValue("l2_hitrate", "0.990");
			c.SetValue("l2_latency", "3");

			c.SetValue("sys_latency", "20");*/
			Console.WriteLine(c.Parameters.ElementAt(3));
			return c;
		}
		public Configuration RandomizeParameter(int parameterIndex, Configuration c)
		{
			return RandomizeParameter(c.Parameters.ElementAt(parameterIndex).Key, c);
		}
		private Configuration RandomizeParameter(string name, Configuration c)
		{
			switch (name)
			{
				case "superscalar":
				{
					var superscalar = random.Next(1, 16);
					c.SetValue("superscalar", superscalar.ToString());
				}
				break;
				case "rename":
				{
					c.SetValue("rename", random.Next(int.Parse(c.GetValue("superscalar")) + 1, 512).ToString());
				}
				break;
				case "reorder":
				{
					c.SetValue("reorder", random.Next(int.Parse(c.GetValue("superscalar")) + 1, 512).ToString());
				}
				break;
				case "rsb_architecture":
				{
					c.SetValue("rsb_architecture", rsb_architecture[random.Next(rsb_architecture.Count)]);
				}
				break;
				case "rs_per_rsb":
				{
					c.SetValue("rs_per_rsb", random.Next(1, 8).ToString());
				}
				break;
				case "speculative":
				{
					c.SetValue("speculative", (random.Next() % 2 == 0) ? "true" : "false");
				}
				break;
				case "speculation_accuracy":
				{
					c.SetValue("speculation_accuracy", (0.9 + 0.1 * random.NextDouble()).ToString("F3"));
				}
				break;
				case "separate_dispatch":
				{
					c.SetValue("separate_dispatch", (random.Next() % 2 == 0) ? "true" : "false");
				}
				break;
				case "vdd":
				{
					c.SetValue("vdd", ((1.8 + 1.5 * random.NextDouble()).ToString("F1")));
				}
				break;
				case "frequency":
				{
					c.SetValue("frequency", ((int)(1.8 + 1.5 * random.NextDouble())).ToString());
				}
				break;
				case "integer":
				{
					c.SetValue("integer", random.Next(1, 8).ToString());
				}
				break;
				case "floating":
				{
					c.SetValue("floating", random.Next(1, 8).ToString());
				}
				break;
				case "branch":
				{
					c.SetValue("branch", random.Next(1, 8).ToString());
				}
				break;
				case "memory":
				{
					c.SetValue("memory", random.Next(1, 8).ToString());
				}

				break;
				default:
				break;
			}
			return c;
		}
	}
}
