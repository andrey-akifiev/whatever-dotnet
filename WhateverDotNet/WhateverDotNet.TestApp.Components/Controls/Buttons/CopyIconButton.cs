using System.Windows;

namespace WhateverDotNet.TestApp.Components.Controls;

public class CopyIconButton : BaseIconButton
{
    static CopyIconButton()
    {
        DefaultStyleKeyProperty
            .OverrideMetadata(
                typeof(CopyIconButton),
                new FrameworkPropertyMetadata(typeof(CopyIconButton)));
    }
}