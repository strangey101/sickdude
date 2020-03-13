using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dudes
{
    class Dude
    {
        public int X { get; set; }
        public int Y { get; set; }

        public int DestinationX { get; set; }
        public int DestinationY { get; set; }

        public int Health { get; set; }
        public double Age { get; set; }

        public void GoToDestination(double steps)
        {
            double xDistance = Math.Abs(DestinationX - X);

        }

        public bool IsAtDestination()
        {
            bool success = false;

            return success;
        }
    }
}
