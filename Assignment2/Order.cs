namespace Assignment2
{
    public class Order
    {
        public string SenderId { get; set; }
        public long CardNo { get; set; }
        public double Amount { get; set; }
        public double UnitPrice { get; set; }

        public Order(string senderId, long cardNo, double amt, double unitPrice)
        {
            SenderId = senderId;
            CardNo = cardNo;
            Amount = amt;
            UnitPrice = unitPrice;
        }
    }
}
