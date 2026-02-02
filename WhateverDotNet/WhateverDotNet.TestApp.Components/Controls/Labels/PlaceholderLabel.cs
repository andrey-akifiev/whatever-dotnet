using System.Windows;

namespace WhateverDotNet.TestApp.Components.Controls;

public class PlaceholderLabel : BaseLabel
{
    static PlaceholderLabel()
    {
        DefaultStyleKeyProperty
            .OverrideMetadata(
                typeof(PlaceholderLabel),
                new FrameworkPropertyMetadata(typeof(PlaceholderLabel)));
    }
}