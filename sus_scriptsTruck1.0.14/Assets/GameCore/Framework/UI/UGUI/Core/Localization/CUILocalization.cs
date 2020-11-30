using Framework;
using UnityEngine;
using UnityEngine.UI;

namespace UGUI
{
    [DisallowMultipleComponent]
	public class CUILocalization : MonoBehaviour
	{
		public int m_key;

		private Text m_text;

        private void Awake()
        {
            m_text = this.GetComponent<Text>();
            if(m_text == null)
            {
                Destroy(this);
                return;
            }
            SetDisplay();
        }

		public void SetKey(string key)
		{
			m_key = 0;
			SetDisplay();
		}

		public void SetDisplay()
        {
            return;
            if (!GameDataMgr.HasInstance())
            {
                return;
            }
            var data = GameDataMgr.Instance.table.GetLocalizationById(this.m_key);
            m_text.text = data;
        }
	}
}
