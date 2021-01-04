using Accord;
using Accord.Math.Random;
using Accord.Statistics.Distributions.Univariate;
using ANOVA.Frontend.Utilities;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ANOVA.Frontend
{
    /// <summary>
    /// Interaction logic for MonteCarloCalculationWindow.xaml
    /// </summary>
    public partial class MonteCarloCalculationWindow : IWindowReturnable
    {
        public const int MaxNumberOfDecimals = 15;

        public Window PreviousWindow { get; set; }

        public int Number { get; set; }


        public bool Precision { get; set; }

        public MonteCarloCalculationWindow(bool precision)
        {
            InitializeComponent();
            Precision = precision;
            if (precision)
                NumberText.Text = $"Number of decimals less than {MaxNumberOfDecimals}";
            else
                NumberText.Text = "Number of variables";
            OriginalPiValue.Text = Math.PI.ToString();
        }

        public void ReturnToPreviousWindow()
        {
            WindowUtility.ShowWindow(this, PreviousWindow);
        }

        public void SetReturningWindow(Window window)
        {
            PreviousWindow = window;
        }


        private async void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            Number = int.Parse(NumberBox.Text);
            if (Precision && Number > MaxNumberOfDecimals)
            {
                WriteMessage();
                return;
            }
            double piApproximation = 0;
            int total = 0;
            int numInCircle = 0;
            // Construct a random number generator that generates random deviates
            // distributed uniformly over the interval [-1,1]
            Process.Text = "Processing...";
            Task task = new Task(() =>
            {
                Random rng = new Random();

                // We'll approximate pi to within 5 digits.
                double tolerance = double.Parse($"1e-{Number}");
                double x, y; // Coordinates of the random point.

                if (Precision)
                    while (Math.Abs(Math.PI - piApproximation) > tolerance)
                    {
                        x = rng.NextDouble();
                        y = rng.NextDouble();
                        if (Math.Sqrt(x * x + y * y) <= 1.0) // Is the point in the circle?
                            ++numInCircle;
                        ++total;
                        piApproximation = 4.0 * (numInCircle / (double)total);
                    }
                else
                {
                    while (total < Number)
                    {
                        x = rng.NextDouble();
                        y = rng.NextDouble();
                        if (x * x + y * y <= 1.0) // Is the point in the circle?
                            ++numInCircle;
                        ++total;
                        piApproximation = 4.0 * (numInCircle / (double)total);
                    }
                }
            });
            task.Start();
            await task;
            Process.Text = string.Empty;
            PiValue.Text = piApproximation.ToString();
            NumberOfDots.Text = numInCircle.ToString();
        }
        private void NumberBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void WriteMessage()
        {
            // Write message and clear fields
            MessageBox.Show("You did not enter valid number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
