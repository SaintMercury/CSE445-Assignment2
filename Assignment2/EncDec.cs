using Newtonsoft.Json;

// Should just work, but if it doesn't
// manage nuget packages -> search( json.NET )
namespace Assignment2
{
    public static class EncDec
    {
        public static string EncodeOrder(Order order)
        {
            var jsonStr = JsonConvert.SerializeObject(order);
            return jsonStr;
        }

        public static Order DecodeOrder(string jsonOrder)
        {
            var order = JsonConvert.DeserializeObject<Order>(jsonOrder);
            return order;
        }
    }
}
