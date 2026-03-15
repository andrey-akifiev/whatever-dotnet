using System.Collections.ObjectModel;

namespace WhateverDotNet.TestApp.Components.Controls;

/// <summary>
/// Store for the listing panel: holds items and the currently selected/open item<br/>
/// so a details view can bind to it and react to selection changes.
/// </summary>
/// <typeparam name="TItemModel">Item type; must be cloneable and implement <see cref="IEnlistableModel"/>.</typeparam>
public interface IEnlistableItemsStore<TItemModel>
    where TItemModel : class, IEnlistableModel
{
    /// <summary>
    /// Raised when <see cref="SelectedItem"/> changes (selection or open state).
    /// </summary>
    public event Action? SelectedItemChanged;

    /// <summary>
    /// All items in the list.
    /// </summary>
    public ObservableCollection<TItemModel> Items { get; }

    /// <summary>
    /// Currently selected/open item (e.g. shown in the details view). Null if none.
    /// </summary>
    public TItemModel? SelectedItem { get; }

    /// <summary>
    /// Adds a new item (via the configured factory), adds it to <see cref="Items"/>,
    /// and sets it as <see cref="SelectedItem"/>.
    /// </summary>
    public void Add(TItemModel item);

    /// <summary>
    /// Removes the item from <see cref="Items"/>. If it was selected, clears selection.
    /// </summary>
    public void Remove(TItemModel item);

    public void SelectItem(TItemModel item);
}