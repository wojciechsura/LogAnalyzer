using LogAnalyzer.BusinessLogic.ViewModels.Interfaces;
using LogAnalyzer.Models.DialogResults;
using LogAnalyzer.Models.Views.JsonCodeWindow;
using LogAnalyzer.Services.Common;
using Spooksoft.VisualStateManager.Commands;
using Spooksoft.VisualStateManager.Conditions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.BusinessLogic.ViewModels
{
    public class JsonCodeWindowViewModel : INotifyPropertyChanged
    {
        // Private fields -----------------------------------------------------

        private readonly IJsonCodeWindowAccess access;

        private string code;

        // Private methods ----------------------------------------------------

        private void DoCancel()
        {
            access.Close(false);
        }

        private void DoOk()
        {
            Result.DialogResult = true;
            Result.Result = new JsonCodeResult(code);

            access.Close(true);
        }

        // Protected methods --------------------------------------------------

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // Public methods -----------------------------------------------------

        public JsonCodeWindowViewModel(IJsonCodeWindowAccess access, JsonCodeModel model)
        {
            this.access = access;

            OkCommand = new AppCommand((obj) => DoOk());
            CancelCommand = new AppCommand((obj) => DoCancel());

            code = model.Code;
            Title = model.Title;
            Hint = model.Hint;
            ShowCancel = model.ShowCancel;

            Result = new ModalDialogResult<JsonCodeResult>();
        }

        // Public properties --------------------------------------------------

        public string Code
        {
            get => code;
            set
            {
                code = value;
                OnPropertyChanged(nameof(Code));
            }
        }

        public string Title { get; }
        public string Hint { get; }
        public bool ShowCancel { get; }
        public AppCommand OkCommand { get; }
        public AppCommand CancelCommand { get; }

        public ModalDialogResult<JsonCodeResult> Result { get; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
