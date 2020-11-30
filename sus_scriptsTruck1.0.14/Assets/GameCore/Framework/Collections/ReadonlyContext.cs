using Framework;

using System;
using System.Collections;
using System.Collections.Generic;

public struct ReadonlyContext<T>
{
	public struct Enumerator : IDisposable, IEnumerator, IEnumerator<T>
	{
		private List<T> Reference;

		private List<T>.Enumerator IterReference;

		object IEnumerator.Current
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public T Current
		{
			get
			{
				return IterReference.Current;
			}
		}

		public Enumerator(List<T> InRefernce)
		{
			Reference = InRefernce;
			IterReference = InRefernce.GetEnumerator();
		}

		public void Reset()
		{
			IterReference = Reference.GetEnumerator();
		}

		public void Dispose()
		{
			IterReference.Dispose();
			Reference = null;
		}

		public bool MoveNext()
		{
			return IterReference.MoveNext();
		}
	}

	private List<T> Reference;

	public bool isValidReference
	{
		get
		{
			return Reference != null;
		}
	}

	public T this[int index]
	{
		get
		{
			return Reference[index];
		}
	}

	public int Count
	{
		get
		{
			return Reference.Count;
		}
	}

	public ReadonlyContext(List<T> InReference)
	{
		Reference = InReference;
	}

	public Enumerator GetEnumerator()
	{
		return new Enumerator(Reference);
	}
}
