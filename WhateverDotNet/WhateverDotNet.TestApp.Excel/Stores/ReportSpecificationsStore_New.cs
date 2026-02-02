using WhateverDotNet.TestApp.Components.Stores;
using WhateverDotNet.TestApp.Excel.Models;

namespace WhateverDotNet.TestApp.Excel.Stores;

public class ReportSpecificationsStore_New
    : BaseModelStore<ReportSpecificationModel>
{
    protected override Task<IEnumerable<ReportSpecificationModel>> LoadAsyncImpl(CancellationToken cancellationToken)
    {
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
                    }
                ],
            },
            new ReportSpecificationModel
            {
                Name = "Sample Report 2",
                Description = "This is a sample report specification.",
                Worksheets = Array.Empty<ReportSpecificationWorksheetModel>(),
            }
        };

        return Task.FromResult(loadedSpecifications);
    }

    protected override Task SaveAsyncImpl(IReadOnlyCollection<ReportSpecificationModel> items, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}