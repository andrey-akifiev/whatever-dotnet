using WhateverDotNet.TestApp.Components.Models;

namespace WhateverDotNet.TestApp.Excel.Models;

public class ReportSpecificationColumnModel
    : ICloneable, 
      IEquatable<ReportSpecificationColumnModel>,
      ISampleProvider<ReportSpecificationColumnModel>
{
    private string? _displayName;

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

    public bool IsComparable { get; set; }

    public bool IsValuable { get; set; }

    public string? Name { get; set; }

    public ReportSpecificationColumnTypes Type { get; set; }

    public object Clone()
    {
        return new ReportSpecificationColumnModel
        {
            DisplayName = DisplayName,
            Format = Format,
            IsComparable = IsComparable,
            IsValuable = IsValuable,
            Name = Name,
            Type = Type,
        };
    }

    public static ReportSpecificationColumnModel CreateSample()
    {
        return new ReportSpecificationColumnModel
        {
            Name = "Column",
            Format = "C",
            IsComparable = false,
            IsValuable = false,
            Type = ReportSpecificationColumnTypes.String,
        };
    }

    public override bool Equals(object? obj) =>
        Equals(obj as ReportSpecificationColumnModel);

    public bool Equals(ReportSpecificationColumnModel? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.Name == other.Name
            && this.DisplayName == other.DisplayName
            && this.Format == other.Format
            && this.IsComparable == other.IsComparable
            && this.IsValuable == other.IsValuable
            && this.Type == other.Type;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, DisplayName, Format, IsComparable, IsValuable, Type);
    }
}