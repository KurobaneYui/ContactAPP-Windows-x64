using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Contact_Windowsx64
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FavouratePage : Page
    {
        public FavouratePage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Disabled;
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ContactFavourate item = (ContactFavourate)e.ClickedItem;
            MainPage page = ((this.Parent as Frame).Parent as NavigationView).Parent as MainPage;
            page.FromFavouratePage(item);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            favourateGridView.ItemsSource = ContactDatabase.CurrentInstance.GetFavourateContacts().FavourateEditors;
        }

        private void Favourate_Button_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as Button;
            if (item != null)
            {
                Int32 id = (Int32)item.CommandParameter;
                ContactEditor editor = ContactDatabase.CurrentInstance.GetContactEditor(id);
                var icon = item.Content as FontIcon;
                if (icon != null)
                {
                    editor.IsFavourate = !((icon.Foreground as SolidColorBrush).Color == Windows.UI.Colors.Red);
                    icon.Foreground = editor.IsFavourate ? new SolidColorBrush(Windows.UI.Colors.Red) : new SolidColorBrush(Windows.UI.Colors.Gray);
                    ContactDatabase.CurrentInstance.InsertOrUpdate(ref editor);
                }
            }
        }
    }
}
