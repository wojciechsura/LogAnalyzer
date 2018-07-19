using LogAnalyzer.Common.Tools;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LogAnalyzer.Wpf.ValidationRules
{
    public class DateConverterValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is string dateString && DateConverter.Validate(dateString))
                return ValidationResult.ValidResult;
            else
                return new ValidationResult(false, "Invalid date");
        }
    }
}
