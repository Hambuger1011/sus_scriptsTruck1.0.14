using Framework;

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text;
using System;
using Tiinoo.DeviceConsole;
using UberLogger;

namespace Tiinoo.DeviceConsole
{
	public class WindowConsole : MonoBehaviour
	{
		#region inspector
		public GameObject objConsole;

		public GameObject btnToMaxSize;
		public GameObject btnToNormalSize;

        [Header("切换显示")]
        public Toggle infoToggle;
        public Toggle warningToggle;
        public Toggle errorToggle;
        public Toggle exceptionToggle;

        [Header("log显示数量")]
		public Text uiInfoCount;
		public Text uiWarningCount;
		public Text uiErrorCount;
        public Text uiExceptionCount;

        [Header("搜索框")]
        public InputField uiFilter;

		public InfinityScrollVerticalLayoutGroup consoleLayoutGroup;

		public ScrollRect stackTraceScrollRect;

        [Header("栈信息")]
		public Text uiStackTrace;

        [Header("图标")]
		public Sprite iconInfo;
		public Sprite iconWarning;
		public Sprite iconError;
        public Sprite iconException;

        [Header("颜色")]
        public Color bgLogLight;
		public Color bgLogDark;
		public Color bgLogSelected;

        [Header("调用栈")]
        public InputField inLogStack;
		#endregion
        

		public static bool isVisible;

		private bool m_isInited;

        //是否显示
		private bool m_isShowInfo = true;
		private bool m_isShowWarning = true;
		private bool m_isShowError = true;
        bool m_isShowException = true;

        //log数量
        private int m_infoCount;
		private int m_warningCount;
		private int m_errorCount;
        private int m_exceptionCount;

        private RectTransform m_consoleRectTransform;
		private float m_consoleNormalHeight;

		private string METHOD_DO_FILTER = "FilterLogsAndRefresh";
		private string m_searchText = "";
		private bool m_isSearchTextAtBegin = false;

		private bool m_isDirty;

        /// <summary>
        /// 需要显示的日志
        /// </summary>
        List<LogInfo> m_filteredLogBuffer = new List<LogInfo>();

        #region singleton
        private static WindowConsole m_instance;
		
		public static WindowConsole Instance
		{
			get {return m_instance;}
		}
		
		void Awake()
		{
			m_instance = this;
            infoToggle.onValueChanged.AddListener(ShowHideInfo);
            warningToggle.onValueChanged.AddListener(ShowHideWarning);
            errorToggle.onValueChanged.AddListener(ShowHideError);
            exceptionToggle.onValueChanged.AddListener(ShowException);
        }
		#endregion
		
		void OnEnable()
		{
			isVisible = true;
			LogHandler.onLogAdded += HandleOnLogAdded;
			consoleLayoutGroup.onItemSelected += HandleOnLogSelected;

			if (!m_isInited)
			{
				Init();
				m_isInited = true;
			}

			if (m_isInited)
			{
				FilterLogsAndRefresh();
			}
		}
		
		void OnDisable()
		{
			isVisible = false;
			LogHandler.onLogAdded -= HandleOnLogAdded;
			consoleLayoutGroup.onItemSelected -= HandleOnLogSelected;
		}

		void Update()
		{
			if (m_isDirty)
			{
				Refresh();
				m_isDirty = false;
			}
		}

		void Init()
		{
			m_consoleRectTransform = objConsole.GetComponent<RectTransform>();
			m_consoleNormalHeight = m_consoleRectTransform.sizeDelta.y;
			
			StretchToNormalSize();
		}

		private void MarkDirty()
		{
			m_isDirty = true;
		}

		#region action
		private void HandleOnLogAdded(LogInfo log)
		{
			bool isPassed = StatisticAndFilter(log);
			if (isPassed)
			{
				consoleLayoutGroup.AddItem(log);
			}
			RefreshLogCount();
		}
		
		private void HandleOnLogSelected(object item)
		{
			LogInfo log = item as LogInfo;
			SetStackTrace(log);
		}
		
		private void SetStackTrace(LogInfo log)
		{
            if(log == null)
            {
                uiStackTrace.text = string.Empty;
                inLogStack.text = "";
            }
            else
            {
                uiStackTrace.text = log.GetSimpleMessage() + System.Environment.NewLine+log.GetStackTrace();
                inLogStack.text = log.GetSimpleMessage();
            }

   //         if (log != null)
			//{
			//	string stackTrace = string.Empty;
			//	if (string.IsNullOrEmpty(log.stackTrace))
			//	{
			//		stackTrace = log.message + "\n" + STR_NO_STACK_TRACE;
			//	}
			//	else
			//	{
			//		stackTrace = log.message + "\n" + log.stackTrace;
			//	}
				
			//	if (stackTrace.Length > STACK_TRACE_MAX_LENGTH)
			//	{
			//		stackTrace = stackTrace.Substring(0, STACK_TRACE_MAX_LENGTH);
			//		stackTrace = stackTrace + "\n" + STR_TRUNCATED;
			//	}
				
			//	uiStackTrace.text = stackTrace;
			//}
			//else
			//{
			//	uiStackTrace.text = "";
			//}
			
			stackTraceScrollRect.normalizedPosition = new Vector2(0, 1);
		}

        /// <summary>
        /// 显示普通日志
        /// </summary>
		public void ShowHideInfo(bool isShow)
		{
			m_isShowInfo = isShow;
			FilterLogsAndRefresh();
		}

        /// <summary>
        /// 显示普通警告
        /// </summary>
        public void ShowHideWarning(bool isShow)
		{
			m_isShowWarning = isShow;
			FilterLogsAndRefresh();
		}

