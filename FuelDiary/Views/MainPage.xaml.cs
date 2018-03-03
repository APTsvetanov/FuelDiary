using System;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage;


namespace FuelDiary
{
    public sealed partial class MainPage : Page
    {

        private StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
        
        public MainPage()
        {
            this.InitializeComponent();
            SetNewTrip();
        }
        
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            CoreApplication.Exit();
        }

        private void btnNewTrip_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Views.NewTrip));            
        }

        private void btnDiaryPage_Click(object sender, RoutedEventArgs e)
        {
            
            Frame.Navigate(typeof(Views.Diary));
        }

        

        public async void SetNewTrip()
        {
            try
            {
                StorageFile currentTripFile = await storageFolder.GetFileAsync("CurrentTrip.txt");
                var text = await FileIO.ReadTextAsync(currentTripFile);
                if (text.Length > 0)
                {
                    btnNewTrip.IsEnabled = false;
                }
                else
                {
                    btnNewTrip.IsEnabled = true;
                }
            }
            catch { }
        }

    }
}
