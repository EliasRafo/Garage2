using System.ComponentModel.DataAnnotations;

namespace Garage2.Models.ViewModels
{
    public class ReceiptViewModel
    {
        public Vehicle Vehicle { get; set; }

        [Display(Name = "Checked out")]
        public DateTime CheckOutTime { get; set; }

        [Display(Name = "Duration")]
        public string ParkingPeriod { get; set; }
        [Display(Name = "Price")]
        public string Price { get; set; }
    }
}
