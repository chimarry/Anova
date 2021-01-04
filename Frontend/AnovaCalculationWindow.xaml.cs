using Accord.Statistics.Distributions.Univariate;
using ANOVA.Frontend.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for AnovaCalculationWindow.xaml
    /// </summary>
    public partial class AnovaCalculationWindow : IRefreshable, IWindowReturnable
    {
        public Window PreviousWindow { get; set; }
        public int MeasurementsCount { get; set; }
        public int AlternativesCount { get; set; }

        public const int PixelOffset = 5;

        public AnovaCalculationWindow(int measurementsCount, int alternativesCount)
        {
            MeasurementsCount = measurementsCount;
            AlternativesCount = alternativesCount;
            InitializeComponent();
            InitializeComponents();
        }

        public void Refresh()
        {
            WindowUtility.ShowWindow(this, new AnovaCalculationWindow(MeasurementsCount, AlternativesCount));
        }

        public void ReturnToPreviousWindow()
        {
            WindowUtility.ShowWindow(this, PreviousWindow);
        }

        public void SetReturningWindow(Window window)
        {
            PreviousWindow = window;
        }

        private void AnovaButton_Click(object sender, RoutedEventArgs e)
        {
            CalculateAndWriteAnovaValues();

        }

        private void ContrastsButton_Click(object sender, RoutedEventArgs e)
        {
            CalculateAndWriteContrastValues();
        }

        private void CalculateAndWriteContrastValues()
        {
            // Calculations
            List<double> meanValues = new List<double>();
            List<double> effects = new List<double>();
            double[][] measurements = GetMeasurements();
            double SSE = 0.0, Se2, t = 0.0;

            // Get alternative's measurements and calculate their mean value
            for (int i = 0; i < measurements.Length; ++i)
                meanValues.Add(measurements[i].Average());

            // Calculate total mean value
            double totalMeanValue = measurements.Select(x => x.Sum()).Sum() / (MeasurementsCount * AlternativesCount);

            // Calculate effects
            foreach (double meanValue in meanValues)
                effects.Add(meanValue - totalMeanValue);
            if (!effects.Any(x => x != 0.0))
            {
                WriteMessage("Enter measurements.");
                return;
            }
            for (int i = 0; i < AlternativesCount; ++i)
                for (int j = 0; j < MeasurementsCount; ++j)
                    SSE += Math.Pow(measurements[i][j] - meanValues[i], 2);
            Se2 = SSE / (AlternativesCount * (MeasurementsCount - 1));

            double Sc = Math.Sqrt(Se2 * 2 / (MeasurementsCount * AlternativesCount));

            if (string.IsNullOrEmpty(PercentageTextBox.Text))
            {
                WriteMessage("Percentage must be specified.");
                return;
            }
            double percentage = double.Parse(PercentageTextBox.Text);
            if (percentage >= 1.0 || percentage < 0.0 || percentage == 0.5)
            {
                WriteMessage("Percentage must be in correct format (decimal form). Must not be 100%(1.0) or 50%(0.5).");
                return;
            }

            // Find interval around Sc
            int degOfFreedom = AlternativesCount * (MeasurementsCount - 1);
            double percentageNumber = 1 - (1 - percentage) / 2;
            if (MeasurementsCount <= 30)
            {
                TDistribution distribution = new TDistribution(degOfFreedom);
                t = distribution.GetRange(percentageNumber).Max;
            }
            else
            {
                NormalDistribution distribution = new NormalDistribution();
                t = distribution.GetRange(percentageNumber).Max;
            }
            double constantFactor = t * Sc;
            int index = 1;
            for (int i = 0; i < AlternativesCount; ++i)
                for (int j = i + 1; j < AlternativesCount; ++j)
                {
                    double c = effects[i] - effects[j];
                    ((TextBlock)ContrastsGrid.Children.Cast<UIElement>().First(e => Grid.GetRow(e) == 1 && Grid.GetColumn(e) == index)).Text =
                        (c - constantFactor) < 0 && (c + constantFactor) >= 0 ? "Statistically the same." : "Statistically different.";
                    index++;
                }
        }

        private void CalculateAndWriteAnovaValues()
        {
            // Calculations
            List<double> meanValues = new List<double>();
            List<double> effects = new List<double>();
            double totalMeanValue, SSA = 0.0, SST = 0.0, SSE = 0.0, Sa2, Se2, F, Ft;
            double[][] measurements = GetMeasurements();

            // Get alternative's measurements and calculate their mean value
            for (int i = 0; i < measurements.Length; ++i)
                meanValues.Add(measurements[i].Average());

            // Calculate total mean value
            totalMeanValue = measurements.Select(x => x.Sum()).Sum() / (MeasurementsCount * AlternativesCount);

            // Calculate effects
            foreach (double meanValue in meanValues)
                effects.Add(meanValue - totalMeanValue);

            // Calculate SST, SSE, SSA
            SSA = MeasurementsCount * (effects.Select(x => Math.Pow(x, 2)).Sum());
            for (int i = 0; i < AlternativesCount; ++i)
                for (int j = 0; j < MeasurementsCount; ++j)
                    SSE += Math.Pow(measurements[i][j] - meanValues[i], 2);
            SST = SSA + SSE;

            // Deviations
            Sa2 = SSA / (AlternativesCount - 1);
            Se2 = SSE / (AlternativesCount * (MeasurementsCount - 1));

            // F- test
            F = Sa2 / Se2;

            // F-distribution value
            if (string.IsNullOrEmpty(PercentageTextBox.Text))
            {
                WriteMessage("Percentage must be specified.");
                return;
            }
            double percentage = double.Parse(PercentageTextBox.Text);
            if (percentage >= 1.0 || percentage < 0.0 || percentage == 0.5)
            {
                WriteMessage("Percentage must be in correct format (decimal form). Must not be 100%(1.0) or 50%(0.5).");
                return;
            }
            // Find interval around Sc
            Ft = Math.Round(new FDistribution(AlternativesCount - 1, AlternativesCount * (MeasurementsCount - 1)).GetRange(percentage).Max, 2);

            for (int i = 1; i < AlternativesCount + 1; ++i)
            {
                TextBlock meanValueTextBlock = (TextBlock)MatrixGrid.Children.Cast<UIElement>().First(e => Grid.GetRow(e) == (MeasurementsCount + 1) && Grid.GetColumn(e) == i);
                meanValueTextBlock.Visibility = Visibility.Visible;
                meanValueTextBlock.Text = meanValues[i - 1].AnovaValue();

                TextBlock effectTextBlock = (TextBlock)MatrixGrid.Children.Cast<UIElement>().First(e => Grid.GetRow(e) == (MeasurementsCount + 2) && Grid.GetColumn(e) == i);
                effectTextBlock.Visibility = Visibility.Visible;
                effectTextBlock.Text = effects[i - 1].AnovaValue();
            }
            string finishResult = string.Empty;
            ((TextBlock)AnovaResultsGrid.Children.Cast<UIElement>().First(e => Grid.GetRow(e) == 0 && Grid.GetColumn(e) == 1)).Text = totalMeanValue.AnovaValue();
            ((TextBlock)AnovaResultsGrid.Children.Cast<UIElement>().First(e => Grid.GetRow(e) == 1 && Grid.GetColumn(e) == 1)).Text = SSA.AnovaValue();
            ((TextBlock)AnovaResultsGrid.Children.Cast<UIElement>().First(e => Grid.GetRow(e) == 2 && Grid.GetColumn(e) == 1)).Text = SST.AnovaValue();
            ((TextBlock)AnovaResultsGrid.Children.Cast<UIElement>().First(e => Grid.GetRow(e) == 3 && Grid.GetColumn(e) == 1)).Text = SSE.AnovaValue();
            ((TextBlock)AnovaResultsGrid.Children.Cast<UIElement>().First(e => Grid.GetRow(e) == 4 && Grid.GetColumn(e) == 1)).Text = Sa2.AnovaValue();
            ((TextBlock)AnovaResultsGrid.Children.Cast<UIElement>().First(e => Grid.GetRow(e) == 5 && Grid.GetColumn(e) == 1)).Text = Se2.AnovaValue();
            ((TextBlock)AnovaResultsGrid.Children.Cast<UIElement>().First(e => Grid.GetRow(e) == 6 && Grid.GetColumn(e) == 1)).Text = Math.Round(F, 2).ToString();
            ((TextBlock)AnovaResultsGrid.Children.Cast<UIElement>().First(e => Grid.GetRow(e) == 7 && Grid.GetColumn(e) == 1)).Text = Math.Round(Ft, 2).ToString();
            if (F.CompareTo(double.NaN) == 0)
                finishResult = "Could not been compared.";
            else
                finishResult = (F > Ft) ? "Statistically different." : "Statistically the same";
            ((TextBlock)AnovaResultsGrid.Children.Cast<UIElement>().First(e => Grid.GetRow(e) == 8 && Grid.GetColumn(e) == 1)).Text = finishResult;
        }

        private void WriteMessage(string message)
        {
            // Write message and clear fields
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private double[][] GetMeasurements()
        {
            double[][] measurements = new double[AlternativesCount][];
            for (int i = 0; i < AlternativesCount; ++i)
                measurements[i] = new double[MeasurementsCount];
            for (int i = 1; i < AlternativesCount + 1; ++i)
                for (int j = 1; j < MeasurementsCount + 1; ++j)
                {
                    string measurement = ((TextBox)MatrixGrid.Children.Cast<UIElement>().First(e => Grid.GetRow(e) == j && Grid.GetColumn(e) == i)).Text;
                    if (string.IsNullOrEmpty(measurement))
                    {
                        measurement = ((TextBox)MatrixGrid.Children.Cast<UIElement>().First(e => Grid.GetRow(e) == j && Grid.GetColumn(e) == i)).Text = "0.0";
                    }
                    measurements[i - 1][j - 1] = double.Parse(measurement);
                }
            return measurements;
        }

        public void InitializeComponents()
        {
            AddMatrixGridDefinitions();

            AddPredefinedText();
            double width = AlternativesCount <= 10 ? MatrixGridDimensions.Width / (AlternativesCount + 1) - PixelOffset : 60;
            double height = MeasurementsCount <= 10 ? MatrixGridDimensions.Height / (MeasurementsCount + 3) - PixelOffset : 60;
            for (int i = 1; i < MeasurementsCount + 1; ++i)
                for (int j = 1; j < AlternativesCount + 1; ++j)
                {
                    TextBox textBox = new TextBox()
                    {
                        FontSize = 16,
                        Foreground = Brushes.White,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Background = new SolidColorBrush(new Color() { R = 255, G = 255, B = 255, A = 20 }),
                        Width = width,
                        Height = height,
                        TextWrapping = TextWrapping.Wrap
                    };
                    textBox.PreviewTextInput += PercentageTextBox_PreviewTextInput;
                    MatrixGrid.Children.Add(textBox);
                    Grid.SetRow(textBox, i);
                    Grid.SetColumn(textBox, j);
                }

            for (int i = 1; i < AlternativesCount + 1; ++i)
                for (int j = MeasurementsCount + 1; j < MeasurementsCount + 3; ++j)
                {
                    TextBlock textBox = new TextBlock()
                    {
                        FontSize = 16,
                        Foreground = Brushes.White,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Background = new SolidColorBrush(new Color() { R = 255, G = 255, B = 255, A = 20 }),
                        Width = width,
                        Height = height,
                        TextWrapping = TextWrapping.Wrap,
                        Visibility = Visibility.Hidden
                    };
                    MatrixGrid.Children.Add(textBox);
                    Grid.SetRow(textBox, j);
                    Grid.SetColumn(textBox, i);
                }
        }

        private void AddMatrixGridDefinitions()
        {
            // Create Columns
            for (int i = 0; i < AlternativesCount + 1; ++i)
                MatrixGrid.ColumnDefinitions.Add(new ColumnDefinition());

            for (int i = 0; i < AlternativesCount; ++i)
                for (int j = i + 1; j < AlternativesCount; ++j)
                    ContrastsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            ContrastsGrid.ColumnDefinitions.Add(new ColumnDefinition());

            // Create Rows
            for (int i = 0; i < MeasurementsCount + 1; ++i)
                MatrixGrid.RowDefinitions.Add(new RowDefinition());

            // Mean values and effects
            MatrixGrid.RowDefinitions.Add(new RowDefinition());
            MatrixGrid.RowDefinitions.Add(new RowDefinition());

            ContrastsGrid.RowDefinitions.Add(new RowDefinition());
            ContrastsGrid.RowDefinitions.Add(new RowDefinition());
        }

        private void AddPredefinedText()
        {
            for (int i = 1; i < AlternativesCount + 1; ++i)
            {
                TextBlock header = new TextBlock()
                {
                    FontSize = 10,
                    Foreground = Brushes.White,
                    Text = $"Alternative {i} ",
                    TextWrapping = TextWrapping.Wrap,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                MatrixGrid.Children.Add(header);
                Grid.SetRow(header, 0);
                Grid.SetColumn(header, i);
            }

            for (int i = 1; i < MeasurementsCount + 1; ++i)
            {
                TextBlock header = new TextBlock()
                {
                    FontSize = 10,
                    Foreground = Brushes.White,
                    Text = $"Measurement {i} ",
                    TextWrapping = TextWrapping.Wrap,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                };
                MatrixGrid.Children.Add(header);
                Grid.SetRow(header, i);
                Grid.SetColumn(header, 0);
            }


            // Mean value header
            TextBlock meanValue = new TextBlock()
            {
                FontSize = 10,
                Foreground = Brushes.White,
                Text = $"Mean value",
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            MatrixGrid.Children.Add(meanValue);
            Grid.SetRow(meanValue, MeasurementsCount + 1);
            Grid.SetColumn(meanValue, 0);

            // Effect header
            TextBlock effect = new TextBlock()
            {
                FontSize = 10,
                Foreground = Brushes.White,
                Text = $"Effect",
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            MatrixGrid.Children.Add(effect);
            Grid.SetRow(effect, MeasurementsCount + 2);
            Grid.SetColumn(effect, 0);

            for (int i = 0; i < 9; ++i)
            {
                TextBlock anovaResult = new TextBlock()
                {
                    FontSize = 10,
                    Foreground = Brushes.White,
                    TextWrapping = TextWrapping.Wrap,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                };
                AnovaResultsGrid.Children.Add(anovaResult);
                Grid.SetRow(anovaResult, i);
                Grid.SetColumn(anovaResult, 1);
            }

            TextBlock compareText = new TextBlock()
            {
                FontSize = 10,
                Foreground = Brushes.White,
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Text = "Compare: ",
                Width = 60,
                Height = 30,
            };
            ContrastsGrid.Children.Add(compareText);
            Grid.SetRow(compareText, 0);
            Grid.SetColumn(compareText, 0);

            TextBlock contrastText = new TextBlock()
            {
                FontSize = 10,
                Foreground = Brushes.White,
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Text = $"Result: ",
                Width = 60,
                Height = 30,
            };
            ContrastsGrid.Children.Add(contrastText);
            Grid.SetRow(contrastText, 1);
            Grid.SetColumn(contrastText, 0);

            // Contrasts
            var index = 1;
            for (int i = 0; i < AlternativesCount; ++i)
                for (int j = i + 1; j < AlternativesCount; ++j)
                {
                    TextBlock contrastName = new TextBlock()
                    {
                        FontSize = 10,
                        Foreground = Brushes.White,
                        TextWrapping = TextWrapping.Wrap,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Text = $"Alternatives {i + 1}:{j + 1} ",
                        Width = 60,
                        Height = 30,
                    };
                    ContrastsGrid.Children.Add(contrastName);
                    Grid.SetRow(contrastName, 0);
                    Grid.SetColumn(contrastName, index);
                    TextBlock contrastValue = new TextBlock()
                    {
                        FontSize = 10,
                        Foreground = Brushes.White,
                        TextWrapping = TextWrapping.Wrap,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Width = 60,
                        Height = 30,
                    };
                    ContrastsGrid.Children.Add(contrastValue);
                    Grid.SetRow(contrastValue, 1);
                    Grid.SetColumn(contrastValue, index);
                    index++;
                }
        }

        private void PercentageTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9\\.-]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
