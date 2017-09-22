﻿using System;
using System.Threading;

namespace Assignment2
{
    public delegate void PriceCutEvent(float price); // emit the price and id of plant emitting event

    class Plant
    {
        public static event PriceCutEvent PriceCut; // Link event to delegate
        private static Semaphore ActiveCountSemaphore = new Semaphore(1, 1);
        private static int NUMBER_OF_ACTIVE_PLANTS = 0;
        private const int MAX_PRICECUTS = 3;
        private const int CARS_PER_TICK = 100;

        private float currentPrice;
        private float previousPrice;
        private int numberOfCars;
        private int priceCuts;
        public OrderBuf OrderBuffer { get; set; }
        public OrderBuf ConfirmationBuffer { get; set; }

        public Plant(OrderBuf orderBuffer, OrderBuf confirmationOrder)
        {
            // Make previous price so high that we immediately fire a promo event
            this.previousPrice = 0;
            this.currentPrice = 1000000;
            this.numberOfCars = 100;
            this.priceCuts = 0;
            this.OrderBuffer = orderBuffer;
            this.ConfirmationBuffer = confirmationOrder;
        }

        public void PlantFunc()
        {
            string plantName = Thread.CurrentThread.Name;

            Console.WriteLine("Plant {0} is starting up!", plantName);

            Plant.ActiveCountSemaphore.WaitOne(-1);
            Plant.NUMBER_OF_ACTIVE_PLANTS++;
            Plant.ActiveCountSemaphore.Release();

            while (this.priceCuts < Plant.MAX_PRICECUTS)
            {
                Thread.Sleep(Program.WAIT_TIME);

                this.produceCars(Plant.CARS_PER_TICK);

                getOrder(plantName);
                determinePrice();
                Console.WriteLine("Plant {0} current price: {1}", Thread.CurrentThread.Name, this.currentPrice);
            }

            Plant.ActiveCountSemaphore.WaitOne(-1);
            Plant.NUMBER_OF_ACTIVE_PLANTS--;
            Plant.ActiveCountSemaphore.Release();

            Console.WriteLine("Plant {0} is shutting down...", plantName);
        }

        public void produceCars(int numberOfCars)
        {
            this.numberOfCars += numberOfCars;
        }

        public float determinePrice()
        {
            int numberOfOrders = 1;
            float stockPrice = 250.0f;

            this.previousPrice = this.currentPrice;
            this.currentPrice = Pricing.CalculatePrice(numberOfOrders, this.numberOfCars, stockPrice);

            if (this.currentPrice < this.previousPrice)
            {
                var plantName = Thread.CurrentThread.Name;
                if (PriceCut != null)
                {
                    Console.WriteLine("\nPRICE CUT!!");
                    PriceCut(this.currentPrice);
                }

                ++priceCuts;
            }

            return this.currentPrice;
        }

        public void getOrder(string plantName)
        {
            string encOrder = OrderBuffer.GetFirstAvailableCell();
            if (encOrder != null)
            {
                Order order = EncDec.DecodeOrder(encOrder);
                processOrder(order);
            }
        }

        private void processOrder(Order order)
        {
            // Make a new thread, do the thing, dab, close thread or whatever

            const float SALES_TAX = 1.09f;
            float subTotal = order.Amount * order.UnitPrice;
            float total = subTotal * SALES_TAX;

            this.numberOfCars -= order.Amount;


            order.TimeFulfilled = DateTime.Now;
            order.ReceiverId = Thread.CurrentThread.Name;

            int index = Int32.Parse(order.SenderId);
            string encodedOrder = EncDec.EncodeOrder(order);
            ConfirmationBuffer.SetCellByIndex(encodedOrder, index);
        }

        public static int ActivePlantCount()
        {
            return Plant.NUMBER_OF_ACTIVE_PLANTS;
        }
    }
}
