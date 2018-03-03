using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FuelDiary.Views
{
    public sealed partial class SelctedTrip : Page
    {
        private StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
        private Models.Trip selectedTrip;
        private string avgConsumption;
        private string tMoneyCost;


        public SelctedTrip()
        {
            this.InitializeComponent();
            InitPage();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Views.Trips));
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog cd = new ContentDialog()
            {
                Title = "DELETING SELECTED TRIP!",
                Content = "Are you sure for deleting this trip from your FUEL DIARY?",
                CloseButtonText = "No",
                PrimaryButtonText = "Yes"
            };

            ContentDialogResult cdResult = await cd.ShowAsync();

            if (cdResult == ContentDialogResult.Primary)
            {
                await DeleteTrip();
                Frame.Navigate(typeof(Views.Trips));
            }
        }

        private async Task DeleteTrip()
        {
            var TripsFile = await storageFolder.GetFileAsync("Trips.txt");
            var TripsJSON = await FileIO.ReadTextAsync(TripsFile);
            var trips = JsonConvert.DeserializeObject<List<Models.Trip>>(TripsJSON);
            var index = -1;
            var num = 0;
            foreach (var t in trips)
            {
                if (t.Number == selectedTrip.Number)
                {
                    index = trips.IndexOf(t);
                    num = t.Number;
                    break;
                }
            }
            if (index >= 0)
                trips.RemoveAt(index);
            foreach (var t in trips)
            {
                if (t.Number > num)
                    t.Number--;
            }
            var updatedTrips = JsonConvert.SerializeObject(trips);
            await FileIO.WriteTextAsync(TripsFile, updatedTrips);
        }

        private async void InitPage()
        {
            await SetSelectedTrip();
            await SetAvgConsumption();
            await SetTotalMoneyCostConsumption();
            SetPage();
        }

        private async Task SetSelectedTrip()
        {
            var sTripFile = await storageFolder.GetFileAsync("selectedTrip.txt");
            var sTripJSON = await FileIO.ReadTextAsync(sTripFile);
            selectedTrip = JsonConvert.DeserializeObject<Models.Trip>(sTripJSON);
        }

        private void SetPage()
        {
            tbNumber.Text = selectedTrip.Number.ToString();
            tbVehicle.Text = selectedTrip.Vehicle;
            tbDistance.Text = selectedTrip.Distance.ToString();
            tbOdometer.Text = selectedTrip.Odometer.ToString();
            tbFuel.Text = selectedTrip.UsedFuel.ToString();
            tbConsumptionFuel.Text = selectedTrip.Consumption;
            tbMoney.Text = selectedTrip.MoneySpent;
            tbAvgConsumption.Text = avgConsumption;
            tbTotalMoneySpent.Text = tMoneyCost;
        }


        private async Task SetAvgConsumption()
        {
            var TripsFile = await storageFolder.GetFileAsync("Trips.txt");
            var TripsJSON = await FileIO.ReadTextAsync(TripsFile);
            var allTrips = JsonConvert.DeserializeObject<List<Models.Trip>>(TripsJSON);

            List<Models.Trip> selectedCarTrips = new List<Models.Trip>();
            foreach (var t in allTrips)
            {
                if (t.Vehicle == selectedTrip.Vehicle)
                {
                    selectedCarTrips.Add(t);
                }
            }

            var avgFuel = 0d;
            var units = selectedTrip.Consumption.Split().ToArray()[1];
            var fUnit = units.Split('/').ToArray()[0];
            var oUnit = units.Substring(3);

            foreach(var t in selectedCarTrips)
            {
                var fuel = double.Parse(t.Consumption.Split().ToArray()[0]);
                var currUnits = t.Consumption.Split().ToArray()[1];
                var currFunit = currUnits.Split('/').ToArray()[0];
                var currOunit = currUnits.Substring(3);

                if (fUnit == currFunit && oUnit == currOunit)
                    avgFuel += fuel;
                else
                {
                    if(fUnit != currFunit)
                    {
                        fuel = FuelConvert.GetConvertedFuel(fuel, currFunit, fUnit);
                    }
                    if(oUnit != currOunit)
                    {
                        fuel = FuelConvert.GetDifferentMetricsFuel(fuel, currOunit, oUnit);
                    }
                    avgFuel += fuel;
                }
            }

            avgFuel /= selectedCarTrips.Count;

            avgConsumption = $"{Math.Round(avgFuel)} {fUnit}{oUnit}";
        }
        
        public static class FuelConvert
        {
            public static double GetConvertedFuel(double value, string inFUnit, string outFUnit)
            {
                double result;
                
                if (inFUnit == "lit")
                    result = LitToGal(value);
                else
                    result = GalToLit(value);

                return result;
            }

            public static double GetDifferentMetricsFuel(double value, string inMetric, string outMetric)
            {
                double result;

                if (inMetric == "km")
                    result = KmToMiles(value);
                else
                    result = MilesToKm(value);

                return result;
            }

            private static double LitToGal(double val)
            {
                return val * 0.264172052;
            }

            private static double GalToLit(double val)
            {
                return val * 3.78541178;
            }

            public static double KmToMiles(double val)
            {
                return (val / (0.621371192 * 100)) * 100;
            }

            public static double MilesToKm(double val)
            {
                return (val / (1.609344 * 100)) * 100 ;
            }
        }

        private async Task SetTotalMoneyCostConsumption()
        {
            var TripsFile = await storageFolder.GetFileAsync("Trips.txt");
            var TripsJSON = await FileIO.ReadTextAsync(TripsFile);
            var allTrips = JsonConvert.DeserializeObject<List<Models.Trip>>(TripsJSON);

            List<Models.Trip> selectedCarTrips = new List<Models.Trip>();
            foreach (var t in allTrips)
            {
                if (t.Vehicle == selectedTrip.Vehicle)
                {
                    selectedCarTrips.Add(t);
                }
            }

            var totalMoneySpent = 0d;
            var currency = selectedTrip.MoneySpent.Split().ToArray()[1];

            foreach (var t in selectedCarTrips)
            {
                var money = double.Parse(t.MoneySpent.Split().ToArray()[0]);
                var curr = t.MoneySpent.Split().ToArray()[1];

                if (curr == currency)
                    totalMoneySpent += money;
                else
                    totalMoneySpent += CurrencyConvert.GetConvertedCurrency(money, curr, currency);
            }

            tMoneyCost = $"{totalMoneySpent} {currency}";

        }

        public static class CurrencyConvert
        {
            public static double GetConvertedCurrency(double value, string inputCurrency, string outputCurrency)
            {
                double result;
                switch (outputCurrency)
                {
                    case "EUR":
                        if (inputCurrency == "BGN")
                        {
                            result = BGN_To_EUR(value);
                        }
                        else
                        {
                            result = BGN_To_EUR(otherToBGN(inputCurrency, value));
                        }
                        break;
                    case "USD":
                        if (inputCurrency == "BGN")
                        {
                            result = BGN_To_USD(value);
                        }
                        else
                        {
                            result = BGN_To_USD(otherToBGN(inputCurrency, value));
                        }
                        break;
                    case "GBR":
                        if (inputCurrency == "BGN")
                        {
                            result = BGN_To_GBR(value);
                        }
                        else
                        {
                            result = BGN_To_GBR(otherToBGN(inputCurrency, value));
                        }
                        break;
                    case "CZK":
                        if (inputCurrency == "BGN")
                        {
                            result = BGN_To_CZK(value);
                        }
                        else
                        {
                            result = BGN_To_CZK(otherToBGN(inputCurrency, value));
                        }
                        break;
                    default:
                        result = otherToBGN(inputCurrency, value);
                        break;
                }

                return result;
            }

            private static double otherToBGN(string currency, double val)
            {
                switch (currency)
                {
                    case "EUR":
                        return EUR_To_BGN(val);
                    case "GBR":
                        return GPR_To_BGN(val);
                    case "CZK":
                        return CZK_To_BGN(val);
                    case "USD":
                        return USD_To_BGN(val);
                    default:
                        return 0;
                }
            }

            private static double BGN_To_EUR(double bgn)
            {
                return Math.Round(bgn * 0.511523319, 2);
            }

            private static double BGN_To_GBR(double bgn)
            {
                return Math.Round(bgn * 0.4563772, 2);
            }

            private static double BGN_To_USD(double bgn)
            {
                return Math.Round(bgn * 0.511523319, 2);
            }

            private static double BGN_To_CZK(double bgn)
            {
                return Math.Round(bgn * 0.63012, 2);
            }

            private static double EUR_To_BGN(double eur)
            {
                return Math.Round(eur * 1.95494509, 2);
            }

            private static double GPR_To_BGN(double eur)
            {
                return Math.Round(eur * 2.19116994, 2);
            }

            private static double CZK_To_BGN(double eur)
            {
                return Math.Round(eur * 0.0770646861, 1);
            }

            private static double USD_To_BGN(double eur)
            {
                return Math.Round(eur * 1.5869993, 1);
            }
        }

    }
}

