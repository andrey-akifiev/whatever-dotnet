using WhateverDotNet.TestApp.Components.Commands;
using WhateverDotNet.TestApp.Excel.Models;

namespace WhateverDotNet.TestApp.Excel.Commands;

public class CreateReportSpecificationCommand : BaseAsyncCommand, ICreateReportSpecificationCommand
{
    public Task Execute(ReportSpecificationModel reportSpecification)
    {
        throw new NotImplementedException();
    }

    public override Task ExecuteAsync(object parameter)
    {
        throw new NotImplementedException();
    }
}