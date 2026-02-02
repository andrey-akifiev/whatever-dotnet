using System.Collections.ObjectModel;
using System.Collections.Specialized;
using WhateverDotNet.TestApp.Components.ViewModels;
using WhateverDotNet.TestApp.Excel.Models;
using WhateverDotNet.TestApp.Excel.Stores;

namespace WhateverDotNet.TestApp.Excel.ViewModels;

public class ReportSpecificationsListingViewModel : BaseViewModel, IDisposable
{
    private readonly ReportSpecificationsStore _reportsStore;
    private readonly SelectedReportSpecificationStore _selectedReportStore;

    private readonly ObservableCollection<ReportSpecificationsListingItemViewModel> _reportsViewModels;

    public IEnumerable<ReportSpecificationsListingItemViewModel> ReportsViewModels => _reportsViewModels;

    public ReportSpecificationsListingItemViewModel? SelectedReportViewModel
    {
        get => _reportsViewModels
                    .FirstOrDefault(li => 
                        li.Report?.Id == _selectedReportStore.SelectedReport?.Id);
        set => _selectedReportStore.SelectedReport = value?.Report;
    }

    public ReportSpecificationsListingViewModel(
        ReportSpecificationsStore reportsStore,
        SelectedReportSpecificationStore selectedReportStore)
    {
        _reportsStore = reportsStore
            ?? throw new ArgumentNullException(nameof(reportsStore));
        _selectedReportStore = selectedReportStore
            ?? throw new ArgumentNullException(nameof(selectedReportStore));

        _reportsViewModels = new ObservableCollection<ReportSpecificationsListingItemViewModel>();

        _selectedReportStore.SelectedReportChanged += SelectedReportStore_SelectedReportChanged;

        _reportsStore.ReportCreated += ReportsStore_ReportCreated;
        _reportsStore.ReportsLoaded += ReportsStore_ReportsLoaded;
        _reportsStore.ReportRemoved += ReportsStore_ReportRemoved;
        _reportsStore.ReportUpdated += ReportsStore_ReportUpdated;

        _reportsViewModels.CollectionChanged += ListingItemViewModels_CollectionChanged;
    }

    public override void Dispose()
    {
        _selectedReportStore.SelectedReportChanged -= SelectedReportStore_SelectedReportChanged;

        _reportsStore.ReportCreated -= ReportsStore_ReportCreated;
        _reportsStore.ReportsLoaded -= ReportsStore_ReportsLoaded;
        _reportsStore.ReportRemoved -= ReportsStore_ReportRemoved;
        _reportsStore.ReportUpdated -= ReportsStore_ReportUpdated;

        base.Dispose();
    }

    private void AddReport(ReportSpecificationModel report)
    {
        ReportSpecificationsListingItemViewModel reportViewModel = new (report);
        _reportsViewModels.Add(reportViewModel);
    }

    private void ListingItemViewModels_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(ReportsViewModels));
    }

    private void ReportsStore_ReportCreated(ReportSpecificationModel report)
    {
        AddReport(report);
    }

    private void ReportsStore_ReportsLoaded()
    {
        _reportsViewModels.Clear();

        foreach (ReportSpecificationModel reportSpecification in _reportsStore.Reports)
        {
            ReportSpecificationsListingItemViewModel itemViewModel = new ReportSpecificationsListingItemViewModel(reportSpecification);
            _reportsViewModels.Add(itemViewModel);
        }
    }

    private void ReportsStore_ReportRemoved(ReportSpecificationModel report)
    {
        ReportSpecificationsListingItemViewModel? reportViewModel = _reportsViewModels
            .FirstOrDefault(rvm => rvm.Report?.Id == report.Id);

        if (reportViewModel != null)
        {
            _reportsViewModels.Remove(reportViewModel);
        }
    }

    private void ReportsStore_ReportUpdated(ReportSpecificationModel report)
    {
        ReportSpecificationsListingItemViewModel? reportViewModel = _reportsViewModels
            .FirstOrDefault(rvm => rvm.Report?.Id == report.Id);

        if (reportViewModel != null)
        {
            reportViewModel.Update(report);
        }
    }

    private void SelectedReportStore_SelectedReportChanged()
    {
        OnPropertyChanged(nameof(SelectedReportViewModel));
    }
}