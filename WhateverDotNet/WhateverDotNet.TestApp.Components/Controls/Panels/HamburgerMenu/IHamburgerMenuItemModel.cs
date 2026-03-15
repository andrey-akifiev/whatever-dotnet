using WhateverDotNet.TestApp.Components.Stores;

namespace WhateverDotNet.TestApp.Components.Controls;

public interface IHamburgerMenuItemModel : IModel
{
    public string? Description { get; }
    public string? Icon { get; }
    public string? Title { get; }
}