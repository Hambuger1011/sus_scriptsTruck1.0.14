using Framework;

namespace GameLogic.Pool
{
	public class ClassObjPool<T> : ClassObjPoolBase where T : PooledClassObject, new()
	{
		private static ClassObjPool<T> instance;

		public static uint NewSeq()
		{
			if (instance == null)
			{
				instance = new ClassObjPool<T>();
			}
			instance.reqSeq += 1u;
			return instance.reqSeq;
		}

		public static T Get()
		{
			if (instance == null)
			{
				instance = new ClassObjPool<T>();
			}
			if (instance.pool.Count > 0)
			{
				T val = (T)instance.pool[instance.pool.Count - 1];
				instance.pool.RemoveAt(instance.pool.Count - 1);
				instance.reqSeq += 1u;
				val.usingSeq = instance.reqSeq;
				val.holder = instance;
				val.OnUse();
				return val;
			}
			T val2 = new T();
			instance.reqSeq += 1u;
			val2.usingSeq = instance.reqSeq;
			val2.holder = instance;
			val2.OnUse();
			return val2;
		}

		public override void Release(PooledClassObject obj)
		{
			T item = obj as T;
			obj.usingSeq = 0u;
			obj.holder = null;
			base.pool.Add(item);
		}
	}
}
