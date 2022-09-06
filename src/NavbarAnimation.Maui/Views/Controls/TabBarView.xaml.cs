using Microsoft.Maui.Controls.Shapes;

namespace NavbarAnimation.Maui.Views.Controls;

public partial class TabBarView : ContentView
{
    bool initialSizeChange = true;
    TabBarViewDrawable drawable;
    int currentPosition = 0;
    private readonly float graphicsViewMargin = 10;

    public event Action<object, TabBarEventArgs> CurrentPageSelectionChanged;


    public TabBarView()
	{
		InitializeComponent();
        drawable = CreateDrawable();

        drawable.IconHeight = 20;

        graphicsView.Drawable = drawable;
        graphicsView.Invalidate();
    }


    private async Task AnimateToPosition(int position)
    {
        uint animationLength = 300;

        var animation = new Animation(v =>
        {
            drawable.SelectedRectPosition = (float)v;
            graphicsView.Invalidate();
        }, drawable.SelectedRectPosition, position, easing: Easing.SpringOut);

        animation.Commit(this, "Animation", length: animationLength);

        await Task.Delay((int)animationLength);

        currentPosition = position;
    }

    private void GraphicsViewSizeChanged(object sender, EventArgs e)
    {
        if (initialSizeChange && graphicsView.Width != 0)
        {
            SetDefaultSelection();
            initialSizeChange = false;
        }
    }

    private void SetDefaultSelection()
    {
        drawable.SelectedRectPosition = 0;
        graphicsView.Invalidate();
    }

    private TabBarViewDrawable CreateDrawable()
    {
        var drawable = new TabBarViewDrawable(new List<TabContent>
        {
            new TabContent(GetGeometry("DevoirsTextPath"), GetGeometry("DevoirsPath")),
            new TabContent(GetGeometry("AgendaTextPath"), GetGeometry("AgendaPath")),
            new TabContent(GetGeometry("NotesTextPath"), GetGeometry("NotesPath")),
            new TabContent(GetGeometry("MessagesTextPath"), GetGeometry("MessagesPath")),
            new TabContent(GetGeometry("AbsencesTextPath"), GetGeometry("AbsencesPath")),
        }, graphicsViewMargin);

        drawable.SelectionColor = GetColor("Primary");
        drawable.DefaultColor = GetColor("BackColor");
        drawable.TabBarColor = GetColor("TabBarColor");

        return drawable;
    }

    private Color GetColor(string key)
    {
        App.Current.Resources.TryGetValue(key, out object color);

        return color as Color;
    }

    private PathGeometry GetGeometry(string key)
    {
        App.Current.Resources.TryGetValue(key, out object stringPath);
        var geometry = new PathGeometryConverter().ConvertFromInvariantString(stringPath.ToString()) as PathGeometry;

        return geometry;
    }

    private void TabButtonClicked(PageType page, int position)
    {
        _ = AnimateToPosition(position);
        CurrentPageSelectionChanged?.Invoke(this, new TabBarEventArgs(page));
    }

    private void DevoirsButtonClicked(object sender, EventArgs e)
    {
        TabButtonClicked(PageType.DevoirsPage, 0);
    }

    private void AgendaButtonClicked(object sender, EventArgs e)
    {
        TabButtonClicked(PageType.AgendaPage, 1);
    }

    private void NotesButtonClicked(object sender, EventArgs e)
    {
        TabButtonClicked(PageType.NotesPage, 2);
    }

    private void MessagesButtonClicked(object sender, EventArgs e)
    {
        TabButtonClicked(PageType.MessagesPage, 3);
    }

    private void AbsencesButtonClicked(object sender, EventArgs e)
    {
        TabButtonClicked(PageType.AbsencesPage, 4);
    }
}