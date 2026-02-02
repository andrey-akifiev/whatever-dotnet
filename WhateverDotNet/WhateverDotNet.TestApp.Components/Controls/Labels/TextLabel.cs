using System.Windows;

namespace WhateverDotNet.TestApp.Components.Controls;

public class TextLabel : BaseLabel
{
    static TextLabel()
    {
        DefaultStyleKeyProperty
            .OverrideMetadata(
                typeof(TextLabel),
                new FrameworkPropertyMetadata(typeof(TextLabel)));
    }
}