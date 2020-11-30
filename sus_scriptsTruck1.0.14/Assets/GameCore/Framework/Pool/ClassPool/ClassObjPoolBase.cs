using Framework;

using System.Collections.Generic;

namespace GameLogic.Pool
{
	public abstract class ClassObjPoolBase : IObjPoolCtrl
	{
		protected List<object> pool = new List<object>(128);

		protected uint reqSeq;

		public int capacity
		{
			get
			{
				return pool.Capacity;
			}
			set
			{
				pool.Capacity = value;
			}
		}

		public abstract void Release(PooledClassObject obj);
	}
}
