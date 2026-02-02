namespace WhateverDotNet.TestApp.Components.Commands;

public abstract class BaseAsyncCommand : BaseCommand
{
    public override async void Execute(object? parameter)
    {
        try
        {
            await ExecuteAsync(parameter!);
        }
        catch (Exception)
        {
            // do nothing
        }
    }

    public abstract Task ExecuteAsync(object parameter);
}