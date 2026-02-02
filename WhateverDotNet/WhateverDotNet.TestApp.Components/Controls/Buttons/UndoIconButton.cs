using System.Windows;

namespace WhateverDotNet.TestApp.Components.Controls;

public class UndoIconButton : BaseIconButton
{
    static UndoIconButton()
    {
        DefaultStyleKeyProperty
            .OverrideMetadata(
                typeof(UndoIconButton),
                new FrameworkPropertyMetadata(typeof(UndoIconButton)));
    }
}