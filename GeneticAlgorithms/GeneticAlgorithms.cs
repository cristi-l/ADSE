using PSATSim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithms
{
	public class GeneticAlgorithms
	{
		private  int id=20;
		private int individualID { get { return id++; }  }
		int populationSize=3;
		List<Configuration> population=new List<Configuration>();
		static Random r = new Random();
		public List<Configuration> Crossover(Configuration parent1, Configuration parent2)
		{
			Configuration child1 = new Configuration(individualID.ToString());
			Configuration child2 = new Configuration(individualID.ToString());
			int cutPoint = r.Next(1, parent1.Parameters.Count - 1);
			for(int i = 0; i < cutPoint; i++)
			{
				child1.Parameters.Add(parent1.Parameters.ElementAt(i).Key, parent1.Parameters.ElementAt(i).Value);
				child2.Parameters.Add(parent2.Parameters.ElementAt(i).Key, parent2.Parameters.ElementAt(i).Value);
			}
			for(int i = cutPoint; i < parent1.Parameters.Count; i++)
			{
				child1.Parameters.Add(parent2.Parameters.ElementAt(i).Key, parent2.Parameters.ElementAt(i).Value);
				child2.Parameters.Add(parent1.Parameters.ElementAt(i).Key, parent1.Parameters.ElementAt(i).Value);
			}
			return new List<Configuration> { parent1, parent2 };
		}
		public void InitRandomPopulation(List<string> selectedTraces)
		{
			ConfigurationGenerator cg = new ConfigurationGenerator();
			for (int i = 0; i < populationSize; i++)
			{
				population.Add(cg.RandomConfig(individualID.ToString()));
			}
			PsatSim sim = new PsatSim();
			var results = sim.Run(population,selectedTraces);
			for(int i = 0; i < population.Count;i++)
			{
				population[i] = new GeneticIndividual(population[i], 0.0, 0.0);
				for(int j = 0; j < selectedTraces.Count; j++)
				{
					((GeneticIndividual)population[i]).Ipc += results[i + population.Count * j].ipc;
					((GeneticIndividual)population[i]).Power += results[i + population.Count* j].power;
				}
				if (selectedTraces.Count > 1)
				{
					((GeneticIndividual)population[i]).Ipc /= selectedTraces.Count;
					((GeneticIndividual)population[i]).Power /= selectedTraces.Count;
				}
			}
			var v = Crossover(population[1], population[2]);
		}
		public Configuration Mutate(Configuration child)
		{
			return child;
		}
		public void NSGA2()
		{

		}












		public void SPEA2()
		{

		}
	}
}
