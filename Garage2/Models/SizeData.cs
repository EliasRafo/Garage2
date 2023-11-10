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
        public const int Ariplane = Full * 3;
    }
}
