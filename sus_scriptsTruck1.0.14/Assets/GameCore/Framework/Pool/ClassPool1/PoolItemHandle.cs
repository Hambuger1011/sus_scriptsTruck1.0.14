using Framework;

/*
可回收实例handle
*/
using System;

namespace Common
{
	public struct PoolItemHandle<T> : IEquatable<PoolItemHandle<T>> where T : class,IPoolItem
	{
		public uint _handleSeq;

		public T _handleObj;

		public T handle
		{
			get
			{
                if(Validate())
                {
                    return this._handleObj;
                }
                return null;
			}
		}

		public PoolItemHandle(T obj)
		{
			if (obj != null && obj.GetUsingSeq() > 0u)
			{
				this._handleSeq = obj.GetUsingSeq();
				this._handleObj = obj;
			}
			else
			{
				this._handleSeq = 0u;
				this._handleObj = null;
			}
		}

		public bool Validate()
		{
            try
            {
                if (_handleObj == null)
                {
                    return false;
                }
                return (this._handleSeq == this._handleObj.GetUsingSeq());
            }
            catch(Exception ex)
            {
                LOG.Error(string.Concat(typeof(T),"\n",ex));
                //throw ex;
            }
            return false;
		}

		public void Release()
		{
			this._handleObj = default(T);
			this._handleSeq = 0u;
		}

		public bool Equals(PoolItemHandle<T> other)
		{
			return this == other;
		}

		public override bool Equals(object obj)
		{
			return obj != null && base.GetType() == obj.GetType() && this == (PoolItemHandle<T>)obj;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static implicit operator bool(PoolItemHandle<T> ptr)
		{
			return ptr._handleObj != null && ptr._handleObj.GetUsingSeq() == ptr._handleSeq;
		}

		public static bool operator ==(PoolItemHandle<T> lhs, PoolItemHandle<T> rhs)
		{
			return lhs._handleObj == rhs._handleObj && lhs._handleSeq == rhs._handleSeq;
		}

		public static bool operator !=(PoolItemHandle<T> lhs, PoolItemHandle<T> rhs)
		{
			return lhs._handleObj != rhs._handleObj || lhs._handleSeq != rhs._handleSeq;
		}

		//public static implicit operator T(PoolItemHandle<T> ptr)
		//{
		//	return ptr.handle;
		//}
	}
}
