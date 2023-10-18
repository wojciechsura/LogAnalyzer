using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLogParser.Editor
{
    public class FieldViewModel : INotifyPropertyChanged
    {
        private string field;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public string Field
        {
            get => field;
            set
            {
                field = value;
                OnPropertyChanged(nameof(Field));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
