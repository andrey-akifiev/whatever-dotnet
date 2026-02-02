using System.Windows;

namespace WhateverDotNet.TestApp.Components.Controls;

public class RegularTextBox : LabeledTextBox
{
    static RegularTextBox()
    {
        DefaultStyleKeyProperty
            .OverrideMetadata(
                typeof(RegularTextBox),
                new FrameworkPropertyMetadata(typeof(RegularTextBox)));
        ShowCopyProperty
            .OverrideMetadata(
                typeof(RegularTextBox),
                new PropertyMetadata(true));
        TextMarginProperty
            .OverrideMetadata(
                typeof(RegularTextBox),
                new PropertyMetadata(new Thickness(10.0, 0.0, 0.0, 0.0)));
    }
}