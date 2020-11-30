using Framework;

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Tiinoo.DeviceConsole
{
	[RequireComponent(typeof(Button))]
	public class KeyButton : MonoBehaviour 
	{
		public KeyCode key;

		private Button m_button;
		private Graphic m_targetGraphic;
		private Color m_normalColor;
		
		void Awake() 
		{
			m_button = GetComponent<Button>();
			m_targetGraphic = m_button.targetGraphic;
		}
		
		void Update() 
		{
			if (Input.GetKeyDown(key)) 
			{
				HandleKeyDown();
			} 
			else if (Input.GetKeyUp(key)) 
			{
				HandleKeyUp();
			}
		}
		
		private void HandleKeyUp() 
		{
			StartColorTween(m_button.colors.normalColor, false);
		}
		
		private void HandleKeyDown() 
		{
			StartColorTween(m_button.colors.pressedColor, false);
			m_button.onClick.Invoke();
		}
		
		private void StartColorTween(Color targetColor, bool immediate) 
		{
			if (m_targetGraphic == null)
			{
				return;
			}
			
			float duration = immediate ? 0f : m_button.colors.fadeDuration;
			m_targetGraphic.CrossFadeColor(targetColor, duration, true, true);
		}
	}
}

