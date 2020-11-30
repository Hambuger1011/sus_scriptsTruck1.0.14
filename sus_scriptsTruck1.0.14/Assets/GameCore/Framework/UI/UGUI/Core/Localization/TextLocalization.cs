﻿using Framework;
using UnityEngine;
using UnityEngine.UI;

namespace UGUI
{
    [DisallowMultipleComponent]
	public class TextLocalization : MonoBehaviour
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
            if (!GameDataMgr.HasInstance())
            {
                return;
            }
            var data = GameDataMgr.Instance.table.GetLocalizationById(this.m_key);
            if (string.IsNullOrEmpty(data))
            {
	            LOG.Error($"获取配置文本{m_key}为空");
	            return;
            }
            m_text.text = data;
            var contentSizeFitter = this.GetComponent<ContentSizeFitter>();
            if (null!= contentSizeFitter)
	            contentSizeFitter.SetLayoutVertical();
        }
	}
}
