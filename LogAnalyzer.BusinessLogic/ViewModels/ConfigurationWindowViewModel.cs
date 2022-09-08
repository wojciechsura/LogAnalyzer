using Autofac;
using LogAnalyzer.API.Types;
using LogAnalyzer.BusinessLogic.ViewModels.Configuration;
using LogAnalyzer.BusinessLogic.ViewModels.Interfaces;
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
    public class ConfigurationWindowViewModel : INotifyPropertyChanged
    {
        private readonly IConfigurationWindowAccess access;
        private readonly IMessagingService messagingService;
        private readonly IConfigurationService configurationService;
        private List<BaseConfigurationViewModel> pages;
        private BaseConfigurationViewModel selectedPage;

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
            for (int i = 0; i < pages.Count; i++)
            {
                ValidationResult result = pages[i].Validate();
                if (!result.Valid)
                {
                    messagingService.Warn(result.Message);
                    return;
                }
            }

            configurationService.Configuration.SuspendNotifications();
            try
            {
                for (int i = 0; i < pages.Count; i++)
                {
                    pages[i].Commit();
                }
            }
            finally
            {
                configurationService.Configuration.ResumeNotifications();
            }

            access.Close(true);
        }

        public ConfigurationWindowViewModel(IConfigurationWindowAccess access, IMessagingService messagingService, IConfigurationService configurationService)
        {
            this.access = access;
            this.messagingService = messagingService;
            this.configurationService = configurationService;

            OkCommand = new AppCommand((obj) => DoOk());
            CancelCommand = new AppCommand((obj) => DoCancel());

            pages = new List<BaseConfigurationViewModel>
            {
                LogAnalyzer.Dependencies.Container.Instance.Resolve<OpeningViewModel>()
            };

            selectedPage = pages.FirstOrDefault();
        }

        public List<BaseConfigurationViewModel> Pages => pages;

        public BaseConfigurationViewModel SelectedPage
        {
            get => selectedPage;
            set
            {
                selectedPage = value;
                OnPropertyChanged(nameof(SelectedPage));
            }
        }

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
