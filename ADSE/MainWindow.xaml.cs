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
using System.Windows.Threading;

namespace ADSE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		bool flag;
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
			flag = true;

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

		private async void button_Click(object sender, RoutedEventArgs e)
		{
			if (flag == true)
			{
				flag = false;

				await Task.Run(() => flag = ga.SPEA2());
				//ga.SPEA2();
				x = new double[ga.population.Count];
				y = new double[ga.population.Count];
				for (int i = 0; i < ga.population.Count; i++)
				{
					x[i] = ((GeneticIndividual)ga.population[i]).Ipc;
					y[i] = ((GeneticIndividual)ga.population[i]).Energy;
				}
				population.PlotXY(x, y);
				x = new double[ga.firstFront.Count];
				y = new double[ga.firstFront.Count];
				for (int i = 0; i < ga.firstFront.Count; i++)
				{
					x[i] = ((GeneticIndividual)ga.firstFront[i]).Ipc;
					y[i] = ((GeneticIndividual)ga.firstFront[i]).Energy;
				}
				best.PlotXY(x, y);

			}
		}

		private async void button2_Click(object sender, RoutedEventArgs e)
		{
			if (flag == true)
			{
				flag = false;
				await Task.Run(() => flag = ga.NSGA2(ga.nsgaPopulation, 0, pbStatus));

				ga.FindFirstFront(ga.simulationResults.nsgaPopulation);
				ga.simulationResults.firstFront.Clear();
				ga.StoreBestIndividuals();

				x = new double[ga.simulationResults.nsgaPopulation.Count];
				y = new double[ga.simulationResults.nsgaPopulation.Count];
				for (int i = 0; i < ga.simulationResults.nsgaPopulation.Count; i++)
				{
					x[i] = (1.0 / (ga.simulationResults.nsgaPopulation[i]).Ipc);
					y[i] = (ga.simulationResults.nsgaPopulation[i]).Energy;
				}
				population.PlotXY(x, y);

				x = new double[ga.simulationResults.firstFront.Count];
				y = new double[ga.simulationResults.firstFront.Count];
				for (int i = 0; i < ga.simulationResults.firstFront.Count; i++)
				{
					x[i] = (1.0 / (ga.simulationResults.firstFront[i]).Ipc);
					y[i] = (ga.simulationResults.firstFront[i]).Energy;
				}
				best.PlotXY(x, y);
			}
		}

		private async void button1_Click(object sender, RoutedEventArgs e)
		{
			if (flag == true)
			{
				flag = false;
				await Task.Run(() => flag = ga.InitRandomPopulation(selectedTraces));
				button1.IsEnabled = false;

				
				x = new double[ga.population.Count];
				y = new double[ga.population.Count];
				for (int i = 0; i < ga.population.Count; i++)
				{
					x[i] = ((GeneticIndividual)ga.population[i]).Ipc;
					y[i] = ((GeneticIndividual)ga.population[i]).Energy;
				}
				best.PlotXY(x, y);
			}
		}
	}
}
