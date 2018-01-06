using PSATSim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GeneticAlgorithms;
using InteractiveDataDisplay.WPF;
namespace ADSE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

		GeneticAlgorithms.GeneticAlgorithms ga;
		double[] x, y;
		List<string> selectedTraces = new List<string>
			{
				"Traces\\fpppp.tra",
				//"Traces\\applu.tra",
				//"Traces\\toast.tra"
			};
		public MainWindow()
        {
            InitializeComponent();
            PsatSim psatSim = new PsatSim();

            //trebuie selectate din interfata precum si populationCount
           
			//generare configuratii random
			ga = new GeneticAlgorithms.GeneticAlgorithms(selectedTraces);
			//ga.InitRandomPopulation(selectedTraces);
			//ga.SPEA2();
            

        }

        private List<Configuration> CreateRandomConfigurations(int count)
        {
            int configurationIndex = 0;
            ConfigurationGenerator configurationGenerator = new ConfigurationGenerator();
            var result = new List<Configuration>();
            for (int i = 0; i < count; i++)
            {
                result.Add(configurationGenerator.RandomConfig(string.Format("configuration-{0}", configurationIndex++)));
            }
            return result;
        }

		private void button_Click(object sender, RoutedEventArgs e)
		{
			ga.SPEA2();
			x = new double[ga.population.Count];
			y = new double[ga.population.Count];
			for (int i = 0; i < ga.population.Count; i++)
			{
				x[i] = ((GeneticIndividual)ga.population[i]).Ipc;
				y[i] = ((GeneticIndividual)ga.population[i]).Energy;
			}
			arch.PlotXY(x, y);
		}

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            ga.NSGA2(ga.nsgaPopulation, 0);
            x = new double[ga.simulationResults.bestIndividuals.Count];
            y = x;
            for (int i = 0; i < ga.simulationResults.bestIndividuals.Count; i++)
            {
                x[i] = 1.0/(ga.simulationResults.bestIndividuals[i]).Ipc;
                y[i] = (ga.simulationResults.bestIndividuals[i]).Energy;
            }
            arch.PlotXY(x, y);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
		{
			button1.IsEnabled = false;
			ga.InitRandomPopulation(selectedTraces);
			x = new double[ga.population.Count];
			y = new double[ga.population.Count];
			for (int i = 0; i < ga.population.Count; i++)
			{
				x[i] = ((GeneticIndividual)ga.population[i]).Ipc;
				y[i] = ((GeneticIndividual)ga.population[i]).Energy;
			}
			arch.PlotXY(x, y);
			
		}
	}
}
