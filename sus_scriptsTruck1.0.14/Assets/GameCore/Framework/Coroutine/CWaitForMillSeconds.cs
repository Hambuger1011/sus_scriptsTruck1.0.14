using Framework;

using System;

public class CWaitForMillSeconds : CCoroutineYieldBase
{
	public int m_interval = 0;

	public CWaitForMillSeconds(int interval = 0)
	{
		this.m_interval = interval;
	}
}
