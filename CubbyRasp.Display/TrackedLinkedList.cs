using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubbyRasp.Display
{
    public class TrackedLinkedList<T>
    {
        private LinkedList<T> _linkedList;
        private LinkedListNode<T> _current;

        private readonly object _lockObject = new object();

        public TrackedLinkedList(IList<T> list)
        {
            _linkedList = new LinkedList<T>(list);
            if (list.Any())
            {
                _current = _linkedList.First;
            }
        }

        public void Add(T item)
        {
            lock (_lockObject)
            {
                _linkedList.AddLast(item);
            }
        }

        public void Clear()
        {
            this.Replace(new List<T>());
        }

        public bool Any()
        {
            lock (_lockObject)
            {
                return this._linkedList.Any();
            }
        }

        public void Replace(IList<T> list)
        {
            lock (_lockObject)
            {
                this._linkedList = new LinkedList<T>(list);

                if (list.Any())
                {
                    _current = this._linkedList.First;
                }
                else
                {
                    _current = null;
                }
            }
        }

        public T Next
        {
            get
            {
                lock (_lockObject)
                {
                    if (!_linkedList.Any())
                    {
                        return default(T);
                    }
                    else
                    {
                        _current = _current.NextOrFirst();
                        return _current.Value;
                    }
                }
            }
        }

        public T Previous
        {
            get
            {
                lock (_lockObject)
                {
                    if (!_linkedList.Any())
                    {
                        return default(T);
                    }
                    else
                    {
                        _current = _current.PreviousOrLast();
                        return _current.Value;
                    }
                }
            }
        }
    }
}
