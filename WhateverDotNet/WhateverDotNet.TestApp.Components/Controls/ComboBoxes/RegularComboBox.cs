using System.Windows;

namespace WhateverDotNet.TestApp.Components.Controls;

public class RegularComboBox : LabeledComboBox
{
    static RegularComboBox()
    {
        DefaultStyleKeyProperty
            .OverrideMetadata(
                typeof(RegularComboBox),
                new FrameworkPropertyMetadata(typeof(RegularComboBox)));
        ShowCopyProperty
            .OverrideMetadata(
                typeof(RegularComboBox),
                new PropertyMetadata(true));
        ContentMarginProperty
            .OverrideMetadata(
                typeof(RegularComboBox),
                new PropertyMetadata(new Thickness(10.0, 0.0, 0.0, 0.0)));
    }
}
