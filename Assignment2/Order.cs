using System;

namespace Assignment2
{
    public class Order
    {
        public string SenderId { get; set; }        
        public long CardNo { get; set; }
        public int Amount { get; set; }
        public float UnitPrice { get; set; }
        public DateTime TimeCreated { get; set; }
        public DateTime TimeFulfilled { get; set; }
        public string ReceiverId { get; set; }

        public Order(string senderId, long cardNo, int amt, float unitPrice)
        {
            SenderId = senderId;
            CardNo = cardNo;
            Amount = amt;
            UnitPrice = unitPrice;
            TimeCreated = DateTime.Now;
        }
    }
}
