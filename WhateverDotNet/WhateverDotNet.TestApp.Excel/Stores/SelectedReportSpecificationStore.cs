using WhateverDotNet.TestApp.Excel.Models;

namespace WhateverDotNet.TestApp.Excel.Stores;

public class SelectedReportSpecificationStore : IDisposable
{
    public event Action? SelectedReportChanged;

    private readonly ReportSpecificationsStore _reportsStore;

    private ReportSpecificationModel? _selectedReport;

    public SelectedReportSpecificationStore(
        ReportSpecificationsStore reportsStore)
    {
        _reportsStore = reportsStore
            ?? throw new ArgumentNullException(nameof(reportsStore));

        _reportsStore.ReportCreated += ReportsStore_ReportSpecificationCreated;
        _reportsStore.ReportUpdated += ReportsStore_ReportSpecificationUpdated;
    }

    public ReportSpecificationModel? SelectedReport
    {
        get => _selectedReport;
        set
        {
            _selectedReport = value;
            SelectedReportChanged?.Invoke();
        }
    }

    public void Dispose()
    {
        _reportsStore.ReportCreated -= ReportsStore_ReportSpecificationCreated;
        _reportsStore.ReportUpdated -= ReportsStore_ReportSpecificationUpdated;
    }

    private void ReportsStore_ReportSpecificationCreated(ReportSpecificationModel report)
    {
        SelectedReport = report;
    }

    private void ReportsStore_ReportSpecificationUpdated(ReportSpecificationModel report)
    {
        if (SelectedReport?.Id == report.Id)
        {
            SelectedReport = report;
        }
    }
}