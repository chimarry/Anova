using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANOVA.Frontend
{
    /// <summary>
    /// Classes that implement this interface could be refreshed when they implement this method
    /// </summary>
   public interface IRefreshable
    {
        void Refresh();
    }
}
