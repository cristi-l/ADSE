using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSATSim;

namespace GeneticAlgorithms
{
	public class GeneticIndividual : PSATSim.Configuration
	{
		public double Ipc { get; set; }
		public double Power { get; set; }
		public double Fitness { get; set; }
		public double Strength { get; set; }
		public GeneticIndividual(string name) : base(name)
		{
		}
		public GeneticIndividual()
		{

		}

		public GeneticIndividual(Configuration configuration, double ipc, double power)
		{
			this.Name = configuration.Name;
			this.Parameters = configuration.Parameters;
			this.Ipc = ipc;
			this.Power = power;
		}
	}
}
