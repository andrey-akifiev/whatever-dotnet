namespace WhateverDotNet.TestApp.Components.Controls.DesignModels;

public class IconButtonDesignModel
{
    public IconButtonDesignModel()
    {
        Icon = "delete";
        Text = "Delete";
        IsEnabled = true;
    }

    public string Icon { get; set; }

    public string Text { get; set; }

    public bool IsEnabled { get; set; }
}