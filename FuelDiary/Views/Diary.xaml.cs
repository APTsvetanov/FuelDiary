using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FuelDiary.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Diary : Page
    {
        private StorageFolder storageFolder = ApplicationData.Current.LocalFolder;


        public Diary()
        {
            this.InitializeComponent();
            SetCurrentTrip();
        }

        private void btnCurrentTrip_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Views.CurrentTrip));

        }

        private void btnTripsPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Views.Trips));

        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
        

        public async void SetCurrentTrip()
        {
            try
            {
                StorageFile currentTripFile = await storageFolder.GetFileAsync("CurrentTrip.txt");
                var text = await FileIO.ReadTextAsync(currentTripFile);
                if (text.Length > 0)
                {
                    btnCurrentTrip.IsEnabled = true;
                }
                else
                {
                    btnCurrentTrip.IsEnabled = false;
                }
            }
            catch { }
        }


    }
}
