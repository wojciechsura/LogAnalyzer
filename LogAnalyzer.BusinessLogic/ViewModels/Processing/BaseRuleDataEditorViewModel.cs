using LogAnalyzer.API.Types;
using LogAnalyzer.Common.Extensions;
using LogAnalyzer.Models.Engine.PredicateDescriptions;
using LogAnalyzer.Models.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LogAnalyzer.BusinessLogic.ViewModels.Processing
{
    public abstract class BaseRuleDataEditorViewModel : INotifyPropertyChanged
    {
        // Public types -------------------------------------------------------

        public class ComparisonMethodInfo
        {
            public ComparisonMethodInfo(ComparisonMethod method)
            {
                ComparisonMethod = method;
                Display = method.GetAttribute<DescriptionAttribute>().Description;
                SummaryDisplay = method.GetAttribute<SummaryDisplayAttribute>().Summary;
            }

            public ComparisonMethod ComparisonMethod { get; }
            public string Display { get; }
            public string SummaryDisplay { get; }
        }

        // Private fields -----------------------------------------------------

        private ComparisonMethodInfo selectedComparisonMethod;

        // Private methods ----------------------------------------------------

        private void BuildComparisonMethods()
        {
            foreach (ComparisonMethod method in Enum.GetValues(typeof(ComparisonMethod)))
            {
                ComparisonMethods.Add(new ComparisonMethodInfo(method));
            }
        }

        // Protected methods --------------------------------------------------

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // Public methods -----------------------------------------------------

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

        public ObservableCollection<ComparisonMethodInfo> ComparisonMethods { get; }

        // Public properties --------------------------------------------------

        public ComparisonMethodInfo SelectedComparisonMethod
        {
            get => selectedComparisonMethod;
            set
            {
                selectedComparisonMethod = value;
                OnPropertyChanged(nameof(SelectedComparisonMethod));
                OnPropertyChanged(nameof(Summary));
            }
        }

        public abstract ValidationResult Validate();

        public abstract string Summary { get; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
