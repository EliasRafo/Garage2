using System.ComponentModel.DataAnnotations;

namespace Garage2.Models
{
    public class ParkingSpace
    {
        public int Id { get; set; }
        public bool Reserved { get; set; }

        public Vehicle? Vehicle { get; set; }

        public int[]? spaceOccupied { get; set; }

    }
}
