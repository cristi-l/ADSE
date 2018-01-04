using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithms
{
    class SolutionParamethers
    {
        public double Energy { get; set; }
        public double Ipc { get; set; }
        public double Power { get; set; }
        public string ConfigurationName { get; set; }
        public int Np { get; set; }
        public double Strength { get; set; }
        public double Fitness { get; set; }
        public List<SolutionParamethers> MyDominators = new List<SolutionParamethers>();

        public SolutionParamethers(double energy, double ipc, string configurationNo)
        {
            Energy = energy;
            Ipc = ipc;
            ConfigurationName = ConfigurationName;
            Fitness = 0;
            Strength = 0;
            Np = 0;
        }
    }
}
