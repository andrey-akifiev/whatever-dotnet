using WhateverDotNet.TestApp.Components.Commands;
using WhateverDotNet.TestApp.Excel.Models;

namespace WhateverDotNet.TestApp.Excel.Commands;

public class UpdateReportSpecificationCommand : BaseAsyncCommand, IUpdateReportSpecificationCommand
{
    public Task Execute(ReportSpecificationModel reportSpecificationModel)
    {
        throw new NotImplementedException();
    }

    public override Task ExecuteAsync(object parameter)
    {
        throw new NotImplementedException();
    }
}