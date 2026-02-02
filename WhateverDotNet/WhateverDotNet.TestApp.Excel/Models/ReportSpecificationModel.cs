using WhateverDotNet.TestApp.Components.Models;
using WhateverDotNet.TestApp.Components.Stores;

namespace WhateverDotNet.TestApp.Excel.Models;

public class ReportSpecificationModel
    : ICloneable,
        IEquatable<ReportSpecificationModel>,
        IModel,
        ISampleProvider<ReportSpecificationModel>
{
    public ReportSpecificationModel()
    {
        Id = Guid.NewGuid();
    }

    public string? Description { get; set; }

    public string? Name { get; set; }

    public Guid Id { get; set; }
 
    public string? SelectedWorksheetName { get; set; }

    public ReportSpecificationWorksheetModel[]? Worksheets { get; set; }

    public static ReportSpecificationModel CreateSample()
    {
        return new ReportSpecificationModel
        {
            Name = "Report Specification",
            Description = "Report Description",
            Worksheets = 
                [
                    ReportSpecificationWorksheetModel.CreateSample(),
                    ReportSpecificationWorksheetModel.CreateSample(),
                ],
        };
    }

    public object Clone()
    {
        ReportSpecificationWorksheetModel[]? clonedWorksheets = null;

        if (Worksheets != null)
        {
            clonedWorksheets = new ReportSpecificationWorksheetModel[Worksheets.Length];
            for (int idx = 0; idx < Worksheets.Length; idx++)
            {
                clonedWorksheets[idx] = (ReportSpecificationWorksheetModel)Worksheets[idx].Clone();
            }
        }

        return new ReportSpecificationModel
        {
            Name = Name,
            Description = Description,
            SelectedWorksheetName = SelectedWorksheetName,
            Worksheets = clonedWorksheets,
        };
    }

    public override bool Equals(object? obj) =>
        Equals(obj as ReportSpecificationModel);

    public bool Equals(ReportSpecificationModel? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        if (!string.Equals(Name, other.Name))
            return false;
        if (!string.Equals(Description, other.Description))
            return false;
        if (Worksheets is null && other.Worksheets is null)
            return true;
        if (Worksheets == null || other.Worksheets == null)
            return false;
        if (Worksheets.Length != other.Worksheets.Length)
            return false;

        for (int idx = 0; idx < Worksheets.Count(); idx++)
        {
            if (!Worksheets[idx].Equals(other.Worksheets[idx]))
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
        hashCode.Add(Description);

        if (Worksheets != null)
        {
            foreach (ReportSpecificationWorksheetModel sheet in Worksheets)
            {
                hashCode.Add(sheet.GetHashCode());
            }
        }

        return hashCode.ToHashCode();
    }
}