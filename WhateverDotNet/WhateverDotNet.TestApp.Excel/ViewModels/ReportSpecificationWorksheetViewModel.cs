using CommunityToolkit.Mvvm.Input;

using System.Windows.Input;

using WhateverDotNet.TestApp.Components.ViewModels;
using WhateverDotNet.TestApp.Excel.Models;

namespace WhateverDotNet.TestApp.Excel.ViewModels;

public class ReportSpecificationWorksheetViewModel : BaseViewModel
{
    private readonly ReportSpecificationWorksheetModel _originalValue;

    private bool _isDirty;
    private bool _isExpanded;
    private string _title;

    public ReportSpecificationWorksheetViewModel(ReportSpecificationWorksheetModel worksheet)
    {
        _originalValue = worksheet;
        _title = _originalValue.Name ?? "Unknown";

        IsDirty = false;
        IsExpanded = true;

        ColumnsViewModel = new ReportSpecificationColumnsDetailsViewModel();

        if (worksheet?.Columns != null)
        {
            ColumnsViewModel.AddRange(worksheet.Columns);
        }

        AddColumnCommand = new RelayCommand(AddColumn);
    }

    public ReportSpecificationColumnsDetailsViewModel ColumnsViewModel { get; private set; }

    public bool IsDirty
    {
        get => _isDirty;
        set
        {
            _isDirty = value;
            OnPropertyChanged();
        }
    }

    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            _isExpanded = value;
            OnPropertyChanged();
        }
    }

    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            IsDirty = true;
            OnPropertyChanged(nameof(IsDirty));
            OnPropertyChanged(nameof(Title));
        }
    }

    public ICommand AddColumnCommand { get; }

    public ICommand DeleteColumnCommand { get; }

    private void AddColumn()
    {
        
        IsDirty = true;
    }
}