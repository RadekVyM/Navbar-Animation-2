namespace NavbarAnimation.Maui.Views.Controls;

public partial class TabBarLayer : Grid
{
    public static readonly BindableProperty ColorProperty =
        BindableProperty.Create(nameof(Color), typeof(Color), typeof(TabBarLayer), Colors.Black, BindingMode.OneWay);

    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }


    public TabBarLayer()
	{
		InitializeComponent();
        BindingContext = this;
	}
}
