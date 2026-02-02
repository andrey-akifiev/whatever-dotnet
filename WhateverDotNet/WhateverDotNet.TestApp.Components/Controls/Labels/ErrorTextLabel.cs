using System.Windows;

namespace WhateverDotNet.TestApp.Components.Controls;

public class ErrorTextLabel : BaseLabel
{
    static ErrorTextLabel()
    {
        DefaultStyleKeyProperty
            .OverrideMetadata(
                typeof(ErrorTextLabel),
                new FrameworkPropertyMetadata(typeof(ErrorTextLabel)));
    }
}