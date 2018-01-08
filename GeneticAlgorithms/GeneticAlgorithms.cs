using PSATSim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace GeneticAlgorithms
{
    public class GeneticAlgorithms
    {
        private int id = 0;
        private double mutationRate = 0.4;

        private PsatSim sim;
        private Random random;
        public SimulationResults simulationResults;
        private List<string> selectedTraces;
        private int individualID { get { return id++; } }
        int populationSize = 12;

        public List<Configuration> population = new List<Configuration>();

        public List<GeneticIndividual> firstFront = new List<GeneticIndividual>();
        List<GeneticIndividual> bestIndividuals = new List<GeneticIndividual>();
        public List<GeneticIndividual> archive = new List<GeneticIndividual>();
        List<SimulatedConfiguration> results = new List<SimulatedConfiguration>();

        public List<GeneticIndividual> nsgaPopulation = new List<GeneticIndividual>();
        List<Configuration> allConfigurations = new List<Configuration>();

        public GeneticAlgorithms(List<string> selectedTraces,int populationSize)
        {
            this.selectedTraces = selectedTraces;
            sim = new PsatSim();
            random = new Random();
            simulationResults = new SimulationResults();
			this.populationSize = populationSize;
        }
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
            return new List<Configuration> { child1, child2 };
        }
        public bool InitRandomPopulation(List<string> selectedTraces)
        {
            ConfigurationGenerator cg = new ConfigurationGenerator();
            for (int i = 0; i < populationSize; i++)
            {
                population.Add(cg.RandomConfig(individualID.ToString()));

            }
            allConfigurations.AddRange(population);

            var results = sim.Run(population, selectedTraces);
            for (int i = 0; i < population.Count; i++)
            {
                population[i] = new GeneticIndividual(population[i], 0.0, 0.0, 0.0);

                for (int j = 0; j < selectedTraces.Count; j++)
                {
                    ((GeneticIndividual)population[i]).Ipc += 1.0/results[i + population.Count * j].ipc;
                    ((GeneticIndividual)population[i]).Power += results[i + population.Count * j].power;
                    ((GeneticIndividual)population[i]).Energy += results[i + population.Count * j].energy;

                }
                if (selectedTraces.Count > 1)
                {
                    ((GeneticIndividual)population[i]).Ipc /= selectedTraces.Count;
                    ((GeneticIndividual)population[i]).Power /= selectedTraces.Count;
                    ((GeneticIndividual)population[i]).Energy /= selectedTraces.Count;
                }
                nsgaPopulation.Add(new GeneticIndividual(population[i], ((GeneticIndividual)population[i]).Ipc, ((GeneticIndividual)population[i]).Power, ((GeneticIndividual)population[i]).Energy));
            }
			return true;
        }

        private void StoreResults()
        {
            simulationResults.bestIndividuals.AddRange(bestIndividuals);
            simulationResults.firstFront.AddRange(firstFront);
            simulationResults.allConfigurations.AddRange(allConfigurations);
            simulationResults.nsgaPopulation.AddRange(nsgaPopulation);
        }

        public Configuration Mutate(Configuration child)
        {

            ConfigurationGenerator cg = new ConfigurationGenerator();
            for (int i = 0; i < child.Parameters.Count; i++)
            {
                if (r.NextDouble() < mutationRate)
                {
                    cg.RandomizeParameter(i, child);
                }
            }
            return child;
        }

        public bool NSGA2(List<GeneticIndividual> population, int currentGeneration, ProgressBar progressBar)
        {
            Clear();
            FindBestIndividuals(population);
            KeepNondominatedIndividuals();
            DoMutationAndSimulation();
            DoCrossoverForBestIndividuals();
            StoreResults();
			return true;
        }

        public void FindFirstFront(List<GeneticIndividual> population)
        {
            firstFront.Clear();
            foreach (var individual in population)
            {
                individual.FrontNumber = 0;
                foreach (var otherIndividual in population)
                {
                    if (individual != otherIndividual)
                    {
                        if (((1.0 / otherIndividual.Ipc < (1.0 / individual.Ipc) && otherIndividual.Energy <= individual.Energy) || ((1.0 / otherIndividual.Ipc) <= (1.0 / individual.Ipc) && otherIndividual.Energy < individual.Energy)))
                        {
                            individual.FrontNumber++;
                        }
                    }
                }
                //Nondominated
                if (individual.FrontNumber == 0)
                {
                    firstFront.Add(individual);
                }
            }
        }

        public void FindBestIndividuals(List<GeneticIndividual> population)
        {
            foreach (var individual in population)
            {
                individual.FrontNumber = 0;
                foreach (var otherIndividual in population)
                {
                    if (individual != otherIndividual)
                    {
                        if (((1.0 / otherIndividual.Ipc < (1.0 / individual.Ipc) && otherIndividual.Energy <= individual.Energy) || ((1.0 / otherIndividual.Ipc) <= (1.0 / individual.Ipc) && otherIndividual.Energy < individual.Energy)))
                        {
                            //other individual is better
                            individual.FrontNumber++;
                        }
                    }
                }
                //Nondominated
                if (individual.FrontNumber == 0)
                {
                    firstFront.Add(individual);
                    bestIndividuals.Add(individual);
                }
            }
            //adauga la bestIndividuals cei mai putini dominati indivizi (primele fronturi)
            while (bestIndividuals.Count < populationSize / 2)
            {
                //indivizii de pe 1
                foreach (var individual in population)
                {
                    if (individual.FrontNumber == 1)
                    {
                        bestIndividuals.Add(individual);
                    }
                    if (bestIndividuals.Count >= populationSize / 2)
                    {
                        break;
                    }

                }
                if (bestIndividuals.Count < populationSize / 2)
                {
                    //indivizii de pe 2,daca nu sunt suficienti
                    foreach (var individual in population)
                    {
                        if (individual.FrontNumber == 2)
                        {
                            bestIndividuals.Add(individual);
                        }
                        if (bestIndividuals.Count >= populationSize / 2)
                        {
                            break;
                        }
                    }
                }
                if (bestIndividuals.Count < populationSize / 2)
                {
                    break;
                }
            }
        }
        public void StoreBestIndividuals()
        {
            simulationResults.bestIndividuals.AddRange(bestIndividuals);
            simulationResults.firstFront.AddRange(firstFront);
        }
        private void DoMutationAndSimulation()
        {
            foreach (var bestIndividual in bestIndividuals)
            {  //preiau ultimul individ de pe primul front ( 0 )
                Configuration configuration = new Configuration();
                if (bestIndividual.FrontNumber == 0)
                {
                    try
                    {
                        foreach (var individual in population)
                        {
                            if ((bestIndividual.Name.Substring(bestIndividual.Name.LastIndexOf('_') + 1)).Equals(individual.Name))
                            {
                                configuration = individual;
                                break;
                            }
                        }

                        Configuration mutatedConfiguration = Mutate((Configuration)configuration.Clone());

                        mutatedConfiguration.Name = id.ToString();
                        id++;
                        allConfigurations.Add(mutatedConfiguration);

                        //rulare simulator doar pt mutatedConfiguration
                       
                        var newSimulatedConfigurations = sim.Run(new List<Configuration> { mutatedConfiguration }, selectedTraces);
                        double ipc_average = 0, power_average = 0, energy_average = 0;
                        foreach (var item in newSimulatedConfigurations)
                        {
                            ipc_average += item.ipc;
                            power_average += item.power;
                            energy_average += item.energy;
                        }
                        ipc_average /= newSimulatedConfigurations.Count;
                        power_average /= newSimulatedConfigurations.Count;
                        energy_average /= newSimulatedConfigurations.Count;

                        GeneticIndividual tempIndividual = new GeneticIndividual(mutatedConfiguration, ipc_average, power_average, energy_average);
                        nsgaPopulation.Add(tempIndividual);

                       // Thread.Sleep(20);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);

                    }
                }
            }

        }
        private void DoCrossoverForBestIndividuals()
        {
            int length = firstFront.Count;

            int numberOfConfigurations = allConfigurations.Count;

            while (nsgaPopulation.Count < numberOfConfigurations)
            {
                int index_parent1, index_parent2;
                int count = 10;

                do
                {
                    index_parent1 = random.Next(0, bestIndividuals.Count);
                    index_parent2 = random.Next(0, bestIndividuals.Count);
                    count--;
                } while (index_parent1 == index_parent2 && count != 0);
                if (bestIndividuals.Count > 1)
                {
                    if (count == 0)
                    {
                        index_parent1 = bestIndividuals.Count - 1;
                        index_parent2 = bestIndividuals.Count - 2;
                    }

                    Configuration parent1 = bestIndividuals[index_parent1];
                    Configuration parent2 = bestIndividuals[index_parent2];

                    List<Configuration> resultedConfigurations = Crossover(parent1, parent2);
                    allConfigurations.AddRange(resultedConfigurations);

                  
                    var newSimulatedConfigurations = sim.Run(resultedConfigurations, selectedTraces);

                    foreach (var cfg in resultedConfigurations)
                    {
                        if (nsgaPopulation.Count < numberOfConfigurations)
                        {
                            double ipc_average = 0, power_average = 0, energy_average = 0;
                            int ct = 0;
                            foreach (var item in newSimulatedConfigurations)
                            {
                                if (item.cfg == cfg)
                                {
                                    ipc_average += item.ipc;
                                    power_average += item.power;
                                    energy_average += item.energy;
                                    ct++;
                                }
                            }
                            ipc_average /= ct;
                            power_average /= ct;
                            energy_average /= ct;
                            GeneticIndividual tempIndividual = new GeneticIndividual(cfg, ipc_average, power_average, energy_average);
                            nsgaPopulation.Add(tempIndividual);
                        }
                    }
                }
            }
        }
        private void KeepNondominatedIndividuals()
        {
            nsgaPopulation.RemoveAll(item => !bestIndividuals.Contains(item));
        }
        private void Clear()
        {
            bestIndividuals.Clear();
            firstFront.Clear();

        }
		public void ClearAll()
		{
			bestIndividuals.Clear();
			firstFront.Clear();
			population.Clear();
			archive.Clear();
			selectedTraces.Clear();
			nsgaPopulation.Clear();
			allConfigurations.Clear();
		}

        //http://www.cleveralgorithms.com/nature-inspired/evolution/spea.html
        public bool SPEA2()
        {
			if (newPopulation.Count > 0)
			{
				population.Clear();
				population.AddRange(newPopulation);
			}
			CalculateObjectives();
			firstFront.Clear();
            SPEA2Fitness();
            archive = EnvironmentalSelection(population, archive, populationSize / 2);

           
            //population.Clear();
            newPopulation=Reproduce(archive);
			return true;

		}
		List<Configuration> newPopulation = new List<Configuration>();
        private List<Configuration> Reproduce(List<GeneticIndividual> selected)
        {
            List<Configuration> children = new List<Configuration>();
			
			while (children.Count < populationSize)
            {
				int i = r.Next(selected.Count);
				int j = r.Next(selected.Count);

				var p1 = (selected[i].Fitness < selected[j].Fitness ? selected[i] : selected[j]);
				i = r.Next(selected.Count);
				j = r.Next(selected.Count);

				var p2 = (selected[i].Fitness < selected[j].Fitness ? selected[i] : selected[j]);
				if (p1!=p2)
                    children.AddRange(Crossover(p1, p2));
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
            List<Configuration> union = population.Concat(archive).ToList();
			firstFront.Clear();
            foreach (var item in union)
            {
                //adaug frontol ne-dominat
                if (((GeneticIndividual)item).Fitness < 1)
                {
					firstFront.Add((GeneticIndividual)item);
                    newArchive.Add((GeneticIndividual)item);
                }
            }

			if (newArchive.Count == archiveSize)
				return newArchive;
			if (newArchive.Count < archiveSize)
            {
				union = union.OrderBy(c => ((GeneticIndividual)c).Fitness).ToList();
                foreach (var item in union)
                {
                    if (((GeneticIndividual)item).Fitness >= 1.0)
                    {
                        newArchive.Add((GeneticIndividual)item);
                    }
                    if (newArchive.Count == archiveSize)
                        break;
                }
                return newArchive;
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
							if(p1!=p2)
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
                ((GeneticIndividual)individual).Density = CalculateDensity((GeneticIndividual)individual, union);
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
                        if (Dominates((GeneticIndividual)individual, (GeneticIndividual)other) > 0)
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
                        if (Dominates((GeneticIndividual)individual,(GeneticIndividual)other)<0)
                        {//individual este dominat, este dominat de .Fitness indivizi
                            ((GeneticIndividual)individual).R += ((GeneticIndividual)other).Strength;
                        }
                }
            }
        }
        public double CalculateDensity(GeneticIndividual individual, List<Configuration> pop)
        {
            foreach (var other in pop)
            {
				if(individual!=other)
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
                    ((GeneticIndividual)population[i]).Ipc += 1.0/results[i + population.Count * j].ipc;
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
		public int Dominates(GeneticIndividual ind1, GeneticIndividual ind2)
		{
			if (ind1.Energy < ind2.Energy  && ind1.Ipc < ind2.Ipc)
				return 1;
			if (ind1.Energy >= ind2.Energy && ind1.Ipc >= ind2.Ipc)
				return -1;
			return 0;
		}
    }
}