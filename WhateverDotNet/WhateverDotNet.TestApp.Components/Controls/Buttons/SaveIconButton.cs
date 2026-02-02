using System.Windows;

namespace WhateverDotNet.TestApp.Components.Controls;

public class SaveIconButton : BaseIconButton
{
    static SaveIconButton()
    {
        DefaultStyleKeyProperty
            .OverrideMetadata(
                typeof(SaveIconButton),
                new FrameworkPropertyMetadata(typeof(SaveIconButton)));
    }
}