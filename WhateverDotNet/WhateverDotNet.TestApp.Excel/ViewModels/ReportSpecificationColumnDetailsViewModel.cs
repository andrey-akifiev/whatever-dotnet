using WhateverDotNet.TestApp.Components.ViewModels;
using WhateverDotNet.TestApp.Excel.Models;

namespace WhateverDotNet.TestApp.Excel.ViewModels;

public class ReportSpecificationColumnDetailsViewModel : BaseRowViewModel<ReportSpecificationColumnModel>
{
    public ReportSpecificationColumnDetailsViewModel(
        ReportSpecificationColumnModel originalValues,
        Action<BaseRowViewModel<ReportSpecificationColumnModel>> onClone,
        Action<BaseRowViewModel<ReportSpecificationColumnModel>> onDelete)
        : base(originalValues, onClone, onDelete)
    {
    }

    public string? Name
    {
        get => Current.Name;
        set
        {
            if (Current.Name != value)
            {
                Current.Name = value;
                RaisePropertiesChanged(nameof(Name));
            }
        }
    }

    public string? NameOriginal
    {
        get => Original.Name;
    }

    public string? DisplayName
    {
        get => Current.DisplayName;
        set
        {
            if (Current.DisplayName != value)
            {
                Current.DisplayName = value;
                RaisePropertiesChanged(nameof(DisplayName));
            }
        }
    }

    public string? DisplayNameOriginal
    {
        get => Original.DisplayName;
    }

    public string? Format
    {
        get => Current.Format;
        set
        {
            if (Current.Format != value)
            {
                Current.Format = value;
                RaisePropertiesChanged(nameof(Format));
            }
        }
    }

    public string? FormatOriginal => Original.Format;

    public bool IsComparable
    {
        get => Current.IsComparable;
        set
        {
            if (Current.IsComparable != value)
            {
                Current.IsComparable = value;
                RaisePropertiesChanged(nameof(IsComparable));
            }
        }
    }

    public bool IsValuable
    {
        get => Current.IsValuable;
        set
        {
            if (Current.IsValuable != value)
            {
                Current.IsValuable = value;
                RaisePropertiesChanged(nameof(IsValuable));
            }
        }
    }

    public ReportSpecificationColumnTypes Type
    {
        get => Current.Type;
        set
        {
            if (Current.Type != value)
            {
                Current.Type = value;
                RaisePropertiesChanged(nameof(Type));
            }
        }
    }

    public override void Revert()
    {
        base.Revert();

        RaisePropertiesChanged(
            nameof(Name),
            nameof(DisplayName),
            nameof(Format),
            nameof(IsComparable),
            nameof(IsValuable),
            nameof(Type));
    }

    protected override bool AreEqual(ReportSpecificationColumnModel a, ReportSpecificationColumnModel b) =>
        a.Equals(b);

    protected override ReportSpecificationColumnModel CloneModel(ReportSpecificationColumnModel source) =>
        (ReportSpecificationColumnModel)source.Clone();
}