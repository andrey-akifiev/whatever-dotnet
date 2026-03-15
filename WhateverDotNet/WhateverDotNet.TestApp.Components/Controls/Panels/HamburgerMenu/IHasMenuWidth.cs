namespace WhateverDotNet.TestApp.Components.Controls;

/// <summary>
/// Exposes the current width of a sidebar/menu (e.g. for expand/collapse).
/// </summary>
public interface IHasMenuWidth
{
    double MenuWidth { get; }
}