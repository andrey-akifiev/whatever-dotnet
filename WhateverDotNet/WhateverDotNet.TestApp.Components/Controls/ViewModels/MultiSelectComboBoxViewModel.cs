using CommunityToolkit.Mvvm.ComponentModel;

using WhateverDotNet.TestApp.Components.Controls.Views;

namespace WhateverDotNet.TestApp.Components.Controls.ViewModels;

internal class MultiSelectComboBoxViewModel : ObservableObject
{
    private readonly MultiSelectComboBox _view;

    public MultiSelectComboBoxViewModel(MultiSelectComboBox view)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
    }

    public void OnItemsChanged()
    {
        _view.DisplayText = _view.Items is null
            ? null
            : string.Join(", ", _view.Items
                .Where(item => item.IsSelected)
                .Select(item => item.Text));
    }
}