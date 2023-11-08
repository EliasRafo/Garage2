using System.ComponentModel.DataAnnotations;

namespace Garage2.Models.ViewModels
{
    public class OverviewViewModel
    {
        public int Capacity { get; set; }

        public List<VehicleTypesDto> VehiclesStatistic { get; set; }

        public int WheelsNumber { get; set; }
        public double Revenues { get; set; }

        public int FreeSpaces { get; set; }

        public List<ParkingSpace> ParkingSpaces { get; set; }

    }
}
