using ANOVA.Frontend.Utilities;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace ANOVA.Frontend
{
    /// <summary>
    /// Interaction logic for AnovaWindow.xaml
    /// </summary>
    public partial class AnovaWindow : IRefreshable, IWindowReturnable
    {
        public Window PreviousWindow { get; set; }

        public AnovaWindow()
        {
            InitializeComponent();
        }

        public void Refresh()
        {
            WindowUtility.ShowWindow(this, new ChooseModeWindow());
        }

        public void ReturnToPreviousWindow()
        {
            WindowUtility.ShowWindow(this, PreviousWindow);
        }

        public void SetReturningWindow(Window window)
        {
            PreviousWindow = window;
        }

        private void SystemsCountTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidateInput(e);
        }

        private void MeasurementsCountTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidateInput(e);
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(MeasurementsCountTextBox.Text) || string.IsNullOrEmpty(SystemsCountTextBox.Text))
                WriteMessageAndClearFields();
            else
            {
                int measurementsCount = int.Parse(MeasurementsCountTextBox.Text);
                int systemsCount = int.Parse(SystemsCountTextBox.Text);
                AnovaCalculationWindow anovaCalculationWindow = new AnovaCalculationWindow(measurementsCount, systemsCount)
                {
                    PreviousWindow = new AnovaWindow()
                    {
                        PreviousWindow = new ChooseModeWindow()
                    },
                };
                WindowUtility.ShowWindow(this, anovaCalculationWindow);
            }
        }

        private void ValidateInput(TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void WriteMessageAndClearFields()
        {
            // Write message and clear fields
            MessageBox.Show("You did not enter valid number of measurements and systems", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            SystemsCountTextBox.Text = null;
            MeasurementsCountTextBox.Text = null;
        }
    }
}
