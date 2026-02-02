using System.Windows;
using System.Windows.Controls;

namespace WhateverDotNet.TestApp.Components.Controls
{
    public class BaseTextBlock : TextBlock
    {
        static BaseTextBlock()
        {
            DefaultStyleKeyProperty
                .OverrideMetadata(
                    typeof(BaseTextBlock),
                    new FrameworkPropertyMetadata(typeof(BaseTextBlock)));
        }
    }
}