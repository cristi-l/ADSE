using PSATSim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithms
{
    public class SimulationResults
    {
        public List<GeneticIndividual> nsgaPopulation = new List<GeneticIndividual>();
        public List<Configuration> allConfigurations = new List<Configuration>();
        public List<GeneticIndividual> firstFront = new List<GeneticIndividual>();
        public List<GeneticIndividual> bestIndividuals = new List<GeneticIndividual>();
    }
}
