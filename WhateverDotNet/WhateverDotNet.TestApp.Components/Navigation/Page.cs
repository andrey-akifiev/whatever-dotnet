using WhateverDotNet.TestApp.Components.ViewModels;

namespace WhateverDotNet.TestApp.Components.Navigation;

public class Page
{
    public Page()
    {
    }

    public Page(string name, Func<BaseViewModel> viewModelFactory)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (viewModelFactory == null)
        {
            throw new ArgumentNullException(nameof(viewModelFactory));
        }

        Name = name;
        ViewModelFactory = viewModelFactory;
    }

    public Page(string name, string description, Func<BaseViewModel> viewModelFactory)
        : this(name, viewModelFactory)
    {
        Description = description;
    }

    public string Name { get; set; }

    public string? Description { get; set; }

    public bool IsEnabled => ViewModelFactory != null;
 
    public Func<BaseViewModel>? ViewModelFactory { get; set; }
}