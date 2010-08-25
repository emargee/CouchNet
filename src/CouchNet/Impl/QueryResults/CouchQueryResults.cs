using System.Collections;
using System.Collections.Generic;

namespace CouchNet.Impl.QueryResults
{
    public class CouchQueryResults<T> : ICouchQueryResults<T>
    {
        private readonly IList<T> _innerList = new List<T>();

        #region Implementation of IEnumerable

        public IEnumerator<T> GetEnumerator()
        {
            return _innerList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_innerList).GetEnumerator();
        }

        #endregion

        #region Implementation of ICollection<T>

        public void Add(T item)
        {
            _innerList.Add(item);
        }

        public void Clear()
        {
            _innerList.Clear();
        }

        public bool Contains(T item)
        {
            return _innerList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _innerList.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return _innerList.Remove(item);
        }

        public int Count
        {
            get { return _innerList.Count; }
        }

        public bool IsReadOnly
        {
            get { return _innerList.IsReadOnly; }
        }

        #endregion

        #region Implementation of IList<T>

        public int IndexOf(T item)
        {
            return _innerList.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _innerList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _innerList.RemoveAt(index);
        }

        public T this[int index]
        {
            get { return _innerList[index]; }
            set { _innerList[index] = value; }
        }

        #endregion

        #region Implementation of ICouchQueryResults<T>

        public int TotalRows { get; set; }

        public int Offset { get; set; }

        public bool HasResults
        {
            get { return _innerList.Count > 0; }
        }

        public bool IsOk
        {
            get
            {
                if(Response != null)
                {
                    return Response.IsOk;
                }

                return false;
            }
        }

        public ICouchServerResponse Response { get; set; }

        #endregion

        public CouchQueryResults()
        {
            TotalRows = 0;
            Offset = 0;
        }
    }
}