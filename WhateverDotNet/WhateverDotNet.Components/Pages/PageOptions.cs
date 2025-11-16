using WhateverDotNet.Components.Panes;

namespace WhateverDotNet.Components.Pages
{
    public class PageOptions : PaneOptions
    {
        public string[]? NavigationLinks { get; set; }

        public string? OnFilterRequestAlias { get; set; }

        public string? OnFilterRequestEndpoint { get; set; }

        public string? Url { get; set; }
    }
}