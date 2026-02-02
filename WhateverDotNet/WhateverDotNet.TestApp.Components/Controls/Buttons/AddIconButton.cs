using System.Windows;

namespace WhateverDotNet.TestApp.Components.Controls;

public class AddIconButton : BaseIconButton
{
    static AddIconButton()
    {
        DefaultStyleKeyProperty
            .OverrideMetadata(
                typeof(AddIconButton),
                new FrameworkPropertyMetadata(typeof(AddIconButton)));
    }
}