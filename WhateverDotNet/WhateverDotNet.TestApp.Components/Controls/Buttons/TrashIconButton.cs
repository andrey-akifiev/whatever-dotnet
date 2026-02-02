using System.Windows;

namespace WhateverDotNet.TestApp.Components.Controls;

public class TrashIconButton : BaseIconButton
{
    static TrashIconButton()
    {
        DefaultStyleKeyProperty
            .OverrideMetadata(
                typeof(TrashIconButton),
                new FrameworkPropertyMetadata(typeof(TrashIconButton)));
    }
}