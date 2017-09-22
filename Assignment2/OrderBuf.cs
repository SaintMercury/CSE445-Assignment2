using System.Threading;

namespace Assignment2
{
    class OrderBuf
    {
        private Semaphore _Empty;
        private Semaphore _Full;
        private string[] _Cells;

        public OrderBuf(int size)
        {
            _Empty = new Semaphore(size, size);
            _Full = new Semaphore(0, size);
            
            _Cells = new string[size];
            for (int i = 0; i < _Cells.Length; i++)
            {
                _Cells[i] = null;
            }
        }

        public void SetFirstAvailableCell(string orderStr)
        {
            string tempStr;
            if (_Empty.WaitOne(5000))
            {
                lock (_Cells)
                {
                    for (int i = 0; i < _Cells.Length; i++)
                    {
                        tempStr = _Cells[i];
                        if (string.IsNullOrEmpty(tempStr))
                        {
                            _Cells[i] = orderStr;
                            break;
                        }
                    }
                }

                _Full.Release();
            }
        }

        public void SetCellByIndex(string orderStr, int index)
        {
            lock (_Cells)
            {
                _Cells[index] = orderStr;
            }
        }

        public string GetFirstAvailableCell()
        {
            string encodedStr = null;
            if (_Full.WaitOne(1000))
            {
                lock (_Cells)
                {
                    for (int i = 0; i < _Cells.Length; i++)
                    {
                        encodedStr = _Cells[i];
                        if (!string.IsNullOrEmpty(encodedStr))  // Found an occupied cell: null it out -> break -> return
                        {
                            _Cells[i] = null;
                            break;
                        }
                    }
                }
                _Empty.Release();
            }

            return encodedStr;
        }

        public string GetCellByIndex(int index)
        {
            string encodedStr = null;
           
            lock (_Cells)
            {
                encodedStr = _Cells[index];
            }
                
            return encodedStr;
        }
    }
}
