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
		private int id = 0;
		private double mutationRate = 0.3;
		private int individualID { get { return id++; } }
		int populationSize = 10;
		public List<Configuration> population = new List<Configuration>();

		List<GeneticIndividual> firstFront = new List<GeneticIndividual>();
		List<GeneticIndividual> bestIndividuals = new List<GeneticIndividual>();
		public List<GeneticIndividual> archive = new List<GeneticIndividual>();
		List<SimulatedConfiguration> results = new List<SimulatedConfiguration>();
		List<string> selectedTraces = new List<string>();

		PsatSim sim = new PsatSim();
		ConfigurationGenerator cg = new ConfigurationGenerator();
		static Random r = new Random();
		public List<Configuration> Crossover(Configuration parent1, Configuration parent2)
		{
			Configuration child1 = new Configuration(individualID.ToString());
			Configuration child2 = new Configuration(individualID.ToString());
			int cutPoint = r.Next(1, parent1.Parameters.Count - 1);
			for (int i = 0; i < cutPoint; i++)
			{
				child1.Parameters.Add(parent1.Parameters.ElementAt(i).Key, parent1.Parameters.ElementAt(i).Value);
				child2.Parameters.Add(parent2.Parameters.ElementAt(i).Key, parent2.Parameters.ElementAt(i).Value);
			}
			for (int i = cutPoint; i < parent1.Parameters.Count; i++)
			{
				child1.Parameters.Add(parent2.Parameters.ElementAt(i).Key, parent2.Parameters.ElementAt(i).Value);
				child2.Parameters.Add(parent1.Parameters.ElementAt(i).Key, parent1.Parameters.ElementAt(i).Value);
			}
			return new List<Configuration> { parent1, parent2 };
		}
		public void InitRandomPopulation(List<string> selectedTraces)
		{
			this.selectedTraces = selectedTraces;

			for (int i = 0; i < populationSize; i++)
			{
				population.Add(cg.RandomConfig(individualID.ToString()));
			}

			CalculateObjectives();

			var v = Crossover(population[1], population[2]);
			var m = Mutate(v[0]);
		}
		public Configuration Mutate(Configuration child)
		{
			ConfigurationGenerator cg = new ConfigurationGenerator();
			for (int i = 0; i < child.Parameters.Count; i++)
			{
				if (r.NextDouble() < mutationRate)
				{
					cg.RanomizeParameter(i, child);
				}
			}
			return child;
		}


		public void NSGA2(List<GeneticIndividual> population)
		{
			foreach (var individual in population)
			{
				individual.FrontNumber = 0;
				foreach (var otherIndividual in population)
				{
					if (individual != otherIndividual)
					{
						if ((otherIndividual.Ipc < individual.Ipc && otherIndividual.Energy <= individual.Energy) || (otherIndividual.Ipc <= individual.Ipc && otherIndividual.Energy < individual.Energy))
						{
							//other individual is better
							individual.FrontNumber++;
						}
					}
				}

			}
		}











		//http://www.cleveralgorithms.com/nature-inspired/evolution/spea.html
		public void SPEA2()
		{			
				SPEA2Fitness();
				archive = EnvironmentalSelection(population, archive, populationSize / 2);

				List<GeneticIndividual> selected = new List<GeneticIndividual>();
				while (selected.Count < populationSize / 2)
					selected.Add(BinaryTournament(archive));
				population.Clear();
				population.AddRange( Reproduce(selected));
				CalculateObjectives();
			
		}

		private List<Configuration> Reproduce(List<GeneticIndividual> selected)
		{
			List<Configuration> children = new List<Configuration>();
			/*for(int i = 0; i < selected.Count-1; i++)
			{
				children.AddRange(Crossover(selected[i], selected[i + 1]));
			}*/
			while (children.Count < populationSize)
			{
				int i = r.Next(selected.Count);
				int j = r.Next(selected.Count);
				if (i != j)
					children.AddRange(Crossover(selected[i], selected[j]));
			}
			for (int i = 0; i < children.Count; i++)
			{
				children[i] = Mutate(children[i]);
			}
			return children;
		}

		private GeneticIndividual BinaryTournament(List<GeneticIndividual> archive)
		{
			int i = r.Next(archive.Count);
			int j = r.Next(archive.Count);
			while (i == j)
				j = r.Next(archive.Count);
			return (archive[i].Fitness > archive[j].Fitness ? archive[j] : archive[i]);
		}

		private List<GeneticIndividual> EnvironmentalSelection(List<Configuration> population, List<GeneticIndividual> archive, int archiveSize)
		{
			List<GeneticIndividual> newArchive = new List<GeneticIndividual>();
			List<Configuration> union = population.Concat(archive).OrderBy(c => ((GeneticIndividual)c).Fitness).ToList();
			foreach (var item in union)
			{
				//adaug frontol ne-dominat
				if (((GeneticIndividual)item).Fitness < 1)
				{
					newArchive.Add((GeneticIndividual)item);
				}
			}

			if (newArchive.Count < archiveSize)
			{
				foreach (var item in union)
				{
					if (((GeneticIndividual)item).Fitness >= 1.0)
					{
						newArchive.Add((GeneticIndividual)item);
					}
					if (newArchive.Count >= archiveSize)
						break;
				}
				return newArchive.Take(archiveSize).ToList();
			}
			else
			{
				while (newArchive.Count > archiveSize)
				{
					int k = (int)Math.Sqrt(newArchive.Count);
					foreach (var p1 in newArchive)
					{
						foreach (var p2 in newArchive)
						{
							p2.distance = EuclideanDistance(p1, p2);
						}
						var list = newArchive.OrderBy(c => c.distance).ToArray();
						p1.Density = list[k].distance;
					}
					newArchive.RemoveAt(0);
				}
			}

			return newArchive;
		}

		private void SPEA2Fitness()
		{
			List<Configuration> union = population.Concat(archive).ToList();
			calculateRawFitness(union);
			foreach (var individual in union)
			{
				((GeneticIndividual)individual).Density = Calculatedensity((GeneticIndividual)individual, union);
				((GeneticIndividual)individual).Fitness = ((GeneticIndividual)individual).R + ((GeneticIndividual)individual).Density;
			}
		}

		public void calculateRawFitness(List<Configuration> union)
		{

			foreach (var individual in union)
			{

				foreach (var other in union)
				{
					if (individual != other)
						if ((((GeneticIndividual)other).Ipc > ((GeneticIndividual)individual).Ipc &&
								((GeneticIndividual)other).Energy >= ((GeneticIndividual)individual).Energy) ||
								(((GeneticIndividual)other).Ipc >= ((GeneticIndividual)individual).Ipc &&
								((GeneticIndividual)other).Energy > ((GeneticIndividual)individual).Energy))
						{//individual domina, are .Strength indivizi pe care ii domina
							((GeneticIndividual)individual).Strength++;
						}
				}

			}
			foreach (var individual in union)
			{
				foreach (var other in union)
				{
					if (individual != other)
						if ((((GeneticIndividual)other).Ipc < ((GeneticIndividual)individual).Ipc &&
								((GeneticIndividual)other).Energy <= ((GeneticIndividual)individual).Energy) ||
								(((GeneticIndividual)other).Ipc <= ((GeneticIndividual)individual).Ipc &&
								((GeneticIndividual)other).Energy < ((GeneticIndividual)individual).Energy))
						{//individual este dominat, este dominat de .Fitness indivizi
							((GeneticIndividual)individual).R += ((GeneticIndividual)other).Strength;
						}
				}
			}
		}
		public double Calculatedensity(GeneticIndividual individual, List<Configuration> pop)
		{
			foreach (var other in pop)
			{
				((GeneticIndividual)other).distance = EuclideanDistance(((GeneticIndividual)individual), (GeneticIndividual)other);
			}
			var sorted = pop.OrderBy(c => (((GeneticIndividual)c).distance)).ToArray();
			int k = (int)Math.Sqrt(pop.Count);
			return 1.0 / (((GeneticIndividual)sorted[k]).distance + 2.0);
		}

		private double EuclideanDistance(GeneticIndividual individual, GeneticIndividual other)
		{
			return Math.Sqrt(Math.Pow(individual.Ipc - other.Ipc, 2) + Math.Pow(individual.Energy - other.Energy, 2));
		}

		public void CalculateObjectives()
		{
			results = sim.Run(population, selectedTraces);
			for (int i = 0; i < population.Count; i++)
			{
				population[i] = new GeneticIndividual(population[i], 0.0, 0.0, 0.0);

				for (int j = 0; j < selectedTraces.Count; j++)
				{
					((GeneticIndividual)population[i]).Ipc += results[i + population.Count * j].ipc;
					((GeneticIndividual)population[i]).Power += results[i + population.Count * j].power;
					((GeneticIndividual)population[i]).Energy += results[i + population.Count * j].energy;

				}
				if (selectedTraces.Count > 1)
				{
					((GeneticIndividual)population[i]).Ipc /= selectedTraces.Count;
					((GeneticIndividual)population[i]).Power /= selectedTraces.Count;
					((GeneticIndividual)population[i]).Energy /= selectedTraces.Count;
				}
			}
		}
	}
}
