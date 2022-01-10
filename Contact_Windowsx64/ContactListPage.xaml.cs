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
    public sealed partial class ContactListPage : Page
    {
        public ContactListPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Disabled;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            contactListPageListView.ItemsSource = ContactDatabase.CurrentInstance.GetContactsList().FavourateEditors;

            if(e.Parameter != null)
            {
                refreshEditor(((ContactFavourate)e.Parameter).ID);
            }
        }

        private void contactListPageListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ContactEditor item = ContactDatabase.CurrentInstance.GetContactEditor(((ContactFavourate)e.ClickedItem).ID);
            ContactListFrame.Navigate(typeof(ContactShowPage), item);
        }

        public void refreshEditor(Int32 ID)
        {
            for (var i = 0; i < contactListPageListView.Items.Count; i++)
            {
                if (((ContactFavourate)contactListPageListView.Items[i]).ID == ID)
                {
                    contactListPageListView.SelectedIndex = i;
                    break;
                }
            }
            //ContactEditor item = ContactDatabase.CurrentInstance.GetContactEditor(ID);
            //ContactListFrame.Navigate(typeof(ContactShowPage), item);
        }

        private void contactListPageListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                ContactEditor item = ContactDatabase.CurrentInstance.GetContactEditor(((ContactFavourate)e.AddedItems[0]).ID);
                ContactListFrame.Navigate(typeof(ContactShowPage), item);
            }
        }
    }
}
