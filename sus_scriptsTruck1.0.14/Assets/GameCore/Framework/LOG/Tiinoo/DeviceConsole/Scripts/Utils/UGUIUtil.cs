using Framework;

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Tiinoo.DeviceConsole
{
	public class UGUIUtil
	{
		public static void ScrollToTop(ScrollRect verticalScrollRect)
		{
			verticalScrollRect.verticalNormalizedPosition = 1f;
		}
		
		public static void ScrollToBottom(ScrollRect verticalScrollRect)
		{
			verticalScrollRect.verticalNormalizedPosition = 0f;
		}
		
		public static void ScrollToLeft(ScrollRect horizontalScrollRect)
		{
			horizontalScrollRect.horizontalNormalizedPosition = 0f;
		}
		
		public static void ScrollToRight(ScrollRect horizontalScrollRect)
		{
			horizontalScrollRect.horizontalNormalizedPosition = 1f;
		}
	}
}


