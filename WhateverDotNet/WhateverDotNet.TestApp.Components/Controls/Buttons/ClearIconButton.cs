using System.Windows;

namespace WhateverDotNet.TestApp.Components.Controls;

public class ClearIconButton : BaseIconButton
{
    static ClearIconButton()
    {
        DefaultStyleKeyProperty
            .OverrideMetadata(
                typeof(ClearIconButton),
                new FrameworkPropertyMetadata(typeof(ClearIconButton)));
    }
}