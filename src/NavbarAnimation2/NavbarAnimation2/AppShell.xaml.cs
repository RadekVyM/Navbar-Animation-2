using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NavbarAnimation2
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            AddTab(typeof(DevoirsPage), PageEnum.DevoirsPage);
            AddTab(typeof(AgendaPage), PageEnum.AgendaPage);
            AddTab(typeof(NotesPage), PageEnum.NotesPage);
            AddTab(typeof(MessagesPage), PageEnum.MessagesPage);
            AddTab(typeof(AbsencesPage), PageEnum.AbsencesPage);
        }

        private void AddTab(Type page, PageEnum pageEnum)
        {
            Tab tab = new Tab { Route = pageEnum.ToString(), Title = pageEnum.ToString() };
            tab.Items.Add(new ShellContent { ContentTemplate = new DataTemplate(page) });

            tabBar.Items.Add(tab);
        }
    }

    public enum PageEnum
    {
        DevoirsPage, AgendaPage, NotesPage, MessagesPage, AbsencesPage
    }
}