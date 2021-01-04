using ANOVA;
using System.Windows;
using System.Windows.Controls;

namespace ANOVA.Frontend.Controls
{
    /// <summary>
    /// Interaction logic for GoBackControl.xaml
    /// </summary>
    public partial class GoBackControl : UserControl
    {

        public GoBackControl()
        {
            InitializeComponent();
        }

        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            IWindowReturnable toReturn = Window.GetWindow(this) as IWindowReturnable;
            toReturn.ReturnToPreviousWindow();
        }
    }
}
