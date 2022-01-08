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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/unable to open database ?LinkId=402352&clcid=0x409

namespace Contact_Windowsx64
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public ContactFavourate? item = null;

        public MainPage()
        {
            this.InitializeComponent();
            var x = new ContactDatabase();
        }

        private void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if(args.IsSettingsInvoked)
            {
                navigationViewFrame.Navigate(typeof(SettingPage));
                navigationViewFrame.CornerRadius = new CornerRadius(20);
                navigationViewFrame.Margin = new Thickness(12);
            }
            switch (args.InvokedItem)
            {
                case "常用联系人":
                    navigationViewFrame.Navigate(typeof(FavouratePage));
                    break;
                case "联系人列表":
                    if (item == null)
                        navigationViewFrame.Navigate(typeof(ContactListPage));
                    else
                        navigationViewFrame.Navigate(typeof(ContactListPage), item);
                    item = null;
                    navigationViewFrame.CornerRadius = new CornerRadius(0);
                    navigationViewFrame.Margin = new Thickness(0);
                    break;
                case "添加联系人":
                    navigationViewFrame.Navigate(typeof(ContactEditorPage));
                    navigationViewFrame.CornerRadius = new CornerRadius(20);
                    navigationViewFrame.Margin = new Thickness(12);
                    break;
                default:
                    break;
            }  
        }

        private void autoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput && sender.Text != "")
            {
                sender.ItemsSource = ContactDatabase.CurrentInstance.GetAutoSuggestItems(sender.Text).FavourateEditors;
            }
        }

        private void autoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if(args.ChosenSuggestion != null)
            {
                if (contactList.IsSelected)
                {
                    if (item != null)
                        (navigationViewFrame.Content as ContactListPage).refreshEditor(((ContactFavourate)item).ID);
                    item = null;
                }
                else
                    contactList.IsSelected = true;
            }
        }

        private void autoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            item = (ContactFavourate)args.SelectedItem;
        }

        public void FromFavouratePage(ContactFavourate item_)
        {
            item = item_;
            contactList.IsSelected = true;
        }
    }
}
