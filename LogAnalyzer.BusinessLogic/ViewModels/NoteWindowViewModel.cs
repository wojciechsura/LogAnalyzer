using LogAnalyzer.BusinessLogic.ViewModels.Interfaces;
using LogAnalyzer.Models.DialogResults;
using LogAnalyzer.Models.Views.NoteWindow;
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
    public class NoteWindowViewModel : INotifyPropertyChanged
    {
        private readonly INoteWindowAccess access;
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
            Result.Result = new NoteResult(note);

            access.Close(true);
        }

        public NoteWindowViewModel(INoteWindowAccess access, NoteModel model, IMessagingService messagingService)
        {
            this.access = access;
            this.messagingService = messagingService;

            OkCommand = new AppCommand((obj) => DoOk());
            CancelCommand = new AppCommand((obj) => DoCancel());

            note = model.Note;

            Result = new ModalDialogResult<NoteResult>();
        }

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public string Note
        {
            get => note;
            set
            {
                note = value;
                OnPropertyChanged(nameof(Note));
            }
        }

        public ModalDialogResult<NoteResult> Result { get; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
