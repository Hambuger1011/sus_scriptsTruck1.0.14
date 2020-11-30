using Framework;

/*
 * 可回收对象基类
 */
namespace GameLogic.Pool
{
	public class PooledClassObject
	{
		public uint usingSeq;

		public IObjPoolCtrl holder;

		public bool bChkReset = true;

		public virtual void OnUse()
		{
            Reset();
        }

		public virtual void OnRelease()
		{
            Reset();
        }

        public virtual void Reset()
        {

        }

		public void Release()
		{
			if (holder != null)
			{
				OnRelease();
				holder.Release(this);
			}
		}
	}
}
