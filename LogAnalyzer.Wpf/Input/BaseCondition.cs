using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Wpf.Input
{
    public delegate void ValueChangedHandler(object sender, ValueChangedEventArgs e);

    public abstract class BaseCondition
    {
        protected virtual void OnValueChanged(bool newValue)
        {
            var handler = ValueChanged;

            if (handler != null)
                handler(this, new ValueChangedEventArgs(newValue));
        }

        public static BaseCondition operator & (BaseCondition first, BaseCondition second)
        {
            return new CompositeCondition(CompositeCondition.CompositionKind.And, first, second);
        }

        public static BaseCondition operator |(BaseCondition first, BaseCondition second)
        {
            return new CompositeCondition(CompositeCondition.CompositionKind.Or, first, second);
        }

        public static BaseCondition operator !(BaseCondition condition)
        {
            return new NegateCondition(condition);
        }

        public abstract bool GetValue();

        public event ValueChangedHandler ValueChanged;
    }
}
