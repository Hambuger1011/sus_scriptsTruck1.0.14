using Framework;

using System;

namespace GameLogic.Pool
{
	public struct PoolObjHandle<T> : IEquatable<PoolObjHandle<T>> where T : PooledClassObject
	{
		public uint _handleSeq;

		public T _handleObj;

		public T handle
		{
			get
			{
				return _handleObj;
			}
		}

        public override string ToString()
        {
            if(this)
            {
                return this.handle.ToString();
            }
            return "null";
        }

        public PoolObjHandle(T obj)
		{
			if (obj != null && obj.usingSeq != 0)
			{
				_handleSeq = obj.usingSeq;
				_handleObj = obj;
			}
			else
			{
				_handleSeq = 0u;
				_handleObj = null;
			}
		}

		public void Validate()
		{
			_handleSeq = ((_handleObj != null) ? _handleObj.usingSeq : 0);
		}

		public void Release()
		{
			_handleObj = null;
			_handleSeq = 0u;
		}

		public bool Equals(PoolObjHandle<T> other)
		{
			return this == other;
		}

		public override bool Equals(object obj)
		{
			return obj != null && this.GetType() == obj.GetType() && this == (PoolObjHandle<T>)obj;
		}

		public override int GetHashCode()
		{
			return (this).GetHashCode();
		}

		public static implicit operator bool(PoolObjHandle<T> ptr)
		{
			return ptr._handleObj != null && ptr._handleObj.usingSeq == ptr._handleSeq;
		}

		public static bool operator ==(PoolObjHandle<T> lhs, PoolObjHandle<T> rhs)
		{
			return lhs._handleObj == rhs._handleObj && lhs._handleSeq == rhs._handleSeq;
		}

		public static bool operator !=(PoolObjHandle<T> lhs, PoolObjHandle<T> rhs)
		{
			return lhs._handleObj != rhs._handleObj || lhs._handleSeq != rhs._handleSeq;
		}

		public static implicit operator T(PoolObjHandle<T> ptr)
		{
			return ptr.handle;
		}
	}
}
