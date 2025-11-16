using Microsoft.Playwright;

using WhateverDotNet.Abstractions;
using WhateverDotNet.Abstractions.Exceptions;
using WhateverDotNet.Components.Selects;

namespace WhateverDotNet.Components.Panes
{
    public abstract class BasePane<TPaneOptions> : IApplicationPane<TPaneOptions>
        where TPaneOptions : PaneOptions
    {
        protected readonly TPaneOptions _options;
        protected readonly IScenarioContext _scenarioContext;

        private readonly Dictionary<string, IComponent> _componentRegistry = new Dictionary<string, IComponent>();

        public BasePane(IScenarioContext scenarioContext, TPaneOptions options)
        {
            _scenarioContext = scenarioContext;
            _options = options;
        }

        public virtual Task AssertIsDisabledAsync(string componentName)
        {
            // TODO: in C# it looks ridiculous, need to find a better way -- aa
            var cmp = GetComponent(componentName);
            return cmp is IComponent
                ? (cmp as IComponent)!.AssertIsDisabledAsync()
                : (cmp as IApplicationPane)!.AssertIsDisabledAsync(componentName);
        }

        public virtual Task AssertIsEnabledAsync(string componentName)
        {
            var cmp = GetComponent(componentName);
            return cmp is IComponent
                ? (cmp as IComponent)!.AssertIsEnabledAsync()
                : (cmp as IApplicationPane)!.AssertIsEnabledAsync(componentName);
        }

        public virtual Task AssertIsNotVisibleAsync()
        {
            return Assertions.Expect(GetLocator(GetXPathComponent())).ToBeHiddenAsync();
        }

        public virtual Task AssertIsNotVisibleAsync(string componentName)
        {
            var cmp = GetComponent(componentName);
            return cmp is IComponent
                ? (cmp as IComponent)!.AssertIsNotVisibleAsync()
                : (cmp as IApplicationPane)!.AssertIsNotVisibleAsync(componentName);
        }

        public virtual Task AssertIsVisibleAsync()
        {
            return Assertions.Expect(GetComponent()).ToBeVisibleAsync();
        }

        public Task AssertHasNoValidationAsync(string componentName)
        {
            var cmp = GetComponent(componentName);
            return cmp is IComponent
                ? (cmp as IComponent)!.AssertHasNoValidationAsync()
                : (cmp as IApplicationPane)!.AssertHasNoValidationAsync(componentName);
        }

        public virtual Task AssertHasOptionAsync(string componentName, string optionName)
        {
            var cmp = GetComponent(componentName);
            return cmp is IComponent
                ? (cmp as IComponent)!.AssertHasOptionAsync(optionName)
                : (cmp as IApplicationPane)!.AssertHasOptionAsync(componentName, optionName);
        }

        public virtual Task AssertHasTextAsync(string componentName, string expectedText)
        {
            var cmp = GetComponent(componentName);
            return cmp is IComponent
                ? (cmp as IComponent)!.AssertHasTextAsync(expectedText)
                : (cmp as IApplicationPane)!.AssertHasTextAsync(componentName, expectedText);
        }

        public virtual Task AssertHasValidationAsync(string componentName, string? expectedValidationMessage = null)
        {
            var cmp = GetComponent(componentName);
            return cmp is IComponent
                ? (cmp as IComponent)!.AssertHasValidationAsync(expectedValidationMessage)
                : (cmp as IApplicationPane)!.AssertHasValidationAsync(componentName, expectedValidationMessage);
        }

        public virtual Task ClickAsync(string componentName, LocatorClickOptions? options = null)
        {
            var cmp = GetComponent(componentName);
            return cmp is IComponent
                ? (cmp as IComponent)!.ClickAsync(options)
                : (cmp as IApplicationPane)!.ClickAsync(componentName, options);
        }

        public virtual async Task ClickCaptionAsync()
        {
            if (GetOptions().XPathCaption == null)
            {
                throw new OptionNotDefinedException(nameof(_options.XPathCaption), this);
            }

            await GetLocator($"{GetXPathParent()}{GetOptions().XPathCaption}")
                .ClickAsync(new LocatorClickOptions { Force = true })
                .ConfigureAwait(false);
        }

        public virtual TPaneOptions GetOptions()
        {
            return _options;
        }

        public virtual Task<string> SelectItemAsync(string componentName, string itemName, LocatorClickOptions? options = null)
        {
            var cmp = GetComponent(componentName);
            return cmp is IComponent
                ? (cmp as ISelectComponent)!.SelectItemAsync(itemName, new ForceOptions { Force = options?.Force })
                : (cmp as IApplicationPane)!.SelectItemAsync(componentName, itemName, options);
        }

        public virtual Task TypeAsync(string componentName, string text, ForceOptions? options = null)
        {
            var cmp = GetComponent(componentName);
            return cmp is IComponent
                ? (cmp as IComponent)!.TypeAsync(text, options)
                : (cmp as IApplicationPane)!.TypeAsync(componentName, text, options);
        }

        public virtual async Task WaitUntilLoadedAsync()
        {
            string? xPathLoader = GetOptions().XPathLoader;

            if (string.IsNullOrWhiteSpace(xPathLoader))
            {
                return;
            }

            await GetLocator(xPathLoader)
                .WaitForAsync(new LocatorWaitForOptions
                {
                    State = WaitForSelectorState.Detached
                })
                .ConfigureAwait(false);
        }

        public ILocator GetComponent()
        {
            return GetLocator(GetXPathComponent());
        }

        public object GetComponent(string componentName)
        {
            string normalizedComponentName = NormalizeName(componentName);

            object? result = GetOptions()
                .Components
                ?.FirstOrDefault(c => c.GetOptions().Name == normalizedComponentName);

            if (result != null)
            {
                return result;
            }

            result = GetOptions()
                .Panes
                ?.FirstOrDefault(p => p.GetOptions().Name == normalizedComponentName);

            if (result != null)
            {
                return result;
            }

            result = GetOptions()
                .Panes?
                .FirstOrDefault(p => 
                    null != p.GetOptions()
                        .Components
                        ?.FirstOrDefault(c => c.GetOptions().Name == normalizedComponentName))
                ?.GetComponent(componentName);

            if (result != null)
            {
                return result;
            }

            throw new Exception();
        }

        protected ILocator GetLocator(string selector)
        {
            return GetPage().Locator(selector);
        }

        protected IPage GetPage()
        {
            return _scenarioContext.GetBrowserPage();
        }

        protected string GetXPathComponent()
        {
            string? xPathComponent = GetOptions().XPathComponent;

            if (xPathComponent == null)
            {
                throw new OptionNotDefinedException(nameof(_options.XPathComponent), this);
            }

            return $"{GetXPathParent()}{xPathComponent}";
        }

        protected string GetXPathParent()
        {
            return GetOptions()?.XPathParent ?? string.Empty;
        }
        //
        // protected bool IsComplexComponent(ComponentOptions componentOptions)
        // {
        //     
        // }
        //
        // private ComponentOptions[] GetPredefinedComponentsOrThrow()
        // {
        //     ComponentOptions[]? predefinedComponents = GetOptions().ComponentOptions;
        //     
        //     if (predefinedComponents == null || predefinedComponents.Length == 0)
        //     {
        //         throw new ComponentNotSpecifiedException();
        //     }
        //
        //     return predefinedComponents;
        // }
        //
        private string NormalizeName(string name)
        {
            return System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(name).Replace(" ", string.Empty);
        }
    }
}
