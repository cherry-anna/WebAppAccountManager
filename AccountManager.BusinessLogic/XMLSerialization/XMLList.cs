using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AccountManager.BusinessLogic.XMLSerialization
{
    public class XMLList<T> : IList<T>, IXmlSerializable
    {
        private readonly IList<T> src;
        public XMLList(IList<T> wrapped)
        {
            if (wrapped == null) throw new ArgumentNullException("wrapped");
            src = wrapped;
        }
        public XMLList() : this(new List<T>()) { }
        public void Update(IList<T> target)
        {
            if (target == null) throw new ArgumentNullException("target");
            bool changed = target.Count != src.Count;
            if (!changed)
            {
                for (int i = src.Count - 1; i >= 0; i--)
                {
                    T s = src[i];
                    T t = target[i];
                    if (s == null && t == null)
                    {
                    }
                    else if (s == null || t == null)
                    {
                        changed = true; break;
                    }
                    else if (s.Equals(t))
                    {
                        changed = true; break;
                    }
                }
            }
            if (changed)
            {
                target.Clear();
                for (int i = 0; i < src.Count; i++)
                    target.Add(src[i]);
            }
        }
        #region IXmlSerializable Members  
        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }
        void IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
        {
            if (reader.IsEmptyElement || reader.Read() == false)
                return;
            XmlSerializer inner = new XmlSerializer(typeof(T));
            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                T e = (T)inner.Deserialize(reader);
                src.Add(e);
            }
            reader.ReadEndElement();
        }
        void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
        {
            if (src.Count == 0)
                return;
            XmlSerializer inner = new XmlSerializer(typeof(T));
            for (int i = 0; i < src.Count; i++)
            {
                inner.Serialize(writer, src[i]);
            }
        }
        #endregion

        #region IList<T> Members  
        int IList<T>.IndexOf(T item)
        {
            return src.IndexOf(item);
        }
        void IList<T>.Insert(int index, T item)
        {
            src.Insert(index, item);
        }
        void IList<T>.RemoveAt(int index)
        {
            src.RemoveAt(index);
        }
        T IList<T>.this[int index]
        {
            get { return src[index]; }
            set { src[index] = value; }
        }
        #endregion

        #region ICollection<T> Members  
        void ICollection<T>.Add(T item)
        {
            src.Add(item);
        }
        void ICollection<T>.Clear()
        {
            src.Clear();
        }
        bool ICollection<T>.Contains(T item)
        {
            return src.Contains(item);
        }
        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            src.CopyTo(array, arrayIndex);
        }
        int ICollection<T>.Count
        {
            get { return src.Count; }
        }
        bool ICollection<T>.IsReadOnly
        {
            get { return src.IsReadOnly; }
        }
        bool ICollection<T>.Remove(T item)
        {
            return src.Remove(item);
        }
        #endregion

        #region IEnumerable<T> Members  
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return src.GetEnumerator();
        }
        #endregion

        #region IEnumerable Members  
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)src).GetEnumerator();
        }
        #endregion
    }
}
