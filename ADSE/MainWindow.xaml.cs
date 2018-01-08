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
using System.IO;
using System.Text.RegularExpressions;

namespace ADSE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		bool flag;
		GeneticAlgorithms.GeneticAlgorithms geneticAlg;
		double[] x, y;
		List<string> selectedTraces = new List<string>();
			
		
		public MainWindow()
        {
            InitializeComponent();
            PsatSim psatSim = new PsatSim();

            //trebuie selectate din interfata precum si populationCount
           
			//generare configuratii random
			

			flag = true;
			var dir= Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
			var dirInfo = new DirectoryInfo(dir+"\\PSATSim\\Traces");
			FileInfo[] info = dirInfo.GetFiles("*.tra");
			for(int i=0;i<info.Length;i++)
			{
				listView.Items.Add(new CheckBox()
				{
					Name ="Trace"+ i,
					Content = info[i].Name
				});
			}
			foreach (var item in listView.Items)
			{
				if (item.GetType() == typeof(CheckBox))
					((CheckBox)item).Checked += checkBox_Checked;
			}
			buttonInit.IsEnabled = false;
			buttonNSGA.IsEnabled = false;
			buttonSPEA.IsEnabled = false;
			buttonReset.IsEnabled = false;
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

		private async void button_SPEA(object sender, RoutedEventArgs e)
		{
			if (flag == true)
			{
				flag = false;
				buttonNSGA.IsEnabled = false;
				await Task.Run(() => flag = geneticAlg.SPEA2());
				//ga.SPEA2();
				x = new double[geneticAlg.population.Count];
				y = new double[geneticAlg.population.Count];
				for (int i = 0; i < geneticAlg.population.Count; i++)
				{
					x[i] = ((GeneticIndividual)geneticAlg.population[i]).Ipc;
					y[i] = ((GeneticIndividual)geneticAlg.population[i]).Energy;
				}
				population.PlotXY(x, y);

				x = new double[geneticAlg.firstFront.Count];
				y = new double[geneticAlg.firstFront.Count];
				for (int i = 0; i < geneticAlg.firstFront.Count; i++)
				{
					x[i] = ((GeneticIndividual)geneticAlg.firstFront[i]).Ipc;
					y[i] = ((GeneticIndividual)geneticAlg.firstFront[i]).Energy;
				}
				best.PlotXY(x, y);
				

			}
		}

		private async void button_NSGA(object sender, RoutedEventArgs e)
		{
			if (flag == true)
			{
				flag = false;
				buttonSPEA.IsEnabled = false;
				await Task.Run(() => flag = geneticAlg.NSGA2(geneticAlg.nsgaPopulation, 0, pbStatus));

				geneticAlg.FindFirstFront(geneticAlg.simulationResults.nsgaPopulation);
				geneticAlg.simulationResults.firstFront.Clear();
				geneticAlg.StoreBestIndividuals();

				x = new double[geneticAlg.simulationResults.nsgaPopulation.Count];
				y = new double[geneticAlg.simulationResults.nsgaPopulation.Count];
				for (int i = 0; i < geneticAlg.simulationResults.nsgaPopulation.Count; i++)
				{
					x[i] = (1.0 / (geneticAlg.simulationResults.nsgaPopulation[i]).Ipc);
					y[i] = (geneticAlg.simulationResults.nsgaPopulation[i]).Energy;
				}
				population.PlotXY(x, y);

				x = new double[geneticAlg.simulationResults.firstFront.Count];
				y = new double[geneticAlg.simulationResults.firstFront.Count];
				for (int i = 0; i < geneticAlg.simulationResults.firstFront.Count; i++)
				{
					x[i] = (1.0 / (geneticAlg.simulationResults.firstFront[i]).Ipc);
					y[i] = (geneticAlg.simulationResults.firstFront[i]).Energy;
				}
				best.PlotXY(x, y);
			}
		}

		private void checkBox_Checked(object sender, RoutedEventArgs e)
		{
			selectedTraces.Clear();
			foreach (var item in listView.Items)
			{
				if (item.GetType() == typeof(CheckBox))
					if ((bool)((CheckBox)item).IsChecked)
					{
						selectedTraces.Add("Traces\\"+((CheckBox)item).Content.ToString());
					}
			}
			buttonInit.IsEnabled = true;
			
		}



		private void NumericOnly(object sender, TextCompositionEventArgs e)
		{
			e.Handled = IsNumeric(((TextBox)sender).Text+e.Text);
		}
		private bool IsNumeric(string str)
		{
			Regex regex = new Regex("[^0-9]"); //regex that matches disallowed text
			return regex.IsMatch(str);
		}

		private void buttonReset_Click(object sender, RoutedEventArgs e)
		{
			geneticAlg.ClearAll();
			
			buttonInit.IsEnabled = false;
			buttonNSGA.IsEnabled = false;
			buttonSPEA.IsEnabled = false;
		}

		private async void button_Init(object sender, RoutedEventArgs e)
		{
			int nr;
			int.TryParse(textBox.Text, out nr);
			geneticAlg = new GeneticAlgorithms.GeneticAlgorithms(selectedTraces,nr);
			if (flag == true)
			{
				flag = false;
				await Task.Run(() => flag = geneticAlg.InitRandomPopulation(selectedTraces));
				buttonInit.IsEnabled = false;
				buttonReset.IsEnabled = true;
				buttonSPEA.IsEnabled = true;
				buttonNSGA.IsEnabled = true;
				x = new double[geneticAlg.population.Count];
				y = new double[geneticAlg.population.Count];
				for (int i = 0; i < geneticAlg.population.Count; i++)
				{
					x[i] = ((GeneticIndividual)geneticAlg.population[i]).Ipc;
					y[i] = ((GeneticIndividual)geneticAlg.population[i]).Energy;
				}
				best.PlotXY(x, y);
			}
		}
	}
}
