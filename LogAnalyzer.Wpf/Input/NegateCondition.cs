using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Wpf.Input
{
    public class NegateCondition : BaseCondition
    {
        // Private fields ----------------------------------------------------

        private readonly BaseCondition condition;

        // Private methods ---------------------------------------------------

        private void HandleInnerConditionValueChanged(object sender, ValueChangedEventArgs e)
        {
            OnValueChanged(!(e.Value));
        }

        // Public methods ----------------------------------------------------

        public NegateCondition(BaseCondition newCondition)
        {
            if (newCondition == null)
                throw new ArgumentNullException("newCondition");

            condition = newCondition;
            condition.ValueChanged += HandleInnerConditionValueChanged;
        }

        public override bool GetValue()
        {
            return !condition.GetValue();
        }
    }
}
