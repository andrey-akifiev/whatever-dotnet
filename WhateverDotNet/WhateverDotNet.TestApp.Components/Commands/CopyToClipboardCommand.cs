using System.Windows;
using System.Windows.Input;

namespace WhateverDotNet.TestApp.Components.Commands;

public class CopyToClipboardCommand : ICommand
{
    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
    {
        if (parameter is CopyToCliboardCommandParameters param)
        {
            return !string.IsNullOrWhiteSpace(param.Value);
        }

        if (parameter is string str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }

        return parameter != null;
    }

    public void Execute(object? parameter)
    {
        string? text = null;

        if (parameter is CopyToCliboardCommandParameters param)
        {
            var value = param.Value?.ToString();

            if (!string.IsNullOrWhiteSpace(param.Template))
            {
                value = string.Format(param.Template, param.Value);
            }

            text = value;
        }
        else
        {
            text = parameter?.ToString();
        }

        if (text != null)
        {
            Clipboard.SetText(text);
        }
    }
}