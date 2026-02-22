using System.Windows;

namespace WhateverDotNet.TestApp.Components.Controls;

/// <summary>
/// Button with icon on the left and "Sign In" caption on the right, for OKTA/SSO sign-in.
/// Set Command to a command that runs the sign-in flow (e.g. calls IOktaSignInService.SignInAsync)
/// and updates AuthenticationStore.
/// </summary>
public class SignInIconButton : BaseIconButton
{
    static SignInIconButton()
    {
        DefaultStyleKeyProperty
            .OverrideMetadata(
                typeof(SignInIconButton),
                new FrameworkPropertyMetadata(typeof(SignInIconButton)));
        ContentProperty
            .OverrideMetadata(
                typeof(SignInIconButton),
                new FrameworkPropertyMetadata("Sign In"));
        IconPositionProperty
            .OverrideMetadata(
                typeof(SignInIconButton),
                new PropertyMetadata(IconButtonPosition.Left));
    }

    public SignInIconButton()
    {
        Content ??= "Sign In";
    }
}
