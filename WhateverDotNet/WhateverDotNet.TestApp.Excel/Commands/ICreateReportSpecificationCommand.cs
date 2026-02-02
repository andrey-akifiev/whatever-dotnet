using WhateverDotNet.TestApp.Excel.Models;

namespace WhateverDotNet.TestApp.Excel.Commands;

public interface ICreateReportSpecificationCommand
{
    Task Execute(ReportSpecificationModel reportSpecification);
}