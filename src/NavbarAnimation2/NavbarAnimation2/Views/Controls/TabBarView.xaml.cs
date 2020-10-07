using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NavbarAnimation2
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabBarView : ContentView
    {
        double svgSize = 20;
        List<double> thumbPostions;
        List<double> boxViewPostions;

        public TabBarView()
        {
            thumbPostions = new List<double>();
            boxViewPostions = new List<double>();

            InitializeComponent();

            SizeChanged += TabBarViewSizeChanged;

            var pageEnums = Enum.GetValues(typeof(PageEnum)).Cast<PageEnum>();

            foreach (var page in pageEnums)
            {
                string path = "";
                string text = "";

                switch (page)
                {
                    case PageEnum.DevoirsPage:
                        path = "DevoirsPath";
                        text = "devoirs";
                        break;
                    case PageEnum.AgendaPage:
                        path = "AgendaPath";
                        text = "agenda";
                        break;
                    case PageEnum.NotesPage:
                        path = "NotesPath";
                        text = "notes";
                        break;
                    case PageEnum.MessagesPage:
                        path = "MessagesPath";
                        text = "messages";
                        break;
                    case PageEnum.AbsencesPage:
                        path = "AbsencesPath";
                        text = "absences";
                        break;
                }

                var svg = new TabSvgView
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    Colour = App.Current.Resources.GetValue<Color>("TabBarColour"),
                    Path = App.Current.Resources.GetValue<string>(path),
                    SvgHeight = svgSize,
                    SvgWidth = svgSize,
                    Page = page,
                    Text = text,
                    Selected = page == PageEnum.DevoirsPage
                };

                TapGestureRecognizer recognizer = new TapGestureRecognizer();
                recognizer.Tapped += RecognizerTapped;

                svg.GestureRecognizers.Add(recognizer);

                stack.Children.Add(svg);
            }
        }

        private void TabBarViewSizeChanged(object sender, EventArgs e)
        {
            thumbCanvasView.WidthRequest = stack.Children[0].Width * 0.8d;
            boxView.WidthRequest = stack.Children[0].Width;

            double margin = (Width - stack.Width) / 2d;

            thumbPostions.Clear();
            boxViewPostions.Clear();

            TabSvgView selectedSvgView = null;

            for (int i = 0; i < stack.Children.Count; i++)
            {
                TabSvgView svgView = stack.Children[i] as TabSvgView;

                boxViewPostions.Add(margin + (i * (stack.Width / stack.Children.Count)));
                thumbPostions.Add(margin + (i * (stack.Width / stack.Children.Count)) + (((stack.Width / stack.Children.Count) - thumbCanvasView.WidthRequest) / 2d));

                if (svgView.Selected)
                    selectedSvgView = svgView;
            }

            if (selectedSvgView != null)
            {
                thumbCanvasView.TranslationX = thumbPostions[(int)selectedSvgView.Page];
                boxView.TranslationX = boxViewPostions[(int)selectedSvgView.Page];
            }
        }

        private void RecognizerTapped(object sender, EventArgs e)
        {
            TabSvgView svgView = sender as TabSvgView;
            TabSvgView selectedSvgView = stack.Children.FirstOrDefault(v => ((TabSvgView)v).Selected) as TabSvgView;

            Shell.Current.GoToAsync("///" + svgView.Page.ToString());

            selectedSvgView.Selected = false;
            svgView.Selected = true;

            uint animLength = 300;

            Animation thumbAnimation = new Animation(v => thumbCanvasView.TranslationX = v, thumbCanvasView.TranslationX, thumbPostions[(int)svgView.Page]);
            Animation boxViewAnimation = new Animation(v => boxView.TranslationX = v, boxView.TranslationX, boxViewPostions[(int)svgView.Page]);

            Animation animation = new Animation();

            animation.Add(0, 1, thumbAnimation);
            animation.Add(0, 1, boxViewAnimation);

            animation.Commit(this, "Animation", finished: (d,b) => 
            {
                thumbCanvasView.TranslationX = thumbPostions[(int)svgView.Page];
                boxView.TranslationX = boxViewPostions[(int)svgView.Page];
            }, length: animLength);
        }

        private void ThumbCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            var info = e.Info;
            canvas.Clear();

            using(SKPaint paint = new SKPaint())
            {
                paint.Color = App.Current.Resources.GetValue<Color>("MainColour").ToSKColor();
                paint.IsAntialias = true;

                canvas.DrawRoundRect(0, -info.Height, info.Width, info.Height * 2f, info.Height, info.Height, paint);
            }
        }
    }
}