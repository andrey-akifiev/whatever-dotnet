using WhateverDotNet.TestApp.Excel.Models;

namespace WhateverDotNet.TestApp.Excel.Commands;

public interface IRemoveReportSpecificationCommand
{
    Task Execute(ReportSpecificationModel reportSpecificationModel);
}