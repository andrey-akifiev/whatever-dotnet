using ClosedXML.Excel;

namespace WhateverDotNet.TestApp.Components.Services;

public class ExcelFileService
{
    ExcelReportDefinition accReport = new()
    {
        Name = "Account Report",
        Columns = new ExcelReportColumnDefinition[]
        {
                new ExcelReportColumnDefinition
                {
                    IsHidden = true,
                    Name = "exposureType",
                    Type = typeof(string),
                },
                new ExcelReportColumnDefinition
                {
                    IsHidden = true,
                    Name = "summaryId",
                    Type = typeof(double),
                },
                new ExcelReportColumnDefinition
                {
                    IsHidden = true,
                    Name = "rank",
                    Type = typeof(double),
                },
                new ExcelReportColumnDefinition
                {
                    IsHidden = true,
                    Name = "targetId",
                    Type = typeof(double),
                },
                new ExcelReportColumnDefinition
                {
                    IsHidden = true,
                    Name = "eventId",
                    Type = typeof(double),
                },
                new ExcelReportColumnDefinition
                {
                    IsHidden = true,
                    Name = "eventName",
                    Type = typeof(double),
                },
                new ExcelReportColumnDefinition
                {
                    Name = "latitude",
                    Type = typeof(double),
                },
                new ExcelReportColumnDefinition
                {
                    Name = "longitude",
                    Type = typeof(double),
                },
                new ExcelReportColumnDefinition
                {
                    Name = "groundUpLoss",
                    Type =  typeof(double),
                },
                new ExcelReportColumnDefinition
                {
                    Name = "grossLoss",
                    Type = typeof(double),
                },
                new ExcelReportColumnDefinition
                {
                    Name = "netLoss",
                    Type = typeof(double),
                },
                new ExcelReportColumnDefinition
                {
                    Name = "totalCasualties",
                    Type = typeof(double),
                },
        },
    };

    public ExcelReportDefinition Load(string filePath, string? worksheetName = null)
    {
        DataSheet dataSheet = LoadDataSheet(filePath, worksheetName);
        ExcelReportDefinition excelReport = Convert(dataSheet);
        return excelReport;
    }

    private DataSheet LoadDataSheet(string filePath, string? dataSheetName = null)
    {
        XLWorkbook workbook = new(filePath);

        try
        {
            IEnumerable<string> worksheets = workbook
                .Worksheets
                .Select(ws => ws.Name);

            IXLWorksheet worksheet =
                !string.IsNullOrEmpty(dataSheetName) && worksheets.Contains(dataSheetName)
                ? workbook.Worksheet(dataSheetName)
                : workbook.Worksheet(worksheets.FirstOrDefault());

            int rowsUsed = worksheet.RowsUsed().Count();

            if (worksheet.RowsUsed().Count() == 0)
            {
                // TODO: Exception: worksheet is empty
            }

            int columnsUsed = worksheet.ColumnsUsed().Count();

            var parsedColumns = new DataSheetColumn[columnsUsed];

            for (int colIdx = 0; colIdx < columnsUsed; colIdx++)
            {
                var parsedColumnCells = new DataSheetCell[rowsUsed - 1];
                for (int rowIdx = 1; rowIdx < rowsUsed; rowIdx++)
                {
                    IXLCell curCell = worksheet.Cell(rowIdx + 1, colIdx + 1);
                    parsedColumnCells[rowIdx - 1] = new DataSheetCell
                    {
                        Type = curCell.DataType.ToString(),
                        Value = curCell.GetString(),
                    };
                }

                parsedColumns[colIdx] = new DataSheetColumn
                {
                    Name = worksheet.Cell(1, colIdx + 1).GetText(),
                    Cells = parsedColumnCells,
                };
            }

            var parsedSheet = new DataSheet
            {
                Name = worksheet.Name,
                Columns = parsedColumns,
                References = workbook.Worksheets
                    .Select(ws => ws.Name)
                    .ToArray(),
            };

            return parsedSheet;
        }
        finally
        {
            workbook.Dispose();
        }
    }

