﻿using Autofac;
using Fluent;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using LogAnalyzer.BusinessLogic.ViewModels;
using LogAnalyzer.BusinessLogic.ViewModels.Interfaces;
using LogAnalyzer.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using System.Xml;

namespace LogAnalyzer.Windows
{
    /// <summary>
    /// Interaction logic for PythonEditorWindow.xaml
    /// </summary>
    public partial class PythonEditorWindow : RibbonWindow, IPythonEditorWindowAccess
    {
        private readonly PythonEditorWindowViewModel viewModel;

        public PythonEditorWindow(IScriptingHost scriptingHost)
        {
            InitializeComponent();

            editor.SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinition("Python");
         
            var pythonHighlightStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("LogAnalyzer.Resources.Highlighting.Python.xshd");
            XmlReader reader = new XmlTextReader(pythonHighlightStream);
            HighlightingManager.Instance.RegisterHighlighting("Python",
                new[] { ".py" },
                HighlightingLoader.Load(reader, HighlightingManager.Instance));

            editor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("Python");

            viewModel = LogAnalyzer.Dependencies.Container.Instance.Resolve<PythonEditorWindowViewModel>(new NamedParameter("access", this),
                new NamedParameter("scriptingHost", scriptingHost));
            DataContext = viewModel;
        }

        public int GetCaretOffset()
        {
            return editor.CaretOffset;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
