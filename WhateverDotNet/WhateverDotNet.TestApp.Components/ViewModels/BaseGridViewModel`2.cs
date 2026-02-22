using System.Collections.ObjectModel;

using WhateverDotNet.TestApp.Components.Models;

namespace WhateverDotNet.TestApp.Components.ViewModels;

public abstract class BaseGridViewModel<TRowViewModel, TRowModel>
    : RevertibleCollectionViewModel<TRowViewModel, TRowModel>
        where TRowViewModel
            : RevertibleItemViewModel<TRowModel>, IRevertibleViewModel<TRowModel>
        where TRowModel
            : class, ICloneable, IEquatable<TRowModel>, ISampleProvider<TRowModel>
{
    protected BaseGridViewModel(ObservableCollection<TRowViewModel> rows)
        : base(rows)
    {
    }
}