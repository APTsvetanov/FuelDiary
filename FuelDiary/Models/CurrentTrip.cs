
namespace FuelDiary.Models
{
    public class CurrentTrip
    {
        public string Vehicle { get; set; }
        public int Odometer { get; set; }
        public string OUnit { get; set; }
        public int Fuel { get; set; }
        public string FUnit { get; set; }
        public string FType { get; set; }
        public double FPrice { get; set; }
        public string Currency { get; set; }
    }
}
