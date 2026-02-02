using System.Windows;

namespace WhateverDotNet.TestApp.Components.Controls;

public class SimplifiedTextBlock : BaseTextBlock
{
    static SimplifiedTextBlock()
    {
        DefaultStyleKeyProperty
            .OverrideMetadata(
                typeof(SimplifiedTextBlock),
                new FrameworkPropertyMetadata(typeof(SimplifiedTextBlock)));
    }
}