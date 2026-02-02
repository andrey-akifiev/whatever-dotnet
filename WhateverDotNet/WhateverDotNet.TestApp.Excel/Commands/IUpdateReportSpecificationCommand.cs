using WhateverDotNet.TestApp.Excel.Models;

namespace WhateverDotNet.TestApp.Excel.Commands;

public interface IUpdateReportSpecificationCommand
{
    Task Execute(ReportSpecificationModel reportSpecificationModel);
}