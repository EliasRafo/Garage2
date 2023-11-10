using System.ComponentModel.DataAnnotations;

namespace Garage2.Models
{
    public class SizeData
    {
        public const int Full = 3;
        public const int Empty = 0;
        public const int OneThird = Motorcycle;
        public const int TwoThird = Motorcycle*2;

        public const int Car = Full;
        public const int Motorcycle = Full/3;
        public const int Boat = Full * 3;
        public const int Bus = Full * 3;
        public const int Airplane = Full * 3;
   


        public static int AssignSize(Types vehicleType)
        {
            int size = 0;
            switch (vehicleType)
            {
                case Types.Motorcycle:
                    size = Motorcycle;
                    break;
                case Types.Airplane:
                    size = Airplane;
                    break;

                case Types.Boat:
                    size = Boat;
                    break;

                case Types.Bus:
                    size = Bus;
                    break;

                case Types.Car:
                    size = Car;
                    break;

                default:
                    throw new ArgumentException($"Vehicletyp {vehicleType} is not recognized.");
            }
            return size;

        }
    }

}
