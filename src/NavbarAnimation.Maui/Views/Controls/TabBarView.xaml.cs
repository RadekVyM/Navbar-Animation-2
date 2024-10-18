using Microsoft.Maui.Controls.Shapes;

namespace NavbarAnimation.Maui.Views.Controls;

public partial class TabBarView : ContentView
{
    private const string AnimationKey = "Animation";
    private const uint AnimationLength = 300;

    double currentPosition = 0;
    private double itemWidth => buttonsGrid.Width / (buttonsGrid?.Count ?? 1);
    ThumbDrawable thumbDrawable => graphicsView.Drawable as ThumbDrawable;

    public static readonly BindableProperty TabsPaddingProperty =
            BindableProperty.Create(nameof(TabsPadding), typeof(Thickness), typeof(TabBarView), defaultValue: Thickness.Zero, propertyChanged: OnTabsPaddingChanged);

    public Thickness TabsPadding
    {
        get => (Thickness)GetValue(TabsPaddingProperty);
        set => SetValue(TabsPaddingProperty, value);
    }

    public event Action<object, TabBarEventArgs> CurrentPageSelectionChanged;


    public TabBarView()
	{
		InitializeComponent();

        App.Current.Resources.TryGetValue("Primary", out object primaryColor);

        graphicsView.Drawable = new ThumbDrawable
        {
            Color = primaryColor as Color,
            ItemsCount = buttonsGrid?.Count ?? 1
        };

        buttonsGrid.SizeChanged += TabBarViewSizeChanged;
    }

    private void TabBarViewSizeChanged(object sender, EventArgs e)
    {
        UpdateAfterPositionChange();
    }

    private void UpdateAfterPositionChange()
    {
        bottomLayer.Clip = GetOuterClip(currentPosition);
        topLayer.Clip = GetSelectionClip(currentPosition);

        thumbDrawable.Position = currentPosition;
        graphicsView.Invalidate();
    }

    public void SetSelection(int value)
    {
        currentPosition = value;
        UpdateAfterPositionChange();
    }

    private async Task AnimateToPosition(int position)
    {
        this.AbortAnimation(AnimationKey);

        var animation = new Animation(v =>
        {
            currentPosition = v;
            UpdateAfterPositionChange();
        }, currentPosition, position, easing: Easing.SpringOut);

        animation.Commit(this, AnimationKey, length: AnimationLength);

        await Task.Delay((int)AnimationLength);
    }

    private Geometry GetSelectionClip(double position)
    {
        return new RectangleGeometry(new Rect(position * itemWidth, 0, itemWidth, buttonsGrid.Height));
    }

    private Geometry GetOuterClip(double position)
    {
        var leftWidth = Math.Max(position * itemWidth, 0);

        return new GeometryGroup
        {
            Children = new GeometryCollection
            {
                new RectangleGeometry(new Rect(0, 0, leftWidth, buttonsGrid.Height)),
                new RectangleGeometry(new Rect((position * itemWidth) + itemWidth, 0, buttonsGrid.Width - (position * itemWidth) - itemWidth, buttonsGrid.Height))
            }
        };
    }

    private void TabButtonClicked(PageType page, int position)
    {
        _ = AnimateToPosition(position);
        CurrentPageSelectionChanged?.Invoke(this, new TabBarEventArgs(page));
    }

    private void GamesButtonClicked(object sender, EventArgs e)
    {
        TabButtonClicked(PageType.GamesPage, 0);
    }

    private void AppsButtonClicked(object sender, EventArgs e)
    {
        TabButtonClicked(PageType.AppsPage, 1);
    }

    private void MoviesButtonClicked(object sender, EventArgs e)
    {
        TabButtonClicked(PageType.MoviesPage, 2);
    }

    private void BooksButtonClicked(object sender, EventArgs e)
    {
        TabButtonClicked(PageType.BooksPage, 3);
    }

    private void SearchButtonClicked(object sender, EventArgs e)
    {
        TabButtonClicked(PageType.SearchPage, 4);
    }

    private static void OnTabsPaddingChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var tabBarView = bindable as TabBarView;

        if (newValue is not Thickness padding)
            return;

        tabBarView.border.Padding = padding;
    }

    private class ThumbDrawable : IDrawable
    {
        private const float Margin = 10;
        private const float ThumbHeight = 10;

        public Color Color { get; set; }
        public double Position { get; set; } = 0;
        public int ItemsCount { get; set; }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.SaveState();

            float width = dirtyRect.Width - (2 * Margin);
            float selectedRectWidth = width / ItemsCount;
            float selectedRectPosition = Margin + (float)(Position * selectedRectWidth);

            var thumbPath = new PathF()
                .MoveTo(selectedRectPosition, 0)
                .LineTo(selectedRectPosition + selectedRectWidth, 0)
                .QuadTo(selectedRectPosition + selectedRectWidth, ThumbHeight, selectedRectPosition + selectedRectWidth - ThumbHeight, ThumbHeight)
                .LineTo(selectedRectPosition + ThumbHeight, ThumbHeight)
                .QuadTo(selectedRectPosition, ThumbHeight, selectedRectPosition, 0);
            thumbPath.Close();

            canvas.SetFillPaint(new SolidPaint(Color), dirtyRect);
            canvas.FillPath(thumbPath);

            canvas.RestoreState();
        }
    }
}