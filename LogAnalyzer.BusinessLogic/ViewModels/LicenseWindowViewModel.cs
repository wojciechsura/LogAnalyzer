using LogAnalyzer.BusinessLogic.ViewModels.Interfaces;
using LogAnalyzer.Models.Services.WinApiService;
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
    public class LicenseWindowViewModel : INotifyPropertyChanged
    {
        private readonly ILicenseWindowAccess access;
        private readonly ILicenseService licenseService;
        private readonly IMessagingService messagingService;
        private readonly IWinApiService winApiService;

        private string licenseType;
        private string username;
        private string expirationDate;

        private void DoOk()
        {
            access.Close();
        }

        private void DoRemove()
        {
            if (messagingService.Ask("Are you sure you want to remove your license?") == true)
            {
                licenseService.Remove();
                UpdateLicenseData();
            }
        }

        private void DoRegister()
        {
            var filename = winApiService.OpenFile(new[] { new FilterDefinition("*.license", "License files (*.license)") });
            if (filename != null)
            {
                if (licenseService.Install(filename))
                {
                    UpdateLicenseData();
                    messagingService.Inform("License has been successfully installed.");
                }
                else
                {
                    messagingService.Warn("Failed to install license!");
                }
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public LicenseWindowViewModel(ILicenseWindowAccess access, ILicenseService licenseService, IMessagingService messagingService, IWinApiService winApiService)
        {
            this.access = access;
            this.licenseService = licenseService;
            this.messagingService = messagingService;
            this.winApiService = winApiService;

            UpdateLicenseData();

            RegisterCommand = new SimpleCommand((obj) => DoRegister());
            RemoveCommand = new SimpleCommand((obj) => DoRemove());
            OkCommand = new SimpleCommand((obj) => DoOk());
        }

        private void UpdateLicenseData()
        {
            LicenseType = licenseService.Licensed ? (licenseService.Expires == true ? "Timed" : "Full") : "(none)";
            Username = licenseService.Username ?? "";
            ExpirationDate = licenseService.Licensed ? (licenseService.Expires == true ? licenseService.ExpirationDate?.ToString("dd-MM-yyyy") ?? "" : "(none)") : "";
        }

        public string LicenseType
        {
            get => licenseType;
            set
            {
                licenseType = value;
                OnPropertyChanged(nameof(LicenseType));
            }
        }

        public string Username
        {
            get => username;
            set
            {
                username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public string ExpirationDate
        {
            get => expirationDate;
            set
            {
                expirationDate = value;
                OnPropertyChanged(nameof(ExpirationDate));
            }
        }

        public ICommand RegisterCommand { get; }
        public ICommand RemoveCommand { get; }
        public ICommand OkCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
