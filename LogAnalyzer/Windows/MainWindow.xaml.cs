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

        private void CreateLineNumberColumn(GridView gridView, DataTemplate dataTemplate)
        {
            GridViewColumn lineNumberColumn = new GridViewColumn
            {
                Header = "Line",
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

            GridView gridView = new GridView();

            // Line number
            string columnXaml = String.Format(commonColumnXaml, nameof(FilteredLogEntry.LogEntryIndex));
            DataTemplate dataTemplate = (DataTemplate)XamlReader.Load(XmlReader.Create(new StringReader(columnXaml)));

            CreateLineNumberColumn(gridView, dataTemplate);

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

        public void NavigateTo(HighlightedLogEntry selectedSearchResult)
        {
            lvMain.SelectedItem = selectedSearchResult;
            lvMain.ScrollIntoView(selectedSearchResult);
        }
    }
}
