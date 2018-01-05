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
namespace ADSE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            PsatSim psatSim = new PsatSim();

            //trebuie selectate din interfata precum si populationCount
            List<string> selectedTraces = new List<string>
            {
				"Traces\\fpppp.tra",
				"Traces\\applu.tra",
				"Traces\\toast.tra"
			};
			//generare configuratii random
			GeneticAlgorithms.GeneticAlgorithms ga = new GeneticAlgorithms.GeneticAlgorithms();
			ga.InitRandomPopulation(selectedTraces);
			ga.SPEA2();
            

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

    }
}
