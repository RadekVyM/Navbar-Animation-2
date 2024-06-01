using NavbarAnimation.Maui.Views.Pages;
using SimpleToolkit.Core;

namespace NavbarAnimation.Maui;

public partial class AppShell : SimpleToolkit.SimpleShell.SimpleShell
{
    public AppShell()
    {
        InitializeComponent();

        AddTab(typeof(DevoirsPage), PageType.DevoirsPage);
        AddTab(typeof(AgendaPage), PageType.AgendaPage);
        AddTab(typeof(NotesPage), PageType.NotesPage);
        AddTab(typeof(MessagesPage), PageType.MessagesPage);
        AddTab(typeof(AbsencesPage), PageType.AbsencesPage);

        tabBarView.SetSelection(0);

        Loaded += AppShellLoaded;
    }


    private static void AppShellLoaded(object sender, EventArgs e)
    {
        var shell = sender as AppShell;

        shell.Window.SubscribeToSafeAreaChanges(safeArea =>
        {
            shell.tabBarView.Margin = new Thickness(safeArea.Left, safeArea.Top, safeArea.Right, 0);
            shell.tabBarView.TabsPadding = new Thickness(0, 0, 0, safeArea.Bottom);
        });
    }

    private void AddTab(Type page, PageType pageEnum)
    {
        var tab = new Tab { Route = pageEnum.ToString(), Title = pageEnum.ToString() };
        tab.Items.Add(new ShellContent { ContentTemplate = new DataTemplate(page) });

        tabBar.Items.Add(tab);
    }

    private void TabBarViewCurrentPageChanged(object sender, TabBarEventArgs e)
    {
        Shell.Current.GoToAsync("///" + e.CurrentPage.ToString());
    }
}

public enum PageType
{
    DevoirsPage, AgendaPage, NotesPage, MessagesPage, AbsencesPage
}