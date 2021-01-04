using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ANOVA.Frontend
{
    /// <summary>
    /// Classes that implement this interface will be able to set returning window and to return toit
    /// </summary>
    public interface IWindowReturnable
    {
        void ReturnToPreviousWindow();

        void SetReturningWindow(Window window);
    }
}
