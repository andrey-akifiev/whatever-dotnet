using System.Windows;
using System.Windows.Controls;

namespace WhateverDotNet.TestApp.Components.Controls;

/// <summary>
/// Panel that combines a hamburger menu (with expand/collapse), an Add button, and a list of items
/// with title, description, and actions (Clone, Remove). Selection is tracked in the store for use in a details view.
/// </summary>
public partial class ListingPanel : UserControl
{
    public ListingPanel()
    {
        InitializeComponent();
    }

    private void OnActionsButtonClick(object sender, RoutedEventArgs e)
    {
        if (e.OriginalSource is not Button button || button.ContextMenu == null)
        {
            return;
        }

        button.ContextMenu.PlacementTarget = button;
        button.ContextMenu.IsOpen = true;
        e.Handled = true;
    }
}