using System.ComponentModel.DataAnnotations;

namespace Garage2.Models
{
    public enum Types
    {
        [Display(Name = "Car")]
        Car,

        [Display(Name = "Bus")]
        Bus,

        [Display(Name = "Boat")]
        Boat,

        [Display(Name = "Airplane")]
        Airplane,

        [Display(Name = "Motorcycle")]
        Motorcycle
    }
}
