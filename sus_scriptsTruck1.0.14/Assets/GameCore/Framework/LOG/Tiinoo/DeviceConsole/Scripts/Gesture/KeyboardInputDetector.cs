using Framework;

using UnityEngine;
using System.Collections;

namespace Tiinoo.DeviceConsole
{
	public class KeyboardInputDetector : GestureDetector
	{
		public static System.Action<string> onInput;
		
		public override void Update()
		{
			string inputString = Input.inputString;
			if (string.IsNullOrEmpty(inputString))
			{
				return;
			}
			
			if (onInput != null)
			{
				onInput(inputString);
			}
		}
	}
}
