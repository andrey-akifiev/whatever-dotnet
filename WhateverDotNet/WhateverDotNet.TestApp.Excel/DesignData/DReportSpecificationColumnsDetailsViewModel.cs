using WhateverDotNet.TestApp.Excel.Models;
using WhateverDotNet.TestApp.Excel.ViewModels;

namespace WhateverDotNet.TestApp.Excel.DesignData;

internal class DReportSpecificationColumnsDetailsViewModel
    : ReportSpecificationColumnsDetailsViewModel
{
    public DReportSpecificationColumnsDetailsViewModel()
        : base()
    {
        this.AddRange(new[]
            {
                new ReportSpecificationColumnModel
                {
                    Name = "firstColumn",
                    DisplayName = "First Column",
                    Format = "C",
                    IsComparable = false,
                    IsValuable = false,
                    Type = ReportSpecificationColumnTypes.String,
                },
                new ReportSpecificationColumnModel
                {
                    Name = "secondColumn",
                    DisplayName = "Second Column",
                    Format = "C",
                    IsComparable = false,
                    IsValuable = true,
                    Type = ReportSpecificationColumnTypes.Number,
                },
                new ReportSpecificationColumnModel
                {
                    Name = "thirdColumn",
                    DisplayName = "Third Column",
                    Format = "C",
                    IsComparable = true,
                    IsValuable = true,
                    Type = ReportSpecificationColumnTypes.Number,
                },
            });
    }
}