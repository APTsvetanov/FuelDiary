using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Newtonsoft.Json;
using System.Threading.Tasks;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FuelDiary.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NewTrip : Page
    {
        private StorageFolder storageFolder = ApplicationData.Current.LocalFolder;


        public NewTrip()
        {
            this.InitializeComponent();
            InitPage();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            StartNextTrip();
        }

        private async void StartNextTrip()
        {
            if (CorrectDataInput())
            {
                await SaveDataToCurrentTripFileAsync();
                Frame.Navigate(typeof(Views.CurrentTrip));
            }
            else
            {
                ContentDialog IncorrectDataDialog = new ContentDialog
                {
                    Title = "Incorrect data!",
                    Content = "Please enter valid data into the fields!",
                    CloseButtonText = "OK"
                };
                
                ContentDialogResult result = await IncorrectDataDialog.ShowAsync();
            }
        }

        private void TogBtnSelectVehicle_Checked(object sender, RoutedEventArgs e)
        {
            txtVehicle.IsEnabled = false;
            ComboVehicles.IsEnabled = true;
        }

        private void TogBtnSelectVehicle_Unchecked(object sender, RoutedEventArgs e)
        {
            txtVehicle.IsEnabled = true;
            ComboVehicles.IsEnabled = false;
        }

        private async void InitPage()
        {
            InitializeComboBoxes();
            await InitVehicles();
        }
        
        private void InitializeComboBoxes()
        {
            CBoxFuelType.Items.Add("Gasoline");
            CBoxFuelType.Items.Add("Gas");
            CBoxFuelType.Items.Add("Diesel");

            ComboFuel.Items.Add("lit");
            ComboFuel.Items.Add("gal");

            ComboOdometer.Items.Add("km");
            ComboOdometer.Items.Add("miles");

            ComboVehicles.IsEnabled = false;
        }

        private async Task InitVehicles()
        {
            StorageFile tripsFile = await storageFolder.GetFileAsync("Trips.txt");
            var JsonString = await FileIO.ReadTextAsync(tripsFile);
            var trips = JsonConvert.DeserializeObject<List<Models.Trip>>(JsonString);

            HashSet<string> vehicles = new HashSet<string>();

            foreach (var t in trips)
            {
                vehicles.Add(t.Vehicle);
            }            

            foreach(var v in vehicles)
            {
                ComboVehicles.Items.Add(v);
            }
        }

        private async Task SaveDataToCurrentTripFileAsync()
        {
            ContentDialog TripStartedDialog = new ContentDialog
            {
                Title = "Trip started!",
                Content = "You can finish the trip whenever you want from current trip page!",
                CloseButtonText = "OK"
            };


            string vehicle;
            if (txtVehicle.IsEnabled)
                vehicle = txtVehicle.Text;
            else
                vehicle = ComboVehicles.SelectedItem.ToString();

            var odometer = int.Parse(txtOdometer.Text);
            string oUnit = ComboOdometer.SelectedItem.ToString();
            string fType = CBoxFuelType.SelectedItem.ToString();
            string fUnit = ComboFuel.SelectedItem.ToString();
            var fuel = int.Parse(txtFuel.Text);

            var currentTrip = new Models.CurrentTrip()
            {
                Vehicle = vehicle,
                Odometer = odometer,
                OUnit = oUnit,
                Fuel = fuel,
                FUnit = fUnit,
                FType = fType,
                FPrice = 0,
                Currency = "BGN"
            };

            var currentTripString = JsonConvert.SerializeObject(currentTrip);

            StorageFile currentTripFile = await storageFolder.GetFileAsync("CurrentTrip.txt");
            await FileIO.WriteTextAsync(currentTripFile, currentTripString);

            ContentDialogResult result = await TripStartedDialog.ShowAsync();
        }
        

        private bool CorrectDataInput()
        {
            if(txtVehicle.IsEnabled && txtVehicle.Text != null)
            {
                if (txtVehicle.Text.Length < 1 || txtVehicle.Text.Length > 15)
                    return false;
            }
            else
            {
                if (txtVehicle.IsEnabled == true)
                    return false;
                if (ComboVehicles.IsEnabled == true)
                {
                    if (ComboVehicles.SelectedItem == null)
                        return false;
                }
            }
            int odometer = -1;
            if (int.TryParse(txtOdometer.Text, out odometer))
            {
                if (odometer < 0 || odometer > 999999)
                    return false;
            }
            else return false;
            int fuel = -1;
            if (int.TryParse(txtFuel.Text, out fuel))
            {
                if (fuel < 0 || fuel > 300)
                    return false;
            }
            else return false;
            if (!(ComboFuel.SelectedItem == null))
            {
                if (ComboFuel.SelectedItem.ToString().Length < 0)
                    return false;
            }
            else return false;
            if (!(CBoxFuelType.SelectedItem == null))
            {
                if (CBoxFuelType.SelectedItem.ToString().Length < 0)
                    return false;
            }
            else return false;
            if (!(ComboOdometer.SelectedItem == null))
            {
                if (ComboOdometer.SelectedItem.ToString().Length < 0)
                    return false;
            }
            else return false;

            return true;
        }
    }
}
