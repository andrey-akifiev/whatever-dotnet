using System.Windows;

namespace WhateverDotNet.TestApp.Components.Controls;

public class SimplifiedComboBox : BaseComboBox
{
    static SimplifiedComboBox()
    {
        DefaultStyleKeyProperty
            .OverrideMetadata(
                typeof(SimplifiedComboBox),
                new FrameworkPropertyMetadata(typeof(SimplifiedComboBox)));
        ContentMarginProperty
            .OverrideMetadata(
                typeof(SimplifiedComboBox),
                new PropertyMetadata(new Thickness(0.0, 0.0, 0.0, 0.0)));
    }
}
