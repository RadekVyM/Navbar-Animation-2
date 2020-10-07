using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Reflection;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NavbarAnimation2
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabSvgView : ContentView
    {
        static SKTypeface typeface;

        #region Public members

        public PageEnum Page { get; set; }
        public bool Selected { get; set; }

        public object Colour
        {
            get => GetValue(ColourProperty);
            set => SetValue(ColourProperty, value);
        }

        public string Path
        {
            get => (string)GetValue(PathProperty);
            set => SetValue(PathProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public double SvgWidth
        {
            get => (double)GetValue(SvgWidthProperty);
            set => SetValue(SvgWidthProperty, value);
        }

        public double SvgHeight
        {
            get => (double)GetValue(SvgHeightProperty);
            set => SetValue(SvgHeightProperty, value);
        }

        public static readonly BindableProperty ColourProperty =
            BindableProperty.Create(nameof(Colour), typeof(object), typeof(TabSvgView), Color.Black, BindingMode.OneWay, propertyChanged: MyPropertyChanged);

        public static readonly BindableProperty PathProperty =
            BindableProperty.Create(nameof(Path), typeof(string), typeof(TabSvgView), "", BindingMode.OneWay, propertyChanged: MyPropertyChanged);

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(TabSvgView), "", BindingMode.OneWay, propertyChanged: MyPropertyChanged);

        public static readonly BindableProperty SvgWidthProperty =
            BindableProperty.Create(nameof(SvgWidth), typeof(double), typeof(TabSvgView), 20d, BindingMode.OneWay, propertyChanged: MyPropertyChanged);

        public static readonly BindableProperty SvgHeightProperty =
            BindableProperty.Create(nameof(SvgHeight), typeof(double), typeof(TabSvgView), 20d, BindingMode.OneWay, propertyChanged: MyPropertyChanged);

        private static void MyPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            TabSvgView svgView = bindable as TabSvgView;

            try
            {
                svgView.canvasView.InvalidateSurface();
            }
            catch { };
        }

        #endregion


        public TabSvgView()
        {
            InitializeComponent();
        }


        private void CanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            var info = e.Info;
            canvas.Clear();

            if (string.IsNullOrWhiteSpace(Path))
                return;

            if(typeface == null)
            {
                var assembly = Assembly.GetExecutingAssembly();
                var stream = assembly.GetManifestResourceStream("NavbarAnimation2.Fonts.GlacialIndifference-Bold.otf");

                typeface = SKTypeface.FromStream(stream);
            }

            SKPath path = SKPath.ParseSvgPathData(Path);

            using (SKPaint paint = new SKPaint())
            {
                paint.Color = Colour.GetColour().ToSKColor();
                paint.Typeface = typeface;

                canvas.Save();

                path.GetBounds(out SKRect bounds);

                canvas.Translate(info.Width / 2f, (info.Height / 2f) * 0.9f);
                float scale = Math.Min((float)((SvgWidth * DeviceDisplay.MainDisplayInfo.Density) / bounds.Width), (float)((SvgHeight * DeviceDisplay.MainDisplayInfo.Density) / bounds.Height));
                canvas.Scale(scale);
                canvas.Translate(-bounds.MidX, -bounds.MidY);

                canvas.ClipPath(path, SKClipOperation.Difference, true);

                canvas.ResetMatrix();

                path = paint.GetTextPath(Text, 0, 0);
                path.GetBounds(out bounds);

                canvas.Translate(info.Width / 2f, (info.Height / 2f) + (float)(SvgHeight * 0.8f * DeviceDisplay.MainDisplayInfo.Density));
                canvas.Scale(2.3f);
                canvas.Translate(-bounds.MidX, 4);

                canvas.ClipPath(path, SKClipOperation.Difference, true);

                canvas.ResetMatrix();

                canvas.DrawRect(0, 0, info.Width, info.Height, paint);
            };
        }
    }
}