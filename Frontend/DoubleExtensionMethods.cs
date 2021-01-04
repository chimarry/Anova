using System;
using System.Collections.Generic;
using System.Text;

namespace ANOVA.Frontend
{
    public static class DoubleExtensionMethods
    {
        public static string AnovaValue(this double value) => Math.Round(value, 4).ToString();
    }
}
