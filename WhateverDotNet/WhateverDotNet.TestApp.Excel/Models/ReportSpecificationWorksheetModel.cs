using WhateverDotNet.TestApp.Components.Models;

namespace WhateverDotNet.TestApp.Excel.Models;

public class ReportSpecificationWorksheetModel
    : ICloneable,
        IEquatable<ReportSpecificationWorksheetModel>,
        ISampleProvider<ReportSpecificationWorksheetModel>
{
    public string? Name { get; set; }

    public ReportSpecificationColumnModel[]? Columns { get; set; }

    public static ReportSpecificationWorksheetModel CreateSample()
    {
        return new ReportSpecificationWorksheetModel
        {
            Name = "New Worksheet",
            Columns = 
                [
                    ReportSpecificationColumnModel.CreateSample(),
                    ReportSpecificationColumnModel.CreateSample(),
                ],
        };
    }

    public object Clone()
    {
        ReportSpecificationColumnModel[]? clonedColumns = null;

        if (Columns != null)
        {
            clonedColumns = new ReportSpecificationColumnModel[Columns.Length];
            for (int idx = 0; idx < Columns.Length; idx++)
            {
                clonedColumns[idx] = (ReportSpecificationColumnModel)Columns[idx].Clone();
            }
        }

        return new ReportSpecificationWorksheetModel
        {
            Name = Name,
            Columns = clonedColumns,
        };
    }

    public override bool Equals(object? obj) =>
        Equals(obj as ReportSpecificationWorksheetModel);

    public bool Equals(ReportSpecificationWorksheetModel? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        if (!string.Equals(Name, other.Name))
            return false;
        if (Columns is null && other.Columns is null)
            return true;
        if (Columns == null || other.Columns == null)
            return false;
        if (Columns.Length != other.Columns.Length)
            return false;

        for (int idx = 0; idx < Columns.Length; idx++)
        {
            if (!Columns[idx].Equals(other.Columns[idx]))
            {
                return false;
            }
        }

        return true;
    }

    public override int GetHashCode()
    {
        HashCode hashCode = new();

        hashCode.Add(Name);

        if (Columns != null)
        {
            foreach (ReportSpecificationColumnModel column in Columns)
            {
                hashCode.Add(column.GetHashCode());
            }
        }

        return hashCode.ToHashCode();
    }
}