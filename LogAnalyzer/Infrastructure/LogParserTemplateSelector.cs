using LogAnalyzer.API.LogParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LogAnalyzer.Infrastructure
{
    public class LogParserTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (item is ILogParserEditorViewModel)
            {
                DataTemplate result = element.FindResource((item as ILogParserEditorViewModel).EditorResource) as DataTemplate;
                return result;
            }

            return base.SelectTemplate(item, container);
        }
    }
}
