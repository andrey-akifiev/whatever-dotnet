namespace WhateverDotNet.TestApp.Components.Pages;

public interface IPageViewModel
{
    public string? Description { get; }

    /// <summary>
    /// Optional SVG path data or icon key for the menu item.
    /// </summary>
    public string? Icon { get; }

    /// <summary>
    /// Display title shown in the hamburger menu.
    /// </summary>
    public string Title { get; }
}