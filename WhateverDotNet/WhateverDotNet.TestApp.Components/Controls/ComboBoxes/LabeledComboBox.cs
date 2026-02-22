using System.Windows;

namespace WhateverDotNet.TestApp.Components.Controls;

public class LabeledComboBox : BaseComboBox
{
    public static readonly DependencyProperty LabelValueProperty =
        DependencyProperty.Register(
            nameof(LabelValue),
            typeof(string),
            typeof(LabeledComboBox),
            new PropertyMetadata(null));

    static LabeledComboBox()
    {
        DefaultStyleKeyProperty
            .OverrideMetadata(
                typeof(LabeledComboBox),
                new FrameworkPropertyMetadata(typeof(LabeledComboBox)));
    }

    public string? LabelValue
    {
        get => (string?)GetValue(LabelValueProperty);
        set => SetValue(LabelValueProperty, value);
    }
}
