using LogAnalyzer.API.Types;
using LogAnalyzer.BusinessLogic.ViewModels.Interfaces;
using LogAnalyzer.BusinessLogic.ViewModels.Processing;
using LogAnalyzer.Models.DialogResults;
using LogAnalyzer.Models.Engine;
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
        private ModalDialogResult<SearchConfig> result;

        private void DoCancel()
        {
            result.DialogResult = false;
            result.Result = null;
            access.Close(false);            
        }

        private void DoFindAll()
        {
            ValidationResult validationResult = Validate();
            if (!validationResult.Valid)
            {
                messagingService.Inform(validationResult.Message);
                return;
            }

            var findResult = new SearchConfig
            {
                PredicateDescription = BuildProcessCondition()
            };
            Result.DialogResult = true;
            Result.Result = findResult;
            access.Close(true);
        }

        public FindWindowViewModel(IFindWindowAccess access, FindModel model, IMessagingService messagingService) 
            : base(model.AvailableCustomColumns)
        {
            result = new ModalDialogResult<SearchConfig>();

            FindAllCommand = new SimpleCommand((obj) => DoFindAll());
            CancelCommand = new SimpleCommand((obj) => DoCancel());
            this.access = access;
            this.messagingService = messagingService;

            // Select "Message contains" by default
            SelectedColumn = AvailableColumns.FirstOrDefault(c => c.Column == LogEntryColumn.Message);
            dataEditorViewModel.SelectedComparisonMethod = dataEditorViewModel.ComparisonMethods.FirstOrDefault(m => m.ComparisonMethod == ComparisonMethod.Contains);

            if (model.SearchConfig != null)
                RestoreDataEditorViewModel(model.SearchConfig.PredicateDescription);
        }

        public override ValidationResult Validate()
        {
            if (selectedColumn == null)
                return new ValidationResult(false, "Select field for searching");

            return dataEditorViewModel.Validate();
        }

        public override string Summary => null;

        public ICommand FindAllCommand { get; }
        public ICommand CancelCommand { get; }

        public ModalDialogResult<SearchConfig> Result => result;
    }
}
