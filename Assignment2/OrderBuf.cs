using System;
using System.Threading;

namespace Assignment2
{
    class OrderBuf
    {
        private Semaphore _empty;
        private Semaphore _full;
        private string[] _cells;
        private AutoResetEvent _e1;
        private AutoResetEvent _e2;

        public OrderBuf(int size)
        {
            _empty = new Semaphore(size, size);
            _full = new Semaphore(0, size);
            _e1 = new AutoResetEvent(false);
            _e2 = new AutoResetEvent(true);
            
            _cells = new string[size];
        }


        public void SetFirstAvailableCell(string orderStr)
        {
            if (_empty.WaitOne(5000))
            {
                lock (_cells)
                {
                    for (int i = 0; i < _cells.Length; i++)
                    {
                        var tempStr = _cells[i];
                        if (string.IsNullOrEmpty(tempStr))
                        {
                            _cells[i] = orderStr;
                            break;
                        }
                    }
                }

                _full.Release();
            }
        }

        public void SetCellByIndex(string orderStr, int index)
        {
            // Entry
            _e1.Reset();

            // CS
            _cells[index] = orderStr;

            // Exit
            _e2.Set();
        }

        public string GetFirstAvailableCell()
        {
            string encodedStr = null;
            if (_full.WaitOne(1000))
            {
                lock (_cells)
                {
                    for (int i = 0; i < _cells.Length; i++)
                    {
                        encodedStr = _cells[i];
                        if (!string.IsNullOrEmpty(encodedStr))  // Found an occupied cell: null it out -> break -> return
                        {
                            _cells[i] = null;
                            break;
                        }
                    }
                }
                _empty.Release();
            }

            return encodedStr;
        }

        public string GetCellByIndex(int index)
        {
            // Entry
            _e2.Reset();

            // CS
            string encOrder = _cells[index];
            _cells[index] = null;

            // Exit
            _e1.Set();

            return encOrder;
        }
    }
}
