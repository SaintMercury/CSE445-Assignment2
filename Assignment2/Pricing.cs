using System;
using System.Security.Cryptography;

namespace Assignment2
{
    static class Pricing
    {
        private static Random rng = new Random();

        public static float CalculatePrice(float numberOfOrders, float numberOfCars, float stockPrice)
        {
            float SOME_CONSTANT = 1000.0f;
            float price = numberOfOrders / numberOfCars * stockPrice * SOME_CONSTANT;
            return price;
        }
    }
}
