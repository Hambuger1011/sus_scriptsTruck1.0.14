using Framework;

using System;
using System.Collections;
using System.Collections.Generic;

public class DictionaryView<TKey, TValue> : IEnumerable, IEnumerable<KeyValuePair<TKey, TValue>>
{
	public struct Enumerator : IDisposable, IEnumerator, IEnumerator<KeyValuePair<TKey, TValue>>
	{
		private Dictionary<TKey, object> Reference;

		private Dictionary<TKey, object>.Enumerator Iter;

		object IEnumerator.Current
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public KeyValuePair<TKey, TValue> Current
		{
			get
			{
				return new KeyValuePair<TKey, TValue>(Iter.Current.Key, (Iter.Current.Value == null) ? default(TValue) : ((TValue)Iter.Current.Value));
			}
		}

		public Enumerator(Dictionary<TKey, object> InReference)
		{
			Reference = InReference;
			Iter = Reference.GetEnumerator();
		}

		public void Reset()
		{
			Iter = Reference.GetEnumerator();
		}

		public void Dispose()
		{
			Iter.Dispose();
			Reference = null;
		}

		public bool MoveNext()
		{
			return Iter.MoveNext();
		}
	}

	protected Dictionary<TKey, object> Context;

	public int Count
	{
		get
		{
			return Context.Count;
		}
	}

	public TValue this[TKey key]
	{
		get
		{
			object obj = Context[key];
			return (obj == null) ? default(TValue) : ((TValue)obj);
		}
		set
		{
			Context[key] = value;
		}
	}

	public Dictionary<TKey, object>.KeyCollection Keys
	{
		get
		{
			return Context.Keys;
		}
	}

	public DictionaryView()
	{
		Context = new Dictionary<TKey, object>();
	}

	public DictionaryView(int capacity)
	{
		Context = new Dictionary<TKey, object>(capacity);
	}

	IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
	{
		return (IEnumerator<KeyValuePair<TKey, TValue>>)GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		throw new NotImplementedException();
	}

	public void Add(TKey key, TValue value)
	{
		Context.Add(key, value);
	}

	public void Clear()
	{
		Context.Clear();
	}

	public bool ContainsKey(TKey key)
	{
		return Context.ContainsKey(key);
	}

	public bool Remove(TKey key)
	{
		return Context.Remove(key);
	}

	public bool TryGetValue(TKey key, out TValue value)
	{
		object obj = null;
		bool result = Context.TryGetValue(key, out obj);
		value = ((obj == null) ? default(TValue) : ((TValue)obj));
		return result;
	}

	public Enumerator GetEnumerator()
	{
		return new Enumerator(Context);
	}
}
