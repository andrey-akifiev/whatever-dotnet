using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WhateverDotNet.TestApp.Components.Controls;

public class BaseIconButton : Button
{
    public static readonly DependencyProperty CustomForegroundProperty =
        DependencyProperty.Register(
            nameof(CustomForeground),
            typeof(Brush),
            typeof(BaseIconButton),
            new PropertyMetadata(null));

    public static readonly DependencyProperty CustomHighlightedForegroundProperty =
        DependencyProperty.Register(
            nameof(CustomHighlightedForeground),
            typeof(Brush),
            typeof(BaseIconButton),
            new PropertyMetadata(null));

    public static readonly DependencyProperty IconDataProperty =
        DependencyProperty.Register(
            nameof(IconData),
            typeof(Geometry),
            typeof(BaseIconButton),
            new PropertyMetadata(default(Geometry)));

    public static readonly DependencyProperty IconPositionProperty =
        DependencyProperty.Register(
            nameof(IconPosition),
            typeof(IconButtonPosition),
            typeof(BaseIconButton),
            new PropertyMetadata(IconButtonPosition.Left));

    public static readonly DependencyProperty IsBusyProperty =
        DependencyProperty.Register(
            nameof(IsBusy),
            typeof(bool),
            typeof(BaseIconButton),
            new PropertyMetadata(false));

    public static readonly DependencyProperty KindProperty =
        DependencyProperty.Register(
            nameof(Kind),
            typeof(IconButtonKind),
            typeof(BaseIconButton),
            new PropertyMetadata(IconButtonKind.Regular));

    public static readonly DependencyProperty SizeProperty =
        DependencyProperty.Register(
            nameof(Size),
            typeof(double),
            typeof(BaseIconButton),
            new PropertyMetadata(
                defaultValue: 14.0,
                propertyChangedCallback: OnSizePropertyChanged,
                coerceValueCallback: OnSizeCoerceValue));

    static BaseIconButton()
    {
        DefaultStyleKeyProperty
            .OverrideMetadata(
                typeof(BaseIconButton),
                new FrameworkPropertyMetadata(typeof(BaseIconButton)));
    }

    public Brush CustomForeground
    {
        get => (Brush)GetValue(CustomForegroundProperty);
        set => SetValue(CustomForegroundProperty, value);
    }

    public Brush CustomHighlightedForeground
    {
        get => (Brush)GetValue(CustomHighlightedForegroundProperty);
        set => SetValue(CustomHighlightedForegroundProperty, value);
    }

    public Geometry IconData
    {
        get => (Geometry)GetValue(IconDataProperty);
        set => SetValue(IconDataProperty, value);
    }

    public IconButtonPosition IconPosition
    {
        get => (IconButtonPosition)GetValue(IconPositionProperty);
        set => SetValue(IconPositionProperty, value);
    }

    public bool IsBusy
    {
        get => (bool)GetValue(IsBusyProperty);
        set => SetValue(IsBusyProperty, value);
    }

    public IconButtonKind Kind
    {
        get => (IconButtonKind)GetValue(KindProperty);
        set => SetValue(KindProperty, value);
    }

    public double Size
    {
        get => (double)GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    private static void OnSizePropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e) => OnSizeChanged(d, e.NewValue);

    private static object OnSizeCoerceValue(DependencyObject d, object o)
    {
        OnSizeChanged(d, o);
        return o;
    }

    private static void OnSizeChanged(DependencyObject d, object newValue)
    {
        if (d is not BaseIconButton button || newValue is not double size)
        {
            return;
        }

        button.Height = size;

        if (!button.IsFontSizeExplicitlySet())
        {
            button.FontSize = size * 0.75;
        }
    }

    private bool IsFontSizeExplicitlySet() =>
        IsPropertyExplicitlySet(
            DependencyPropertyHelper.GetValueSource(this, FontSizeProperty));

    private static bool IsPropertyExplicitlySet(ValueSource valueSource)
    {
        return valueSource.BaseValueSource == BaseValueSource.Local
            || valueSource.BaseValueSource == BaseValueSource.Style
            || valueSource.BaseValueSource == BaseValueSource.StyleTrigger;
    }
}