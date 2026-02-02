using CommunityToolkit.Mvvm.Input;

using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;

using WhateverDotNet.TestApp.Components.Services;
using WhateverDotNet.TestApp.Components.ViewModels;

namespace WhateverDotNet.TestApp.Components.Controls.ViewModels;

public class ExcelReportViewModel : BaseViewModel
{
    private string? _filePath = null;
    private bool _isLoading = false;
    private string? _lastOpenedDirectory = null;
    private string? _selectedWorksheet = null;
    private IEnumerable<string> _worksheets = Enumerable.Empty<string>();

    public ExcelReportViewModel()
    {
        BrowseFileCommand = new RelayCommand(OnBrowseFileCommand);
        Worksheets = new ObservableCollection<string>();
    }

    public string BrowseFileCopyTemplate => "Test data file path: '{0}'.";

    public ICommand BrowseFileCommand { get; }

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnPropertyChanged();
        }
    }

    public string? FilePath
    {
        get => _filePath;
        set
        {
            _filePath = value;
            OnPropertyChanged();
        }
    }

    public string? SelectedWorksheet
    {
        get => _selectedWorksheet;
        set
        {
            _selectedWorksheet = value;
            OnPropertyChanged();
        }
    }

    public string SelectedWorksheetCopyTemplate => "Worksheet: '{0}'.";

    public IEnumerable<string> Worksheets
    {
        get => _worksheets;
        set
        {
            _worksheets = value;
            OnPropertyChanged();
        }
    }

    private void OnBrowseFileCommand()
    {
        var dlg = new Microsoft.Win32.OpenFileDialog
        {
            DefaultExt = ".xlsx",
            Filter = "Excel Files (*.xlsx;*.xls)|*.xlsx;*.xls",
            InitialDirectory = _lastOpenedDirectory,
        };

        if (dlg.ShowDialog() != true)
        {
            return;
        }

        string filePath = dlg.FileName;

        _lastOpenedDirectory =
            Path.GetDirectoryName(dlg.FileName)
                ?? string.Empty;

        FilePath = filePath;
        LoadWorksheets(filePath);
    }

    private void LoadWorksheets(string filePath)
    {
        IsLoading = true;

        try
        {
            ExcelFileService excelFileService = new();
            var excelReport = excelFileService.Load(filePath);

            Worksheets = excelReport.Worksheets;
            SelectedWorksheet = excelReport.Name;
        }
        finally
        {
            //IsLoading = false;
        }
    }
}