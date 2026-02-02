using CommunityToolkit.Mvvm.Input;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;

namespace WhateverDotNet.TestApp.Components.ViewModels;

public abstract class BaseGridViewModel<TRowViewModel, TRowModel>
    : BaseViewModel, IDisposable
        where TRowViewModel : BaseRowViewModel<TRowModel>
{
    private List<TRowModel> _originalModels = new();

    protected BaseGridViewModel(ObservableCollection<TRowViewModel> rows)
    {
        Rows = rows;
        Rows.CollectionChanged += OnRowsChanged;

        AddRowCommand = new RelayCommand(AddRow);
    }

    public abstract void AddRange(IEnumerable<TRowModel> models);

    public ICommand AddRowCommand { get; }

    public bool IsDirty =>
        Rows.Count != _originalModels.Count
        || Rows.Any(r => r.IsDirty);

    public ObservableCollection<TRowViewModel> Rows { get; }

    public override void Dispose()
    {
        Rows.CollectionChanged -= OnRowsChanged;

        // TODO: Is it really needed here? -- aa
        UnsubscribeRowsPropertyChanged(Rows);

        base.Dispose();
    }

    protected abstract TRowModel CloneModel(TRowModel model);

    protected abstract TRowViewModel CreateNewRow();

    protected abstract TRowViewModel CreateNewRow(TRowModel model);

    protected void AddRow()
    {
        Rows.Add(CreateNewRow());
        OnPropertyChanged(nameof(IsDirty));
    }

    public void CloneRow(TRowViewModel row)
    {
        var cloneModel = CloneModel(row.Current);
        var clonedRow = CreateNewRow(cloneModel);

        var index = Rows.IndexOf(row);
        Rows.Insert(index + 1, clonedRow);

        OnPropertyChanged(nameof(IsDirty));
        
    }

    public void DeleteRow(TRowViewModel row)
    {
        Rows.Remove(row);
        OnPropertyChanged(nameof(IsDirty));
    }

    private void OnRowsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            SubscribeRowsPropertyChanged(e.NewItems);
        }

        if (e.OldItems != null)
        {
            UnsubscribeRowsPropertyChanged(e.OldItems);
        }

        OnPropertyChanged(nameof(IsDirty));
    }

    private void OnRowPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(DirtyTrackingViewModel<TRowViewModel>.IsDirty))
        {
            OnPropertyChanged(nameof(IsDirty));
        }
    }

    private void SubscribeRowPropertyChanged(object columnViewModel) =>
        (columnViewModel as TRowViewModel)
            ?.PropertyChanged += OnRowPropertyChanged;

    private void SubscribeRowsPropertyChanged(System.Collections.IList columnViewModels)
    {
        foreach (var columnViewModel in columnViewModels)
        {
            SubscribeRowPropertyChanged(columnViewModel);
        }
    }

    private void UnsubscribeRowPropertyChanged(object columnViewModel) =>
        (columnViewModel as TRowViewModel)
            ?.PropertyChanged -= OnRowPropertyChanged;

    private void UnsubscribeRowsPropertyChanged(System.Collections.IList columnViewModels)
    {
        foreach (var columnViewModel in columnViewModels)
        {
            UnsubscribeRowPropertyChanged(columnViewModel);
        }
    }
}