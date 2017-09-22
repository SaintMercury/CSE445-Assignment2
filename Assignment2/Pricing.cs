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
            float SOME_CONSTANT = 1.0f;
            var price = numberOfOrders / numberOfCars * stockPrice * SOME_CONSTANT * (rng.NextDouble() + 0.5);
            return (float)price; 
        }
    }
}
