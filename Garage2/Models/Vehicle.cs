using System.ComponentModel.DataAnnotations;

namespace Garage2.Models
{
    public class Vehicle
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Vehicle type")]
        [Required(ErrorMessage = "Vehicle type is required")]
        public Types Type { get; set; }

        [Display(Name = "Registration number")]
        [RegularExpression(@"^[a-zA-Z]{3}[0-9]{3}$", ErrorMessage = "The registration number should be in this form ABC123.")]
        [Required(ErrorMessage = "Registration number is required")]
        [StringLength(6)]
        public string RegNum { get; set; }

        [Display(Name = "Color")]
        [Required(ErrorMessage = "Color is required")]
        [StringLength(30)]
        public string Color { get; set; }

        [Display(Name = "Brand")]
        [Required(ErrorMessage = "Brand is required")]
        [StringLength(30)]
        public string Brand { get; set; }

        [Display(Name = "Model")]
        [Required(ErrorMessage = "Model is required")]
        [StringLength(30)]
        public string Model { get; set; }

        [Display(Name = "Wheels Number")]
        [Required(ErrorMessage = "Wheels Number is required")]
        [Range(0, 12)]
        public int WheelsNumber { get; set; }

        [Display(Name = "ParkingTime")]
        public DateTime ParkingTime { get; set; }
    }
}
