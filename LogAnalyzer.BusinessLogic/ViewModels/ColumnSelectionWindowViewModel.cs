using LogAnalyzer.API.Models;
using LogAnalyzer.BusinessLogic.ViewModels.Interfaces;
using LogAnalyzer.Models.DialogResults;
using LogAnalyzer.Models.Views.ColumnSelectionWindow;
using LogAnalyzer.Services.Common;
using LogAnalyzer.Services.Interfaces;
using Spooksoft.VisualStateManager.Commands;
using Spooksoft.VisualStateManager.Conditions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LogAnalyzer.BusinessLogic.ViewModels
{
    public class ColumnSelectionWindowViewModel : INotifyPropertyChanged
    {
        public class ColumnInfo
        {
            public ColumnInfo(BaseColumnInfo column)
            {
                Display = column.Header;
                Column = column;
            }

            public string Display { get; }
            public BaseColumnInfo Column { get; }
        }

        private readonly IColumnSelectionWindowAccess access;
        private readonly IMessagingService messagingService;
        private ObservableCollection<ColumnInfo> columns;
        private ColumnInfo selectedColumn;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void DoCancel()
        {
            access.Close(false);
        }

        private void DoOk()
        {
            if (selectedColumn == null)
            {
                messagingService.Warn("Please select column!");
                return;
            }

            Result.DialogResult = true;
            Result.Result = new ColumnSelectionResult(selectedColumn.Column);

            access.Close(true);
        }

        public ColumnSelectionWindowViewModel(IColumnSelectionWindowAccess access, ColumnSelectionModel model, IMessagingService messagingService)
        {
            this.access = access;
            this.messagingService = messagingService;

            columns = new ObservableCollection<ColumnInfo>();
            for (int i = 0; i < model.CurrentColumns.Count; i++)
            {
                columns.Add(new ColumnInfo(model.CurrentColumns[i]));
            }
            selectedColumn = columns.FirstOrDefault();

            OkCommand = new AppCommand((obj) => DoOk());
            CancelCommand = new AppCommand((obj) => DoCancel());

            Result = new ModalDialogResult<ColumnSelectionResult>();
        }

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public ObservableCollection<ColumnInfo> Columns => columns;

        public ColumnInfo SelectedColumn
        {
            get => selectedColumn;
            set
            {
                selectedColumn = value;
                OnPropertyChanged(nameof(SelectedColumn));
            }
        }

        public ModalDialogResult<ColumnSelectionResult> Result { get; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
