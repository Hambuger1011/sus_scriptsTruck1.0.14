using Framework;

namespace GameLogic.Pool
{
	public interface IObjPoolCtrl
	{
		void Release(PooledClassObject obj);
	}
}
