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
    public sealed partial class CurrentTrip : Page
    {
        private StorageFolder storageFolder = ApplicationData.Current.LocalFolder;

        public CurrentTrip()
        {
            this.InitializeComponent();
            InitializePage();
            
        }

        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            if (FinishValidData())
            {
                FinishTrip();
            }
            else
            {
                ShowIncorrectDataDialog();
            }
        }

        private async void FinishTrip()
        {
            var odometer = int.Parse(txtOdometer.Text);
            var fuel = int.Parse(txtFuel.Text);

            StorageFile currentTripFile = await storageFolder.GetFileAsync("CurrentTrip.txt");
            var JsonString = await FileIO.ReadTextAsync(currentTripFile);

            var currentTripValues = JsonConvert.DeserializeObject<Models.CurrentTrip>(JsonString);

            if (odometer > currentTripValues.Odometer && fuel < currentTripValues.Fuel)
            {
                ContentDialog FinishCurrentTripDialog = new ContentDialog
                {
                    Title = "FINISHING CURRENT TRIP!",
                    Content = "Do you really want to finish your current trip?",
                    CloseButtonText = "NO",
                    PrimaryButtonText = "YES"
                };

                ContentDialogResult result = await FinishCurrentTripDialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    StorageFile TripsFile = await storageFolder.GetFileAsync("Trips.txt");
                    var JsonTrips = await FileIO.ReadTextAsync(TripsFile);


                    List<Models.Trip> trips;
                    int number;
                    try
                    {
                        trips = JsonConvert.DeserializeObject<List<Models.Trip>>(JsonTrips);
                        number = 0;
                        foreach(var t in trips)
                        {
                            if(t.Number > number)
                            {
                                number = t.Number;
                            }
                        }
                        number++;
                    }
                    catch
                    {
                        trips = new List<Models.Trip>();
                        number = 1;
                    }
                    var distance = (double)odometer - (double)currentTripValues.Odometer;
                    var usedFuel = (double)currentTripValues.Fuel - (double)fuel;
                    var consumption = $"{Math.Round(usedFuel/(distance/100), 2)} {currentTripValues.FUnit}/100{currentTripValues.OUnit}";
                    var moneySpent = $"{currentTripValues.FPrice} {currentTripValues.Currency}";

                    var trip = new Models.Trip
                    {
                        Number = number,
                        Vehicle = currentTripValues.Vehicle,
                        Distance = (int)distance,
                        Odometer = odometer,
                        OUnit = currentTripValues.OUnit,
                        UsedFuel = (int)usedFuel,
                        FUnit = currentTripValues.FUnit,
                        FType = currentTripValues.FType,
                        Consumption = consumption,
                        MoneySpent = moneySpent
                    };

                    trips.Add(trip);

                    var tripsJSON = JsonConvert.SerializeObject(trips);

                    await FileIO.WriteTextAsync(TripsFile, tripsJSON);

                    ClearCurrentTrip();
                    Frame.Navigate(typeof(Views.Diary));
                }
            }
            else
            {
                ContentDialog IncorrectDataDialog = new ContentDialog
                {
                    Title = "Incorrect data!",
                    Content = "Fuel at the end of the trip must be less then this at the beggining or\nOdometer at the end of the trip must be more than this at the beggining!",
                    CloseButtonText = "OK"
                };

                ContentDialogResult result = await IncorrectDataDialog.ShowAsync();
            }
        }
        

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Views.Diary));
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddFuel();
        }

        private async void InitializePage()
        {
            InitCBoxes();
            await InitTextBlocks();
        }

        private void InitCBoxes()
        {
            CBoxFuelPrice.Items.Add("BGN");
            CBoxFuelPrice.Items.Add("EUR");
            CBoxFuelPrice.Items.Add("USD");
            CBoxFuelPrice.Items.Add("GBR");
            CBoxFuelPrice.Items.Add("CZK");
        }

        private async Task InitTextBlocks()
        {
            StorageFile currentTripFile = await storageFolder.GetFileAsync("CurrentTrip.txt");
            var JsonString = await FileIO.ReadTextAsync(currentTripFile);
            var currentTripValues = JsonConvert.DeserializeObject<Models.CurrentTrip>(JsonString);

            tbFuel.Text = currentTripValues.FUnit;
            tbFuel2.Text = currentTripValues.FUnit;
            tbOdometer.Text = currentTripValues.OUnit;
        }

        

        private async void AddFuel()
        {
            if (ValidAddBtnData())
            {
                try
                {

                    StorageFile currentTripFile = await storageFolder.GetFileAsync("CurrentTrip.txt");
                    var JsonString = await FileIO.ReadTextAsync(currentTripFile);
                    var currentTripValues = JsonConvert.DeserializeObject<Models.CurrentTrip>(JsonString);

                    double price = double.Parse(txtFuelPrice.Text);

                    int quantity = int.Parse(txtQuantity.Text);

                    double money = 0;

                    string currency = CBoxFuelPrice.SelectedItem.ToString();

                    money = currentTripValues.FPrice;
                    money += price * quantity;
                    currentTripValues.FPrice = money;

                    quantity = currentTripValues.Fuel;
                    quantity += int.Parse(txtQuantity.Text);
                    currentTripValues.Fuel = quantity;
                    
                    currentTripValues.Currency = currency;

                    txtFuelPrice.Text = "";
                    txtQuantity.Text = "";
                    CBoxFuelPrice.SelectedItem = null;

                    var currentTripJson = JsonConvert.SerializeObject(currentTripValues); 

                    await FileIO.WriteTextAsync(currentTripFile, currentTripJson);                    
                }
                catch
                {
                    ShowIncorrectDataDialog();
                }
            }
            else
            {
                ShowIncorrectDataDialog();
            }
        }

        private async void ShowIncorrectDataDialog()
        {
            ContentDialog IncorrectDataDialog = new ContentDialog
            {
                Title = "Incorrect data!",
                Content = "Please enter valid data into the fields!",
                CloseButtonText = "OK"
            };

            ContentDialogResult result = await IncorrectDataDialog.ShowAsync();
        }

        

        private async void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog CancelCurrentTripDialog = new ContentDialog
            {
                Title = "DELETING CURRENT TRIP DATA!",
                Content = "Do you really want to cancel your current trip?",
                CloseButtonText = "NO",
                PrimaryButtonText = "YES"
            };



            ContentDialogResult result = await CancelCurrentTripDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
                DeleteCurrentTrip();
        }


        private async void ClearCurrentTrip()
        {
            StorageFile currentTripFile = await storageFolder.GetFileAsync("CurrentTrip.txt");
            await FileIO.WriteTextAsync(currentTripFile, "");
            ContentDialog TripCancelledDialog = new ContentDialog
            {
                Title = "Trip finished!",
                Content = "Current trip has been saved!\nNow you can start a new trip from the main page!",
                CloseButtonText = "OK"
            };
            ContentDialogResult result = await TripCancelledDialog.ShowAsync();
            Frame.Navigate(typeof(Views.Diary));

        }

        private async void DeleteCurrentTrip()
        {
            StorageFile currentTripFile = await storageFolder.CreateFileAsync("CurrentTrip.txt", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(currentTripFile, "");
            ContentDialog TripCancelledDialog = new ContentDialog
            {
                Title = "Trip cancelled!",
                Content = "Current trip has been cancelled!\nNow you can start a new trip from the main page!",
                CloseButtonText = "OK"
            };
            ContentDialogResult result = await TripCancelledDialog.ShowAsync();
            Frame.Navigate(typeof(MainPage));
        }

        private bool ValidAddBtnData()
        {
            if (!(CBoxFuelPrice.SelectedItem == null))
            {
                if (CBoxFuelPrice.SelectedItem.ToString().Length < 0)
                    return false;
            }
            else return false;

            double price = -1;
            if (double.TryParse(txtFuelPrice.Text, out price))
            {
                if (price < 0 || price > 20)
                    return false;
            }
            else return false;

            int quantity = -1;
            if (int.TryParse(txtQuantity.Text, out quantity))
            {
                if (quantity < 0 || quantity > 300)
                    return false;
            }
            else return false;

            return true;
        }

        private bool FinishValidData()
        {
            if (txtFuelPrice.Text != null)
                if (txtFuelPrice.Text.Length > 0)
                    return false;
            if (txtQuantity.Text != null)
                if (txtQuantity.Text.Length > 0)
                    return false;

            int fuel = -1;
            if (int.TryParse(txtFuel.Text, out fuel))
            {
                if (fuel < 0)
                    return false;
            }
            else return false;

            int odometer = -1;
            if (int.TryParse(txtOdometer.Text, out odometer))
            {
                if (odometer < 0)
                    return false;
            }
            else return false;

            return true;
        }

    }
}
