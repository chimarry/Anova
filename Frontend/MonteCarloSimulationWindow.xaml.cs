using ANOVA.Frontend.Utilities;
using System.Windows;

namespace ANOVA.Frontend
{
    /// <summary>
    /// Interaction logic for MonteCarloSimulationWindow.xaml
    /// </summary>
    public partial class MonteCarloSimulationWindow : IRefreshable, IWindowReturnable
    {
        public Window PreviousWindow { get; set; }

        public MonteCarloSimulationWindow()
        {
            InitializeComponent();
        }

        public void Refresh()
        {
            WindowUtility.ShowWindow(this, new MonteCarloSimulationWindow());
        }

        public void ReturnToPreviousWindow()
        {
            WindowUtility.ShowWindow(this, PreviousWindow);
        }

        public void SetReturningWindow(Window window)
        {
            PreviousWindow = window;
        }

        private void NumberOfVariablesButton_Click(object sender, RoutedEventArgs e)
        {
            WindowUtility.ShowWindow(this, new MonteCarloCalculationWindow(false) { PreviousWindow = new MonteCarloSimulationWindow() { PreviousWindow = new ChooseModeWindow() } });
        }

        private void SpecificPrecisionButton_Click(object sender, RoutedEventArgs e)
        {
            WindowUtility.ShowWindow(this, new MonteCarloCalculationWindow(true) { PreviousWindow = new MonteCarloSimulationWindow() { PreviousWindow = new ChooseModeWindow() } });
        }
    }
}
