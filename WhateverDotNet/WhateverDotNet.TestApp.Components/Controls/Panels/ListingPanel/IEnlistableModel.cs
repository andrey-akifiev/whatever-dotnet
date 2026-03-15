using WhateverDotNet.TestApp.Components.Stores;

namespace WhateverDotNet.TestApp.Components.Controls;

/// <summary>
/// Model for an item in the listing panel. Used for title and description display;
/// must be <see cref="ICloneable"/> for Clone support.
/// </summary>
public interface IEnlistableModel : IModel
{
    string? Title { get; }
    string? Description { get; }
}