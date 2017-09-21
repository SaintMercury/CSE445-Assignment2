using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Assignment2
{
    class OrderBuf
    {
        private Semaphore _empty;
        private Semaphore _full;
        private List<string> _cells;

        public OrderBuf()
        {
            _empty = new Semaphore(3, 3);
            _full = new Semaphore(0, 3);
            _cells = new List<string>();
        }

        public void SetCell(string orderStr)
        {
            _empty.WaitOne(-1);
            lock (_cells)
            {
                _cells.Add(orderStr);
            }
            _full.Release();  // Signal to consumer that there's an order ready for consumption
        }

        public string GetCell(string receiverName)
        {
            string str = null;

            if (_full.WaitOne(1000)) // attempt to gain access for 1 sec for access, else give up and return null
            {
                lock (_cells)
                {
                    str = _cells.First(); // If the _full semaphore has granted access, 
                    _cells.RemoveAt(0); // there's at least one order in the buffer. Retrieve it

                    _empty.Release(); // Signal to producer that there's an empty cell ready to be set
                }
            }

            return str;
        }

        // Retiring this assuming any plant can retrieve an order
        private int FindByReceiverId(string receiverId)
        {
            List<Order> ordList = new List<Order>();

            foreach (string cell in _cells)
            {
                Order order = EncDec.DecodeOrder(cell);
                ordList.Add(order);
            }

            int orderIndex = ordList.FindIndex(order => order.ReceiverId == receiverId); // -1 if not found

            return orderIndex;
        }
    }
}
