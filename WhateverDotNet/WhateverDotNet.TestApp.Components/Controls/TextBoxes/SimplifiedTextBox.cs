using System.Windows;

namespace WhateverDotNet.TestApp.Components.Controls;

public class SimplifiedTextBox : BaseTextBox
{
    static SimplifiedTextBox()
    {
        DefaultStyleKeyProperty
            .OverrideMetadata(
                typeof(SimplifiedTextBox),
                new FrameworkPropertyMetadata(typeof(SimplifiedTextBox)));
        TextMarginProperty
            .OverrideMetadata(
                typeof(SimplifiedTextBox),
                new PropertyMetadata(new Thickness(0.0, 0.0, 0.0, 0.0)));
    }
}