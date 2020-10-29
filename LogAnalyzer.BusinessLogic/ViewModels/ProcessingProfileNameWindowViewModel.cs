using LogAnalyzer.BusinessLogic.ViewModels.Interfaces;
using LogAnalyzer.Models.DialogResults;
using LogAnalyzer.Models.Views.ProcessingProfileNameWindow;
using LogAnalyzer.Services.Common;
using LogAnalyzer.Services.Interfaces;
using Spooksoft.VisualStateManager.Commands;
using Spooksoft.VisualStateManager.Conditions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LogAnalyzer.BusinessLogic.ViewModels
{
    public class ProcessingProfileNameWindowViewModel : INotifyPropertyChanged
    {
        private readonly IProcessingProfileNameWindowAccess access;
        private readonly IMessagingService messagingService;
        private string note;

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
            Result.DialogResult = true;
            Result.Result = new ProcessingProfileNameResult(note);

            access.Close(true);
        }

        public ProcessingProfileNameWindowViewModel(IProcessingProfileNameWindowAccess access, ProcessingProfileNameModel model, IMessagingService messagingService)
        {
            this.access = access;
            this.messagingService = messagingService;

            OkCommand = new AppCommand((obj) => DoOk());
            CancelCommand = new AppCommand((obj) => DoCancel());

            note = model.Name;

            Result = new ModalDialogResult<ProcessingProfileNameResult>();
        }

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public string Name
        {
            get => note;
            set
            {
                note = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public ModalDialogResult<ProcessingProfileNameResult> Result { get; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
