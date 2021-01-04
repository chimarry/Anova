using ANOVA.Frontend.Utilities;
using System.Windows;

namespace ANOVA.Frontend
{
    /// <summary>
    /// Interaction logic for ChooseModeWindow.xaml
    /// </summary>
    public partial class ChooseModeWindow : Window
    {
        public ChooseModeWindow()
        {
            InitializeComponent();
        }

        private void AnovaButton_Click(object sender, RoutedEventArgs e)
        {
            WindowUtility.ShowWindow(this, new AnovaWindow() { PreviousWindow = new ChooseModeWindow() });
        }

        private void MonteCarloButton_Click(object sender, RoutedEventArgs e)
        {
            WindowUtility.ShowWindow(this, new MonteCarloSimulationWindow() { PreviousWindow = new ChooseModeWindow() });
        }
    }
}
