using System.Windows;

namespace WhateverDotNet.TestApp.Components.Controls;

public class LabeledTextBox : BaseTextBox
{
    public static readonly DependencyProperty LabelValueProperty =
        DependencyProperty.Register(
            nameof(LabelValue),
            typeof(string),
            typeof(LabeledTextBox),
            new PropertyMetadata(null));

    static LabeledTextBox()
    {
        DefaultStyleKeyProperty
            .OverrideMetadata(
                typeof(LabeledTextBox),
                new FrameworkPropertyMetadata(typeof(LabeledTextBox)));
    }

    public string? LabelValue
    {
        get => (string)GetValue(LabelValueProperty);
        set => SetValue(LabelValueProperty, value);
    }
}