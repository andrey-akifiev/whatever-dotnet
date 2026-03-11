using System.ComponentModel;

namespace WhateverDotNet.TestApp.Components.Controls;

public class TableColumn : INotifyPropertyChanged
{
    private string? _displayName;
    private double _width;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Name { get; init; }

    public string DisplayName
    {
        get => _displayName ?? Name;
        set
        {
            if (_displayName == value)
            {
                return;
            }

            _displayName = value;
            OnPropertyChanged(nameof(DisplayName));
        }
    }

    /// <summary>
    /// Calculated width for this column, set by EditableTable.
    /// </summary>
    public double Width
    {
        get => _width;
        set
        {
            if (Equals(_width, value))
            {
                return;
            }

            _width = value;
            OnPropertyChanged(nameof(Width));
        }
    }

    private void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}