        /// <summary>
        /// 显示普通错误
        /// </summary>
        public void ShowHideError(bool isShow)
		{
			m_isShowError = isShow;
			FilterLogsAndRefresh();
		}

        /// <summary>
        /// 显示异常
        /// </summary>
        /// <param name="isShow"></param>
        public void ShowException(bool isShow)
        {
            m_isShowException = isShow;
            FilterLogsAndRefresh();
        }


        bool mIsCleaning = false;
		public void Clean()
        {
            mIsCleaning = true;
            m_filteredLogBuffer.Clear();
            UberLogger.Logger.Clear();
			SetStackTrace(null);
			FilterLogsAndRefresh();
            mIsCleaning = false;

        }
		
		public void StretchToNormalSize()
		{
			btnToNormalSize.SetActive(false);
			btnToMaxSize.SetActive(true);
			m_consoleRectTransform.sizeDelta = new Vector2(m_consoleRectTransform.sizeDelta.x, m_consoleNormalHeight);
		}
		
		public void StretchToMaxSize()
		{
			btnToNormalSize.SetActive(true);
			btnToMaxSize.SetActive(false);
			m_consoleRectTransform.sizeDelta = new Vector2(m_consoleRectTransform.sizeDelta.x, UIWindowMgr.Instance.CanvasHeight);
		}
		
        /// <summary>
        /// 搜索相关日志
        /// </summary>
		public void HandleOnSearchStringChange(string strKey)
		{
			m_searchText = strKey.ToLower();
			if (IsInvoking(METHOD_DO_FILTER))
			{
				CancelInvoke(METHOD_DO_FILTER);
			}
			Invoke(METHOD_DO_FILTER, 0.3f);
		}

		public void ClearFilterString()
		{
			uiFilter.text = "";	// this will trigger a InputFiled.onValueChanged event, which will call HandleOnFilterStringChange()
		}

		private bool HasSearchText()
		{
			if (string.IsNullOrEmpty(m_searchText))
			{
				return false;
			}
			return true;
		}

		public void HandleOnSearchTextAtBeginChange(bool val)
		{
			m_isSearchTextAtBegin = val;

			if (!HasSearchText())
			{
				return;
			}

			if (IsInvoking(METHOD_DO_FILTER))
			{
				return;
			}

			FilterLogsAndRefresh();
		}

		public void Close()
		{
			SetStackTrace(null);
			UIWindowMgr.Instance.CloseCurrentWindow();
		}
		#endregion

		private void Refresh()
		{
			object curSelectedItem = consoleLayoutGroup.SelectedItem;
			consoleLayoutGroup.ClearItems();
            
			for (int i=0; i < m_filteredLogBuffer.Count; ++i)
			{
				consoleLayoutGroup.AddItem(m_filteredLogBuffer[i]);
			}

			if (curSelectedItem != null)
			{
				consoleLayoutGroup.SelectedItem = curSelectedItem;
			}

			RefreshLogCount();
		}
		
		private void RefreshLogCount()
		{
			uiInfoCount.text = CreateLogTypeText(m_infoCount);
			uiWarningCount.text = CreateLogTypeText(m_warningCount);
			uiErrorCount.text = CreateLogTypeText(m_errorCount);
            uiExceptionCount.text = CreateLogTypeText(m_exceptionCount);

        }
		
		private static string CreateLogTypeText(int logCount)
		{
			string s = (logCount > 999) ? "999+" : (logCount + "");
			return s;
		}
		
		private void ResetLogCount()
		{
			m_infoCount = 0;
			m_warningCount = 0;
			m_errorCount = 0;
            m_exceptionCount = 0;
		}
		
		private void StatisticLogCount(LogInfo log)
		{
			LogType logType = log.logType;
			if (logType == LogType.Log)
			{
				m_infoCount++;
			}
			else if (logType == LogType.Warning)
			{
				m_warningCount++;
			}
			else if (logType == LogType.Error || logType == LogType.Assert)
			{
				m_errorCount++;
            }
            else if (logType == LogType.Exception)
            {
                m_exceptionCount++;
            }
		}

		private bool FilterPassCheck1(LogInfo log)
		{
			if (HasSearchText())
			{
				if (!m_isSearchTextAtBegin && !log.Message.Contains(m_searchText))
				{
					return false;
				}
				else if (m_isSearchTextAtBegin && !log.Message.StartsWith(m_searchText))
				{
					return false;
				}
			}

			return true;
		}
		
		private bool FilterPassCheck2(LogInfo log)
		{
			LogType logType = log.logType;
			if ((logType == LogType.Log && !m_isShowInfo)
			    || (logType == LogType.Warning && !m_isShowWarning)
			    || (logType == LogType.Error && !m_isShowError)
			    || (logType == LogType.Exception && !m_isShowException)
			    || (logType == LogType.Assert && !m_isShowError))
			{
				return false;
			}

			return true;
		}

		private void FilterLogsAndRefresh()
		{
			FilterLogs();
			MarkDirty();
		}

		private void FilterLogs()
		{
			ResetLogCount();
            m_filteredLogBuffer.Clear();
            if(!mIsCleaning)
            {
	            foreach(var log in UberLogger.Logger.RecentMessages)
	            {
		            StatisticAndFilter(log);
	            }
            }
		}

		private bool StatisticAndFilter(LogInfo log)
		{
			bool isPassed = FilterPassCheck1(log);
			if (isPassed)
			{
				StatisticLogCount(log);
				isPassed = FilterPassCheck2(log);
				if (isPassed)
				{
					m_filteredLogBuffer.Add(log);
				}
			}
			return isPassed;
		}

        public void OnClickLogStack()
        {
            inLogStack.text = this.uiStackTrace.text;
        }
	}
}


