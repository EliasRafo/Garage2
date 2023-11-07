using System.ComponentModel.DataAnnotations;

namespace Garage2.Models
{
    public enum Types
    {
        [Display(Name = "Car")]
        Car = 4,

        [Display(Name = "Bus")]
        Bus = 3,

        [Display(Name = "Boat")]
        Boat = 2,

        [Display(Name = "Airplane")]
        Airplane = 1,

        [Display(Name = "Motorcycle")]
        Motorcycle = 5
    }
}
