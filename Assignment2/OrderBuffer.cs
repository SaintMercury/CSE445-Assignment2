using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace Assignment2
{
    // The reader/writer problem typically supports multiple readers
    // in this case, we're reading orders, and don't want to read the same order twice
    // That means that whenever an order is read, we want to remove it from the buffer
    // Since reading an order will actually involve removing it (setting its corresponding cell to null)
    // we only ever aquire the writer lock; e.g. A cell is only ever accessed by 1 thread at a time

    class OrderBuffer
    {
        private List<BufferObject> _cells;
        private Semaphore _semaphore;

        public OrderBuffer()
        {
            _cells = new List<BufferObject>(3);
            // Instantiate our buffer with 3 empty buffer objects
            for (var i = 0; i < 3; ++i)
            {
                _cells.Add(new BufferObject());
            }

            // We have 3 empty cells and a max of 3 cells
            _semaphore = new Semaphore(3, 3);
        }

        public string GetAvailableCell()
        {
            string str = null;
            bool aquired = false;

            // wait for as long as necessary for access, and then aquire a spot
            while (!aquired)
            {
                _semaphore.WaitOne(-1);

                // find an available cell
                var cell = _cells.Find(x => x.IsCellReadyToBeAquired);
                if (cell != null) // found a cell ready to be read
                {
                    str = cell.GetCell();
                    aquired = true;
                }
               
                // got what we need, release space in semaphore
                // break from loop only if we've aquired a cell
                _semaphore.Release();
            }

            return str;
        }

        public void SetAvailablCell(string orderStr)
        {
            var set = false;
            while (!set)
            {
                // wait for as long as necessary for access, and then aquire a spot
                _semaphore.WaitOne(-1);

                //find an available cell
                var cell = _cells.Find(x => x.IsCellReadyToBeSet == true);
                if (cell != null) // found an empty cell => set it
                {
                    cell.SetCell(orderStr);
                    set = true;
                }

                // release sem to another thread
                // if successfully set, break from loop
                _semaphore.Release();
            }
        }
    }

    public class BufferObject
    {
        private string _bufferCell;
        private ReaderWriterLock _rwLock;

        // default constructor for empty object
        public BufferObject()
        {
            _rwLock = new ReaderWriterLock();
            _bufferCell = null;
        }
        
        // If lock is open and its cell has been set, it's ready to be retrieved
        public bool IsCellReadyToBeAquired => !_rwLock.IsWriterLockHeld && _bufferCell != null;

        // If lock is open and cell is null, it's free to write too
        public bool IsCellReadyToBeSet => !_rwLock.IsWriterLockHeld && _bufferCell == null;

        public string GetCell()
        {
            // -1 means we wait for as long as it's necessary
            _rwLock.AcquireWriterLock(-1);
            var retStr = _bufferCell;

            // we've gotten the contents, now set cell to null to indicate it's empty
            _bufferCell = null;
            _rwLock.ReleaseWriterLock();

            return retStr;
        }

        public void SetCell(string orderStr)
        {
            // -1 means we wait for as long as it's necessary
            _rwLock.AcquireWriterLock(-1);
            _bufferCell = orderStr;
            _rwLock.ReleaseWriterLock();
        }

    }
}
