
namespace FuelDiary.Models
{
    public class Trip
    {
        public int Number { get; set; }
        public string Vehicle { get; set; }
        public int Distance { get; set; }
        public int Odometer { get; set; }
        public string OUnit { get; set; }
        public int UsedFuel { get; set; }
        public string FUnit { get; set; }
        public string FType { get; set; }
        public string Consumption { get; set; }
        public string MoneySpent { get; set; }
    }
}
