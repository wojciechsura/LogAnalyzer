using Fluent;
using LogAnalyzer.BusinessLogic.ViewModels;
using LogAnalyzer.BusinessLogic.ViewModels.Interfaces;
using Unity;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Unity.Resolution;
using LogAnalyzer.API.Models;
using LogAnalyzer.Common.Tools;
using System.Reflection;
using System.Windows.Markup;
using System.Xml;
using System.IO;

namespace LogAnalyzer.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow, IMainWindowAccess
    {
        private MainWindowViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();

            viewModel = Dependencies.Container.Instance.Resolve<MainWindowViewModel>(new ParameterOverride("access", this));
            DataContext = viewModel;
        }

        public void ClearListView()
        {
            lvMain.View = null;
        }

        private void CreateColumn(string commonColumnXaml, GridView gridView, BaseColumnInfo commonColumn)
        {
            string columnXaml = String.Format(commonColumnXaml, commonColumn.Member);
            DataTemplate dataTemplate = (DataTemplate)XamlReader.Load(XmlReader.Create(new StringReader(columnXaml)));

            GridViewColumn column = new GridViewColumn
            {
                Header = commonColumn.Header,
                CellTemplate = dataTemplate
            };

            gridView.Columns.Add(column);
        }

        private void CreateMarginColumn(GridView gridView, DataTemplate dataTemplate)
        {
            GridViewColumn marginColumn = new GridViewColumn
            {
                Header = "",
                Width = 48,
                CellTemplate = dataTemplate
            };
            gridView.Columns.Add(marginColumn);
        }

        private void CreateLineNumberColumn(GridView gridView, DataTemplate dataTemplate)
        {
            GridViewColumn lineNumberColumn = new GridViewColumn
            {
                Header = "Index",
                Width = 64,
                CellTemplate = dataTemplate
            };
            gridView.Columns.Add(lineNumberColumn);
        }

        private void HandleWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = viewModel.HandleClosing();
        }

        private void SearchResultsDoubleClick(object sender, MouseButtonEventArgs e)
        {
            viewModel.OnSearchResultChosen();
        }

        public void SetupListView(ListView listView, List<BaseColumnInfo> columns)
        {
            listView.View = null;

            string commonColumnXaml = ResourceReader.ReadEmbeddedResource(Assembly.GetExecutingAssembly(), "LogAnalyzer.Resources.Xaml.RegularColumn.xaml");
            string marginColumnXaml = ResourceReader.ReadEmbeddedResource(Assembly.GetExecutingAssembly(), "LogAnalyzer.Resources.Xaml.MarginColumn.xaml");

            GridView gridView = new GridView();

            // Margin
            string marginXaml = marginColumnXaml;
            DataTemplate marginColumnDataTemplate = (DataTemplate)XamlReader.Load(XmlReader.Create(new StringReader(marginXaml)));
            CreateMarginColumn(gridView, marginColumnDataTemplate);

            // Line number
            string columnXaml = String.Format(commonColumnXaml, $"{nameof(LogRecord.LogEntry)}.{nameof(LogEntry.Index)}");
            DataTemplate indexColumnDataTemplate = (DataTemplate)XamlReader.Load(XmlReader.Create(new StringReader(columnXaml)));
            CreateLineNumberColumn(gridView, indexColumnDataTemplate);

            // Other columns
            for (int i = 0; i < columns.Count; i++)
            {
                CreateColumn(commonColumnXaml, gridView, columns[i]);
            }

            listView.View = gridView;
        }

        public void SetupListViews(List<BaseColumnInfo> columns)
        {
            SetupListView(lvMain, columns);
            SetupListView(lvSearchResults, columns);
        }

        public void NavigateTo(LogRecord selectedSearchResult)
        {
            lvMain.SelectedItem = selectedSearchResult;
            lvMain.ScrollIntoView(selectedSearchResult);
        }


        private void RibbonWindow_Drop(object sender, DragEventArgs e)
        {
            // Deferring invoke to avoid hanging source
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                List<string> filePaths = new List<string>(files);

                this.Dispatcher.BeginInvoke(new Action(() => HandleFilesDropped(filePaths)));                
            }
        }

        private void HandleFilesDropped(List<string> filePaths)
        {
            viewModel.FilesDropped(filePaths);
        }

        private void RibbonWindow_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.All;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        public System.Collections.IList GetMainSelectedItems()
        {
            if (lvMain.SelectedItems != null)
                return lvMain.SelectedItems;
            else if (lvMain.SelectedItem != null)
                return new List<object> { lvMain.SelectedItem };
            else
                return new List<object>();
        }
    }
}
