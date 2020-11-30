using Framework;

using UnityEngine;
using System.Collections;

namespace Tiinoo.DeviceConsole
{
	public interface IItemView
	{
		void Refresh(object item, bool evenRow, bool isSelected);
	}
}

