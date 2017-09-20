using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment2
{
    static class Pricing
    {
        private static Random rng = new Random();

        public static float CalculatePrice(int numberOfOrders, int numberOfCars, float stockPrice) // INCOMPLETE
        {
            // Use some dope ass statistical data to determine price
            var price = rng.Next(5, 20); // TODO: do actual price calculation
            return price; 
        }
    }
}
