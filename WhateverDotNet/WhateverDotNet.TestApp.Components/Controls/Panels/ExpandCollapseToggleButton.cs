using System.Windows;
using System.Windows.Controls.Primitives;

namespace WhateverDotNet.TestApp.Components.Controls;

/// <summary>
/// Toggle button with consistent expand/collapse appearance: chevron icon and optional "Collapse" label.
/// Used in HamburgerMenu and ListingPanel. IsChecked = false means expanded; true means collapsed.
/// </summary>
public class ExpandCollapseToggleButton : ToggleButton
{
    static ExpandCollapseToggleButton()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(ExpandCollapseToggleButton),
            new FrameworkPropertyMetadata(typeof(ExpandCollapseToggleButton)));
    }
}