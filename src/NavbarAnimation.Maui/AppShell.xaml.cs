using NavbarAnimation.Maui.Views.Pages;

namespace NavbarAnimation.Maui
{
    public partial class AppShell : OverlayShell.OverlayShell
    {
        public AppShell()
        {
            InitializeComponent();

            AddTab(typeof(DevoirsPage), PageType.DevoirsPage);
            AddTab(typeof(AgendaPage), PageType.AgendaPage);
            AddTab(typeof(NotesPage), PageType.NotesPage);
            AddTab(typeof(MessagesPage), PageType.MessagesPage);
            AddTab(typeof(AbsencesPage), PageType.AbsencesPage);
        }

        private void AddTab(Type page, PageType pageEnum)
        {
            Tab tab = new Tab { Route = pageEnum.ToString(), Title = pageEnum.ToString() };
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
}