namespace Assignment2
{
    class OrderThreadArg
    {
        public float Price { get; set; }
        public string ThreadId { get; set; }
        public float PreviousPrice { get; set; }

        public OrderThreadArg(float price, string threadId, float prevPrice)
        {
            PreviousPrice = prevPrice;
            Price = price;
            ThreadId = threadId;
        }
    }
}