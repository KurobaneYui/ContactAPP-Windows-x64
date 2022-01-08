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
    public sealed partial class ContactShowPage : Page
    {
        public ContactEditor? _item;

        public ContactShowPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Disabled;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter != null)
            {
                _item = (ContactEditor)e.Parameter;
                ContactEditor item = (ContactEditor)_item;

                ImageBrush brush = new ImageBrush();
                brush.ImageSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(this.BaseUri, item.ProfileImage));
                profileImage.Fill = brush;

                Windows.UI.Color color = item.IsFavourate ? Windows.UI.Colors.Red : Windows.UI.Colors.Gray;
                ((favourate.Content as FontIcon).Foreground as SolidColorBrush).Color = color;
                lastName.Text = item.LastName;
                firstName.Text = item.FirstName;
                company.Text = item.Company;
                job.Text = item.Job;
                birthday.Text = item.Birthday;
                address.Text = item.Addr;

                TextBox[] tel = { telNum1, telNum2, telNum3 };
                TextBox[] email = { emailAddr1, emailAddr2, emailAddr3 };
                for (var i = 0; i < tel.Length; i++)
                {
                    if (i < item.TelInfos.Count)
                    {
                        tel[i].Text = item.TelInfos[i].TelNumber;
                        color = item.TelInfos[i].IsFavourate ? Windows.UI.Colors.Red : Windows.UI.Colors.Gray;
                        ((((tel[i].Header as StackPanel).Children[1] as Button).Content as FontIcon).Foreground as SolidColorBrush).Color = color;
                    }
                    else
                    {
                        tel[i].Visibility = Visibility.Collapsed;
                    }
                }
                for (var i = 0; i < email.Length; i++)
                {
                    if (i < item.EmailInfos.Count)
                    {
                        email[i].Text = item.EmailInfos[i].EmailAddress;
                        color = item.EmailInfos[i].IsFavourate ? Windows.UI.Colors.Red : Windows.UI.Colors.Gray;
                        ((((email[i].Header as StackPanel).Children[1] as Button).Content as FontIcon).Foreground as SolidColorBrush).Color = color;
                    }
                    else
                    {
                        email[i].Visibility = Visibility.Collapsed;
                    }
                }
            }
            else
            {
                _item = null;
            }
        }

        private void Edit_Button_Click(object sender, RoutedEventArgs e)
        {
            if (_item != null)
            {
                (this.Parent as Frame).Navigate(typeof(ContactEditorPage), _item);
            }
            else
            {
                (this.Parent as Frame).Navigate(typeof(ContactEditorPage));
            }
        }

        private void Favourate_Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                FontIcon icon = button.Content as FontIcon;
                if (icon != null)
                {
                    if ((icon.Foreground as SolidColorBrush).Color == Windows.UI.Colors.Red)
                    {
                        icon.Foreground = new SolidColorBrush(Windows.UI.Colors.Gray);
                    }
                    else
                    {
                        icon.Foreground = new SolidColorBrush((Windows.UI.Colors.Red));
                    }
                }
            }

            QuickCommit();
        }

        private void QuickCommit()
        {
            ContactEditor item;
            if (_item != null)
            {
                item = (ContactEditor)_item;
                item.IsFavourate = ((favourate.Content as FontIcon).Foreground as SolidColorBrush).Color == Windows.UI.Colors.Red;
                // TODO: add function for every telInfo and emailInfo
                ContactDatabase.CurrentInstance.InsertOrUpdate(ref item);
            }
        }

        private void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            if(_item != null)
            {
                ContactDatabase.CurrentInstance.DeleteContact(((ContactEditor)_item).ID);
            }
            (((((this.Parent as Frame).Parent as Grid).Children[0] as StackPanel).Children[1] as ScrollViewer).Content as ListView).ItemsSource = ContactDatabase.CurrentInstance.GetContactsList().FavourateEditors;
            (this.Parent as Frame).Content = null;
        }
    }
}
