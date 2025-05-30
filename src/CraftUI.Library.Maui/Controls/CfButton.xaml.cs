using System.Windows.Input;

namespace CraftUI.Library.Maui.Controls;

public partial class CfButton
{
    private const string LowerKey = "lower";
    private const string UpperKey = "upper";

    private readonly Animation _lowerAnimation;
    private readonly Animation _upperAnimation;

    public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(CfButton), defaultBindingMode: BindingMode.OneWay);
    public static readonly BindableProperty IsLoadingProperty = BindableProperty.Create(nameof(IsLoading), typeof(bool), typeof(CfButton), defaultBindingMode: BindingMode.OneWay, propertyChanged: IsLoadingChanged);
    public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(CfButton));
    public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(CfButton));
    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(object), typeof(CfButton));
    
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
    
    public bool IsLoading
    {
        get => (bool)GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }
    
    public ICommand? Command
    {
        get => (ICommand?)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }
    
    public CfButton()
    {
        InitializeComponent();
        
        _lowerAnimation = new Animation(v => AnimatedProgressBar.LowerRangeValue = (float)v, start: -0.4, end: 1.0);
        _upperAnimation = new Animation(v => AnimatedProgressBar.UpperRangeValue = (float)v, start: 0.0, end: 1.4);
    }
    
    protected override void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == IsEnabledProperty.PropertyName)
        {
            Button.IsEnabled = IsEnabled;
        }
        else if (propertyName == BackgroundColorProperty.PropertyName)
        {
            Button.BackgroundColor = BackgroundColor;
        }
        else if (propertyName == TextColorProperty.PropertyName)
        {
            AnimatedProgressBar.ProgressColor = TextColor;
            Button.TextColor = TextColor;
        }
        else if (propertyName == TextProperty.PropertyName)
        {
            Button.Text = Text;
        }
    }
    
    private static void IsLoadingChanged(BindableObject bindable, object oldValue, object newValue) => ((CfButton)bindable).UpdateIsLoadingView();
    
    private void Button_OnClicked(object? sender, EventArgs e)
    {
        if (Command != null && Command.CanExecute(CommandParameter))
        {
            Command.Execute(CommandParameter);
        }
    }
    
    private void UpdateIsLoadingView()
    {
        Button.IsEnabled = !IsLoading;
        AnimatedProgressBar.IsVisible = IsLoading;
        
        if (IsLoading)
        {
            _lowerAnimation.Commit(owner: this, name: LowerKey, length: 1000, easing: Easing.CubicInOut, repeat: () => true);
            _upperAnimation.Commit(owner: this, name: UpperKey, length: 1000, easing: Easing.CubicInOut, repeat: () => true);
        }
        else
        {
            this.AbortAnimation(handle: LowerKey);
            this.AbortAnimation(handle: UpperKey);
        }
    }
}