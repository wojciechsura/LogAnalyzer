using LogAnalyzer.Common.Extensions;
using LogAnalyzer.Models.Engine.PredicateDescriptions;
using LogAnalyzer.Models.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.BusinessLogic.ViewModels.Highlighting
{
    public abstract class BaseRuleDataEditorViewModel : INotifyPropertyChanged
    {
        private ComparisonMethodInfo selectedComparisonMethod;

        private void BuildComparisonMethods()
        {
            foreach (ComparisonMethod method in Enum.GetValues(typeof(ComparisonMethod)))
            {
                ComparisonMethods.Add(new ComparisonMethodInfo(method));
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public BaseRuleDataEditorViewModel()
        {
            ComparisonMethods = new ObservableCollection<ComparisonMethodInfo>();
            BuildComparisonMethods();

            SelectedComparisonMethod = ComparisonMethods.FirstOrDefault();
        }

        public BaseRuleDataEditorViewModel(PredicateDescription condition)
        {
            ComparisonMethods = new ObservableCollection<ComparisonMethodInfo>();
            BuildComparisonMethods();

            SelectedComparisonMethod = ComparisonMethods.Single(c => c.ComparisonMethod == condition.Comparison);
        }

        public class ComparisonMethodInfo
        {
            public ComparisonMethodInfo(ComparisonMethod method)
            {
                ComparisonMethod = method;
                Display = method.GetAttribute<DescriptionAttribute>().Description;
            }

            public ComparisonMethod ComparisonMethod { get; }
            public string Display { get; }
        }

        public ObservableCollection<ComparisonMethodInfo> ComparisonMethods { get; }

        public ComparisonMethodInfo SelectedComparisonMethod
        {
            get => selectedComparisonMethod;
            set
            {
                selectedComparisonMethod = value;
                OnPropertyChanged(nameof(SelectedComparisonMethod));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
