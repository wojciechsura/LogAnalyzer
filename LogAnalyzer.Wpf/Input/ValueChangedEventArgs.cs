using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Wpf.Input
{
    public class ValueChangedEventArgs
    {
        // Private fields ------------------------------------------------------

        private bool value;

        // Public methods ------------------------------------------------------

        public ValueChangedEventArgs(bool newValue)
        {
            value = newValue;
        }

        // Public properties ---------------------------------------------------

        public bool Value
        {
            get
            {
                return value;
            }
        }
    }
}
