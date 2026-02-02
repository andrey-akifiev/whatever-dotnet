using WhateverDotNet.TestApp.Excel.Commands;
using WhateverDotNet.TestApp.Excel.Models;

namespace WhateverDotNet.TestApp.Excel.Stores;

public class ReportSpecificationsStore
{
    private readonly ICreateReportSpecificationCommand _createReportSpecificationCommand;
    private readonly IUpdateReportSpecificationCommand _updateReportSpecificationCommand;
    private readonly IRemoveReportSpecificationCommand _removeReportSpecificationCommand;

    private readonly List<ReportSpecificationModel> _reports;

    public event Action ReportsLoaded;

    public event Action<ReportSpecificationModel> ReportCreated;

    public event Action<ReportSpecificationModel> ReportUpdated;

    public event Action<ReportSpecificationModel> ReportRemoved;

    public ReportSpecificationsStore(
        ICreateReportSpecificationCommand createReportSpecificationCommand,
        IUpdateReportSpecificationCommand updateReportSpecificationCommand,
        IRemoveReportSpecificationCommand removeReportSpecificationCommand)
    {
        _createReportSpecificationCommand = createReportSpecificationCommand
            ?? throw new ArgumentNullException(nameof(createReportSpecificationCommand));
        _updateReportSpecificationCommand = updateReportSpecificationCommand
            ?? throw new ArgumentNullException(nameof(updateReportSpecificationCommand));
        _removeReportSpecificationCommand = removeReportSpecificationCommand
            ?? throw new ArgumentNullException(nameof(removeReportSpecificationCommand));

        _reports = new List<ReportSpecificationModel>();
    }

    public IEnumerable<ReportSpecificationModel> Reports => _reports;

    public async Task Create(ReportSpecificationModel reportSpecification)
    {
        await _createReportSpecificationCommand.Execute(reportSpecification);

        _reports.Add(reportSpecification);

        ReportCreated?.Invoke(reportSpecification);
    }

    public async Task Load()
    {
        // TODO: = await ReportSpecificationsDataProvider.LoadReportSpecificationsAsync();
        IEnumerable<ReportSpecificationModel> loadedSpecifications = new ReportSpecificationModel[]
        {
            new ReportSpecificationModel
            {
                Name = "Sample Report 1",
                Description = "This is a sample report specification.",
                Worksheets =
                [
                    new ReportSpecificationWorksheetModel
                    {
                        Name = "Sheet PewPew",
                        Columns = new ReportSpecificationColumnModel[]
                        {
                            new ReportSpecificationColumnModel
                            {
                                Name = "Column 1",
                                DisplayName = "Display Column 1",
                                Type = ReportSpecificationColumnTypes.String,
                            },
                            new ReportSpecificationColumnModel
                            {
                                Name = "Column 2",
                                DisplayName = "Display Column 2",
                                Type = ReportSpecificationColumnTypes.Number,
                            },
                        }
                    },
                    new ReportSpecificationWorksheetModel
                    {
                        Name = "Sheet Ololol",
                    },
                ],
            },
            new ReportSpecificationModel
            {
                Name = "Sample Report 2",
                Description = "This is a sample report specification.",
                Worksheets = Array.Empty<ReportSpecificationWorksheetModel>(),
            }
        };

        _reports.Clear();
        _reports.AddRange(loadedSpecifications);

        ReportsLoaded?.Invoke();
    }

    public async Task Remove(ReportSpecificationModel reportSpecificationModel)
    {
        await _removeReportSpecificationCommand.Execute(reportSpecificationModel);

        _reports.RemoveAll(r => r.Id == reportSpecificationModel.Id);

        ReportRemoved?.Invoke(reportSpecificationModel);
    }

    public async Task Update(ReportSpecificationModel reportSpecificationModel)
    {
        await _updateReportSpecificationCommand.Execute(reportSpecificationModel);

        int currentIndex = _reports.FindIndex(r => r.Id == reportSpecificationModel.Id);

        if (currentIndex != -1)
        {
            _reports[currentIndex] = reportSpecificationModel;
        }
        else
        {
            _reports.Add(reportSpecificationModel);
        }

        ReportUpdated?.Invoke(reportSpecificationModel);
    }
}