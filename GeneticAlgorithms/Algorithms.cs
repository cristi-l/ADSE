using PSATSim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithms
{
    class Algorithms
    {
        public int PopulationSize { get; set; }
        public int NoOfGenerations { get; set; }

        private Random random;

        public Algorithms()
        {
            random = new Random();
        }

        public List<Configuration> Crossover(List<Configuration> selectedPopulation)
        {
            var input = new List<Configuration>(selectedPopulation);
            input = new List<Configuration>(input.OrderBy((item) => random.Next()));

            var output = new List<Configuration>();
            for (int i = 0; i < input.Count; i += 2)
            {
                var parentOne = input[i];
                var parentTwo = input[i + 1];

                var children = GetChildren(parentOne, parentTwo);

                output.AddRange(children);
            }

            return output;
        }
        private IEnumerable<Configuration> GetChildren(Configuration parentOne, Configuration parentTwo)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            return null;
        }

        private List<Configuration> Mutation(List<Configuration> crossoveredPopulation)
        {
            return null;

        }
    }
}
