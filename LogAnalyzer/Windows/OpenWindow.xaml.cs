﻿using FileLogSource.Editor;
using LogAnalyzer.API.LogSource;
using LogAnalyzer.BusinessLogic.ViewModels;
using LogAnalyzer.BusinessLogic.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Unity;
using Unity.Resolution;
using LogAnalyzer.Services.Models;

namespace LogAnalyzer.Windows
{
    /// <summary>
    /// Logika interakcji dla klasy OpenWindow.xaml
    /// </summary>
    public partial class OpenWindow : Window, IOpenWindowAccess
    {
        private OpenWindowViewModel viewModel;

        public OpenWindow()
        {
            InitializeComponent();

            this.viewModel = Dependencies.Container.Instance.Resolve<OpenWindowViewModel>(new ParameterOverride("access", this));
            DataContext = viewModel;
        }

        public ModalDialogResult<OpenResult> DataResult => viewModel.Result;
    }
}
