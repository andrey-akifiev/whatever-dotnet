using CommunityToolkit.Mvvm.ComponentModel;

namespace WhateverDotNet.TestApp.Components.Controls.Models;

public class SelectableItem : ObservableObject
{
    public SelectableItem(string text, bool isSelected = false)
    {
        Text = text;
        IsSelected = isSelected;
    }

    public bool IsSelected { get; set; }

    public string Text { get; }
}