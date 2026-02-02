using System.Windows;
using System.Windows.Controls;

namespace WhateverDotNet.TestApp.Components.Controls;

public class BaseLabel : Label
{
    static BaseLabel()
    {
        DefaultStyleKeyProperty
            .OverrideMetadata(
                typeof(BaseLabel),
                new FrameworkPropertyMetadata(typeof(BaseLabel)));
    }
}