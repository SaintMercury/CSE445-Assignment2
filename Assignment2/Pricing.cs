using System;
using System.Security.Cryptography;

namespace Assignment2
{
    static class Pricing
    {
        private static Random rng = new Random();
        public static float CalculatePrice(int numberOfOrders, int numberOfCars, float stockPrice) // INCOMPLETE
        {
            float SOME_CONSTANT = 1.0f;
            var price = numberOfOrders / numberOfCars * stockPrice * SOME_CONSTANT * (rng.Next(1, 3) * 0.5);
            return (float)price;
        }
    }
}
