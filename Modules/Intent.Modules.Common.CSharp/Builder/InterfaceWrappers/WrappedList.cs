using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.CSharp.Builder.InterfaceWrappers
{
    internal class WrappedList<TConcrete, TInterface>(IList<TConcrete> backingList) : IList<TInterface>
        where TConcrete : TInterface
    {
        public IEnumerator<TInterface> GetEnumerator() => backingList.Cast<TInterface>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(TInterface item) => backingList.Add((TConcrete)item);

        public void Clear() => backingList.Clear();

        public bool Contains(TInterface item) => backingList.Contains((TConcrete)item);

        public void CopyTo(TInterface[] array, int arrayIndex) => Array.Copy(backingList.ToArray(), 0, array, arrayIndex, backingList.Count);

        public bool Remove(TInterface item) => backingList.Remove((TConcrete)item);

        public int Count => backingList.Count;

        public bool IsReadOnly => backingList.IsReadOnly;

        public int IndexOf(TInterface item) => backingList.IndexOf((TConcrete)item);

        public void Insert(int index, TInterface item) => backingList.Insert(index, (TConcrete)item);

        public void RemoveAt(int index) => backingList.RemoveAt(index);

        public TInterface this[int index]
        {
            get => backingList[index];
            set => backingList[index] = (TConcrete)value;
        }
    }
}
