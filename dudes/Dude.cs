using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dudes
{
    class Dude : GameObject
    {
        public enum action { GoingToTown, GoingToShop, StayingAtHome};
        public enum gender { Male, Female};

        public string firstName { get; set; }
        public string surname { get; set; }
        public gender Gender { get; set; }

        public action Action;
        public double DestinationX { get; set; }
        public double DestinationY { get; set; }
        public string DestinationName { get; set; }

        public int Health { get; set; }
        public double Age { get; set; }

        public double timeSinceLastChild { get; set; }

        private int turnsToStayAtHome = 10;
        public int stayAtHomeDaysLeft;

        public void GoToDestination(double steps)
        {
            
            double xToYRatio = (DestinationX - X) / (DestinationY - Y);

            if(DestinationY - Y == 0)
            {
                xToYRatio = 1;
            }

            double xDifference = DestinationX - X;
            double yDifference = DestinationY - Y;

            if(Math.Abs(xDifference) < steps)
            {
                X = DestinationX;
            }
            else
            {
                if (xDifference > 0)
                    X += steps;
                else
                    X -= steps;
            }

            if (Math.Abs(yDifference) < steps)
            {
                Y = DestinationY;
            }
            else
            {
                if (yDifference > 0)
                    Y += steps;
                else
                    Y -= steps;
            }
        }

        public bool IsAtDestination()
        {
            bool success = false;

            if((int)X == (int)DestinationX && (int)Y == (int)DestinationY)
            {
                success = true;
            }

            return success;
        }

        internal void PickNewDestination(List<Shop> shops, List<Town> towns, double desireToGoToShop, double desireToGoToTown, double desireToStayAtHome, double desireToGoFar)
        {
            if (Age < 10)
                return;

            if (Action == action.StayingAtHome && stayAtHomeDaysLeft > 0)
            {
                stayAtHomeDaysLeft--;
                return;
            }

            Random random = new Random();
            double allPossibilities = desireToGoToShop + desireToGoToTown + desireToStayAtHome;

            double choice = random.NextDouble() * allPossibilities;

            if(choice < desireToGoToShop)
            {
                Action = action.GoingToShop;
                // pick a shop
                Shop shopToGoTo = shops[random.Next(shops.Count)];
                DestinationName = shopToGoTo.Name;
                DestinationX = shopToGoTo.X;
                DestinationY = shopToGoTo.Y;
            }
            if(choice >= desireToGoToShop && choice < desireToGoToShop + desireToGoToTown)
            {
                Action = action.GoingToTown;
                // pick a town
                Town townToGoTo = towns[random.Next(towns.Count)];
                DestinationName = townToGoTo.Name;
                DestinationX = townToGoTo.X;
                DestinationY = townToGoTo.Y;
            }
            if(choice >= desireToGoToShop + desireToGoToTown)
            {
                Action = action.StayingAtHome;
                // stay at home for a number of turns
                stayAtHomeDaysLeft = turnsToStayAtHome;
            }
        }
    }
}