    private ExcelReportDefinition Convert(DataSheet dataSheet)
    {
        ExcelReportDefinition excelReport = new() 
        {
            Name = dataSheet.Name,
            Columns = new ExcelReportColumnDefinition[dataSheet.Columns?.Length ?? 0],
            Worksheets = dataSheet.References,
        };

        for (int colIdx = 0; colIdx < excelReport.Columns.Length; colIdx++)
        {
            DataSheetColumn? dataSheetColumn = dataSheet.Columns?[colIdx];

            IEnumerable<string> supportedTypes = dataSheetTypes.Select(dst => dst.Name);
            string fallbackType = dataSheetColumn
                ?.Cells
                ?.Where(c =>
                    c != null
                    && !string.IsNullOrWhiteSpace(c.Type)
                    && supportedTypes.Contains(c.Type))
                ?.OrderByDescending(c =>
                    dataSheetTypes
                        .First(dst => dst.Name == c.Type)
                        .Priority)
                ?.FirstOrDefault()
                ?.Type
                ?? dataSheetTypes
                    .First()
                    .Name;

            DataSheetType targetType = dataSheetTypes.First(dst => dst.Name == fallbackType);

            excelReport.Columns[colIdx] = new ExcelReportColumnDefinition
            {
                Name = dataSheetColumn?.Name,
                Data = dataSheetColumn
                    ?.Cells
                    ?.Select(c => targetType.Convert(c.Value!))
                    .ToArray(),
                Type = targetType.DotnetType,
            };
        }

        return excelReport;
    }

    private static readonly DataSheetType[] dataSheetTypes = new[]
    {
        new DataSheetType(
            name: "Blank",
            priority: 0,
            dotnetType: typeof(object),
            convertFunc: (_) => null),
        new DataSheetType(
            name: "Number",
            priority: 1,
            dotnetType: typeof(double),
            convertFunc: (val) =>
            {
                return double.TryParse(val?.ToString(), out double result)
                    ? result
                    : null;
            }),
        new DataSheetType(
            name: "String",
            priority: 3,
            dotnetType: typeof(string),
            convertFunc: (val) => val?.ToString()),
        new DataSheetType(
            name: "Text",
            priority: 4,
            dotnetType: typeof(string),
            convertFunc: (val) => val?.ToString()),
    };

    private class DataSheetType
    {
        public DataSheetType(string name, int priority, Type dotnetType, Func<string, object?> convertFunc)
        {
            Name = name;
            Priority = priority;
            DotnetType = dotnetType;
            Convert = convertFunc;
        }

        public string Name { get; }

        public int Priority { get; }

        public Type DotnetType { get; }

        public Func<string, object?> Convert { get; }
    }

    private class DataSheet
    {
        public DataSheetColumn[]? Columns { get; set; }

        public string? Name { get; set; }

        public string?[]? References { get; set; }
    }

    private class DataSheetColumn
    {
        public DataSheetCell[]? Cells { get; set; }
     
        public string? Name { get; set; }
    }

    private class DataSheetCell
    {
        public string? Type { get; set; }
        public string? Value { get; set; }
    }

    public class ExcelReportDefinition
    {
        public ExcelReportColumnDefinition[]? Columns { get; set; }

        public string? Name { get; set; }

        public string?[]? Worksheets { get; set; }
    }

    public class ExcelReportColumnDefinition
    {
        private string? _displayName;

        public bool IsHidden { get; set; } = false;

        public object?[]? Data { get; set; }

        public string? DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(_displayName))
                {
                    return Name;
                }

                return _displayName;
            }

            set => _displayName = value;
        }

        public string? Format { get; set; }

        public string? Name { get; set; }

        public Type? Type { get; set; }
    }
}