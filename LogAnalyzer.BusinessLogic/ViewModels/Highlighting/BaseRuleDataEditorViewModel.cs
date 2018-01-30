using LogAnalyzer.Common.Extensions;
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
        private bool not;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public BaseRuleDataEditorViewModel()
        {
            ComparisonMethods = new ObservableCollection<ComparisonMethodInfo>();
            foreach (ComparisonMethod method in Enum.GetValues(typeof(ComparisonMethod)))
            {
                ComparisonMethods.Add(new ComparisonMethodInfo(method));
            }
            SelectedComparisonMethod = ComparisonMethods.FirstOrDefault();
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
            get
            {
                return selectedComparisonMethod;
            }
            set
            {
                selectedComparisonMethod = value;
                OnPropertyChanged(nameof(SelectedComparisonMethod));
            }
        }
    
        public bool Not
        {
            get => not;
            set
            {
                not = value;
                OnPropertyChanged(nameof(Not));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
