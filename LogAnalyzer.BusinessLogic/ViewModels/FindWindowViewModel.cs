using LogAnalyzer.API.Types;
using LogAnalyzer.BusinessLogic.ViewModels.Interfaces;
using LogAnalyzer.BusinessLogic.ViewModels.Processing;
using LogAnalyzer.Models.DialogResults;
using LogAnalyzer.Models.Types;
using LogAnalyzer.Models.Views.FindWindow;
using LogAnalyzer.Services.Common;
using LogAnalyzer.Services.Interfaces;
using LogAnalyzer.Wpf.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LogAnalyzer.BusinessLogic.ViewModels
{
    public class FindWindowViewModel : BaseRuleEditorViewModel
    {
        private readonly IFindWindowAccess access;
        private readonly IMessagingService messagingService;
        private ModalDialogResult<FindResult> result;

        private void DoCancel()
        {
            result.DialogResult = false;
            result.Result = null;
            access.Close(false);            
        }

        private void DoFindNext()
        {
            ValidationResult validationResult = Validate();
            if (!validationResult.Valid)
            {
                messagingService.Inform(validationResult.Message);
                return;
            }

            var findResult = new FindResult
            {
                Action = FindAction.FindNext,
                FindCondition = BuildProcessCondition()
            };
            Result.DialogResult = true;
            Result.Result = findResult;
            access.Close(true);
        }

        public FindWindowViewModel(IFindWindowAccess access, FindModel model, IMessagingService messagingService) 
            : base(model.AvailableCustomColumns)
        {
            result = new ModalDialogResult<FindResult>();

            FindNextCommand = new SimpleCommand((obj) => DoFindNext());
            CancelCommand = new SimpleCommand((obj) => DoCancel());
            this.access = access;
            this.messagingService = messagingService;

            SelectedColumn = AvailableColumns.FirstOrDefault();
        }

        public override ValidationResult Validate()
        {
            if (selectedColumn == null)
                return new ValidationResult(false, "Select field for searching");

            return dataEditorViewModel.Validate();
        }

        public override string Summary => null;

        public ICommand FindNextCommand { get; }
        public ICommand CancelCommand { get; }

        public ModalDialogResult<FindResult> Result => result;
    }
}
