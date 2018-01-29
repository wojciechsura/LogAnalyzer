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
            return MessageBox.Show(message, null, MessageBoxButton.YesNo) == MessageBoxResult.Yes;
        }

        public void Inform(string message)
        {
            MessageBox.Show(message);
        }
    }
}
