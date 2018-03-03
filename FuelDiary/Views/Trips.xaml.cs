using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FuelDiary.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Trips : Page
    {
        private StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
        public List<Models.Trip> trips;

        public Trips()
        {
            this.InitializeComponent();
            InitPage();
        }

        private async void InitPage()
        {
            await InitTrips();
        }

        private async Task InitTrips()
        {
            StorageFile tripsFile = await storageFolder.GetFileAsync("Trips.txt");
            var JsonString = await FileIO.ReadTextAsync(tripsFile);
            trips = JsonConvert.DeserializeObject<List<Models.Trip>>(JsonString);

            trips.Reverse();
            ContentView.ItemsSource = trips;            
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Views.Diary));
        }

        private  void ContentView_ItemClick(object sender, ItemClickEventArgs e)
        {
            
        }

        private async void ContentView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await SelectTrip();
            Frame.Navigate(typeof(Views.SelctedTrip));
        }

        private async Task SelectTrip()
        {
            var obj = ContentView.SelectedItem;
            var selectedTrip = (Models.Trip)obj;

            StorageFile selectedTripFile =
                await storageFolder.CreateFileAsync("selectedTrip.txt", CreationCollisionOption.ReplaceExisting);

            var sTripJSON = JsonConvert.SerializeObject(selectedTrip);

            await FileIO.WriteTextAsync(selectedTripFile, sTripJSON);
        }
    }
}
