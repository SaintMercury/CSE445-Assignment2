using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
            _full.Release();
        }

        public string GetCell(string receiverName)
        {
            string str = null;

            if (_full.WaitOne(1000)) // wait 1 sec for access, else give up and return null
            {
                lock (_cells)
                {
                    var ix = FindByReceiverId(receiverName); // look for orders for this plant
                    if (ix > -1)
                    {
                        str = _cells.ElementAt(ix);
                        _cells.RemoveAt(ix);
                        _empty.Release(); // One cell is now empty so increment amount of empty cells
                    }
                    else
                    {
                        _full.Release(); // weren't able to retrieve an order so increment back to original
                    }
                }
            }

            return str;
        }

        private int FindByReceiverId(string receiverId)
        {
            var ordList = new List<Order>();

            foreach (var cell in _cells)
            {
                var order = EncDec.DecodeOrder(cell);
                ordList.Add(order);
            }

            var ix = ordList.FindIndex(x => x.ReceiverId == receiverId); // -1 if not found

            return ix;
        }
    }
}
