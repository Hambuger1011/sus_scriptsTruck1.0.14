using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{

    public class ChatMsgListViewDemoScript : MonoBehaviour
    {
        public LoopListView2 mLoopListView;
        public Button mScrollToButton;
        public InputField mScrollToInput;
        public Button mBackButton;
        public Button mAppendMsgButton;
        public Button insertMsgBtn;
        public Button changeMsgBtn;

        // Use this for initialization
        void Start()
        {
            mScrollToButton.onClick.AddListener(OnJumpBtnClicked);
            mBackButton.onClick.AddListener(OnBackBtnClicked);
            mAppendMsgButton.onClick.AddListener(OnAppendMsgBtnClicked);
            insertMsgBtn.onClick.AddListener(OnInsertMsgBtnClicked);
            changeMsgBtn.onClick.AddListener(OnChangeMsgBtnClicked);


            Debug.Log("初始化个数:" + ChatMsgDataSourceMgr.Get.TotalItemCount);
            mLoopListView.InitListView(ChatMsgDataSourceMgr.Get.TotalItemCount, OnGetItemByIndex);
        }

        /// <summary>
        /// 实例化item
        /// </summary>
        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
        {
            if (index < 0 || index >= ChatMsgDataSourceMgr.Get.TotalItemCount)
            {
                return null;
            }

            ChatMsg itemData = ChatMsgDataSourceMgr.Get.GetChatMsgByIndex(index);
            if (itemData == null)
            {
                return null;
            }
            LoopListViewItem2 item = null;
            if (itemData.mPersonId == 0)
            {
                item = listView.NewListViewItem("ItemPrefab1");
            }
            else
            {
                item = listView.NewListViewItem("ItemPrefab2");
            }
            ListItem4 demoItem = item.GetComponent<ListItem4>();
            if (item.IsInitHandlerCalled == false)//第一次创建
            {
                item.IsInitHandlerCalled = true;
                demoItem.Init();
            }
            demoItem.SetItemData(itemData, index);
            return item;
        }

        /// <summary>
        /// 返回主菜单
        /// </summary>
        void OnBackBtnClicked()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        }




        /// <summary>
        /// 跳转
        /// </summary>
        void OnJumpBtnClicked()
        {
            int itemIndex = 0;
            if (int.TryParse(mScrollToInput.text, out itemIndex) == false)
            {
                return;
            }
            if (itemIndex < 0)
            {
                return;
            }
            mLoopListView.MovePanelToItemIndex(itemIndex, 0);
        }

        /// <summary>
        /// 追加数据
        /// </summary>
        void OnAppendMsgBtnClicked()
        {
            ChatMsgDataSourceMgr.Get.AppendOneMsg();
            mLoopListView.SetListItemCount(ChatMsgDataSourceMgr.Get.TotalItemCount, false);
            mLoopListView.MovePanelToItemIndex(ChatMsgDataSourceMgr.Get.TotalItemCount - 1, 0);
        }


        /// <summary>
        /// 插入数据
        /// </summary>
        void OnInsertMsgBtnClicked()
        {
            int itemIndex = 0;
            if (int.TryParse(mScrollToInput.text, out itemIndex) == false)
            {
                return;
            }
            if (itemIndex < 0)
            {
                return;
            }
            ChatMsgDataSourceMgr.Get.InsertOneMsg(itemIndex);
            mLoopListView.SetListItemCount(ChatMsgDataSourceMgr.Get.TotalItemCount, false);
            mLoopListView.MovePanelToItemIndex(0, 0);
        }



        /// <summary>
        /// 改变数据
        /// </summary>
        void OnChangeMsgBtnClicked()
        {
            int itemIndex = 0;
            if (int.TryParse(mScrollToInput.text, out itemIndex) == false)
            {
                return;
            }
            if (itemIndex < 0)
            {
                return;
            }
            ChatMsgDataSourceMgr.Get.ChangeItem(itemIndex);
            //mLoopListView.SetListItemCount(ChatMsgDataSourceMgr.Get.TotalItemCount, false);
            //mLoopListView.MovePanelToItemIndex(0, 0);
            mLoopListView.RefreshItemByItemIndex(itemIndex);
        }

    }

}
