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
using System.Drawing;
using Windows.Storage;
using Windows.Storage.Pickers;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Contact_Windowsx64
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ContactEditorPage : Page
    {
        private ContactEditor? _item;
        private MemoryStream _stream;

        public ContactEditorPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Disabled;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (_stream != null)
            {
                _stream.Close();
                _stream = null;
            }

            if (e.Parameter != null)
            {
                _item = (ContactEditor)e.Parameter;
                ContactEditor item = (ContactEditor)_item;

                Windows.UI.Xaml.Media.Imaging.BitmapImage img = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(this.BaseUri, item.ProfileImage));
                img.CreateOptions = Windows.UI.Xaml.Media.Imaging.BitmapCreateOptions.IgnoreImageCache;
                ((profileImage.Content as Windows.UI.Xaml.Shapes.Ellipse).Fill as ImageBrush).ImageSource = img;

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
                for(var i =0;i<item.TelInfos.Count;i++)
                {
                    tel[i].Text = item.TelInfos[i].TelNumber;
                    color = item.TelInfos[i].IsFavourate ? Windows.UI.Colors.Red : Windows.UI.Colors.Gray;
                    ((((tel[i].Header as StackPanel).Children[1] as Button).Content as FontIcon).Foreground as SolidColorBrush).Color = color;
                }
                for (var i = 0; i < item.EmailInfos.Count; i++)
                {
                    email[i].Text = item.EmailInfos[i].EmailAddress;
                    color = item.EmailInfos[i].IsFavourate ? Windows.UI.Colors.Red : Windows.UI.Colors.Gray;
                    ((((email[i].Header as StackPanel).Children[1] as Button).Content as FontIcon).Foreground as SolidColorBrush).Color = color;
                }
            }
            else
            {
                _item = null;
            }
        }

        private void Commit_Button_Click(object sender, RoutedEventArgs e)
        {
            ContactEditor item;
            if (_item != null)
            {
                item = (ContactEditor)_item;
                item.TelInfos.Clear();
                item.EmailInfos.Clear();
            }
            else
            {
                item = new ContactEditor(-1);
            }

            item.LastName = lastName.Text.Trim();
            item.FirstName = firstName.Text.Trim();
            item.Company = company.Text.Trim();
            item.Job = job.Text.Trim();
            item.Birthday = birthday.Text.Trim();
            item.Addr = address.Text.Trim();

            item.IsFavourate = ((favourate.Content as FontIcon).Foreground as SolidColorBrush).Color == Windows.UI.Colors.Red;

            TextBox[] tel = { telNum1, telNum2, telNum3 };
            TextBox[] email = { emailAddr1, emailAddr2, emailAddr3 };
            foreach(TextBox b in tel)
            {
                if(b.Text.Trim() != "")
                {
                    bool isfavourate = ((((b.Header as StackPanel).Children[1] as Button).Content as FontIcon).Foreground as SolidColorBrush).Color == Windows.UI.Colors.Red;
                    TelInfo telinfo = new TelInfo(b.Text.Trim(), isfavourate);
                    item.TelInfos.Add(telinfo);
                }
            }
            foreach (TextBox b in email)
            {
                if (b.Text.Trim() != "")
                {
                    bool isfavourate = ((((b.Header as StackPanel).Children[1] as Button).Content as FontIcon).Foreground as SolidColorBrush).Color == Windows.UI.Colors.Red;
                    EmailInfo emailinfo = new EmailInfo(b.Text.Trim(), isfavourate);
                    item.EmailInfos.Add(emailinfo);
                }
            }

            if (_stream != null)
            {
                item.ProfileImage = @".\id.png";
            }

            ContactDatabase.CurrentInstance.InsertOrUpdate(ref item);

            if (_stream != null)
            {
                FileStream ori = new FileStream(item.ProfileImage, FileMode.OpenOrCreate);
                ori.Seek(0, SeekOrigin.Begin);
                ori.Write(_stream.ToArray(), 0, _stream.ToArray().Length);
                ori.Flush();
                ori.Close();
                _stream.Close();
                _stream = null;
            }

            if (((this.Parent as Frame).Parent as NavigationView) == null)
            {
                (this.Parent as Frame).Navigate(typeof(ContactShowPage), item);
            }
            else
            {
                ContactFavourate i = new ContactFavourate(item.ID);
                (((this.Parent as Frame).Parent as NavigationView).Parent as MainPage).FromFavouratePage(i);
            }
        }

        private void Favourate_Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if(button != null)
            {
                FontIcon icon = button.Content as FontIcon;
                if(icon != null)
                {
                    if((icon.Foreground as SolidColorBrush).Color==Windows.UI.Colors.Red)
                    {
                        icon.Foreground = new SolidColorBrush(Windows.UI.Colors.Gray);
                    }
                    else
                    {
                        icon.Foreground = new SolidColorBrush((Windows.UI.Colors.Red));
                    }
                }
            }
        }

        private async void ProfilImage_Button_Click(object sender, RoutedEventArgs e)
        {
            // TODO: add image upload function
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".bmp");
            openPicker.FileTypeFilter.Add(".webp");

            StorageFile file = await openPicker.PickSingleFileAsync();
            if(file != null)
            {
                _stream = ImageHandler.SaveImageToAPP((await file.OpenReadAsync()).AsStreamForRead());
                FileStream fs = new FileStream(ApplicationData.Current.LocalFolder.Path + @"\tmp.png", FileMode.OpenOrCreate);
                fs.Seek(0, SeekOrigin.Begin);
                fs.Write(_stream.ToArray(), 0, _stream.ToArray().Length);
                fs.Flush();
                fs.Close();

                Windows.UI.Xaml.Media.Imaging.BitmapImage img = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(this.BaseUri, ApplicationData.Current.LocalFolder.Path + @"\tmp.png"));
                img.CreateOptions = Windows.UI.Xaml.Media.Imaging.BitmapCreateOptions.IgnoreImageCache;
                ((profileImage.Content as Windows.UI.Xaml.Shapes.Ellipse).Fill as ImageBrush).ImageSource = img;
            }
            else
            {
                _stream = null;
            }
        }
    }
}
