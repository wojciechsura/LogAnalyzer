using LogAnalyzer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LogAnalyzer.Services
{
    class MessagingService : IMessagingService
    {
        public bool Ask(string message)
        {
            return MessageBox.Show(message, "Log Analyzer", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }

        public void Inform(string message)
        {
            MessageBox.Show(message, "Log Analyzer", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void Warn(string message)
        {
            MessageBox.Show(message, "Log Analyzer", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public void Stop(string message)
        {
            MessageBox.Show(message, "Log Analyzer", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
