using ICSharpCode.AvalonEdit.Document;
using LogAnalyzer.BusinessLogic.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.BusinessLogic.ViewModels
{
    public class PythonEditorWindowViewModel : INotifyPropertyChanged
    {
        private readonly IPythonEditorWindowAccess access;
        private readonly TextDocument document;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public PythonEditorWindowViewModel(IPythonEditorWindowAccess access)
        {
            this.access = access;

            this.document = new TextDocument();
        }

        public TextDocument Document => document;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
