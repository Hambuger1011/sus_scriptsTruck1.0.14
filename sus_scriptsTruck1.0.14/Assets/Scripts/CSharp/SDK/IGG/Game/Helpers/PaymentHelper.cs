using System.Collections.Generic;
using IGG.SDK.Core.Error;
using IGG.SDK.Framework;
using IGG.SDK.Framework.Delegate;
using IGG.SDK.Modules.Compliance.VO;
using IGG.SDK.Modules.GeneralPayment;
using IGG.SDK.Modules.GeneralPayment.Delegate;
using IGG.SDK.Modules.Payment.Primary;
using IGG.SDK.Modules.Payment.Primary.Helper;
using IGG.SDK.Modules.Payment.Primary.VO;
using IGG.SDK.Modules.Payment.VO;
using IGG.SDK.Pay;
using Script.Game.Helpers;
using UnityEngine;
using Error = IGG.SDK.Modules.Payment.Error;

namespace Helpers
{
    /// <summary>
    /// 封装了一些支付相关的业务逻辑，简化研发接入支付。
    /// </summary>
    public class PaymentHelper
    {
        private static IGGInAppPurchase sPayment = null; // 支付实例
        private static OnPayOrSubscribeToListener sOnPayOrSubscribeToListener; // 支付结果监听。
        private static List<IGGGameItem> items;
        private static Dictionary<string,IGGGameItem> shopListMap = new Dictionary<string,IGGGameItem>();

        /// <summary>
        /// 设置支付结果回调监听
        /// </summary>
        /// <param name="onPayOrSubscribeToListener"></param>
        public static void InitPayment(IGGOnPreparedDelegate onPrepared, OnPayOrSubscribeToListener onPayOrSubscribeToListener)
        {
            sOnPayOrSubscribeToListener = onPayOrSubscribeToListener;
            KungfuInstance.Get().PrepareInAppPurchase(new OnPurchaseListener(OnIGGPurchaseFailed, OnIGGPurchaseFinished,
                OnIGGPurchaseStartingFinished, OnIGGSubscriptionShouldMakeRecurringPaymentsInstead),
                delegate(IGGError error)
                {
                    onPrepared(error);
                    if (error.IsNone())
                    {
                        sPayment = KungfuInstance.Get().GetPreparedInAppPurchase();

                    }
                });
        }

        public static void SetOnPayOrSubscribeToListener(OnPayOrSubscribeToListener onPayOrSubscribeToListener)
        {
            sOnPayOrSubscribeToListener = onPayOrSubscribeToListener;
        }
        
        /// <summary>
        /// 生成支付需要用到的payload信息
        /// </summary>
        /// <returns></returns>
        private static IGGPaymentPayload GetPaymentPayload()
        {
            IGGPaymentPayload payload = new IGGPaymentPayload("mock_char_id", 0);
            payload.SetValue("custom1", "custom_value1");
            payload.SetValue("custom2", "custom_value2");
            return payload;
        }

        /// <summary>
        /// 购买（订阅）失败回调。
        /// </summary>
        /// <param name="error"></param>
        /// <param name="type"></param>
        /// <param name="purchase"></param>
        public static void OnIGGPurchaseFailed(IGGError error, IGGPurchaseFailureType type, IGGIAPPurchase purchase)
        {
            Debug.LogError("onIGGPurchaseFailed:" + error.GetCode());
            if (type == IGGPurchaseFailureType.Canceled)
            {
                //特殊处理 用户取消购买
                sOnPayOrSubscribeToListener?.onCancel?.Invoke(purchase);
            }
            else
            {
                sOnPayOrSubscribeToListener?.onFailed?.Invoke(error, type, purchase);
            }
        }

        /// <summary>
        /// 购买（订阅）预处理遇到错误回调（比如订阅过程中发现当前IGGID在后台拥有有效的订阅。）
        /// </summary>
        /// <param name="error"></param>
        public static void OnIGGPurchaseStartingFinished(IGGError error)
        {
            Debug.Log("onIGGPurchaseStartingFinished:" + error.GetCode());
            if (Error.PAYMENT_ERROR_FOR_HAS_SUBSCRIBED_BY_IGGID == error.GetCode())
            {
                //提示用户已订阅
                sOnPayOrSubscribeToListener.onFailedForHasSubscribe();
            }
            else
            {
                sOnPayOrSubscribeToListener.onStartingFailed(error);
            }
        }

        /// <summary>
        /// 某些订阅无法订阅时，要找到对应的消耗类商品进行购买。
        /// </summary>
        /// <param name="gameItem"></param>
        public static void OnIGGSubscriptionShouldMakeRecurringPaymentsInstead(IGGGameItem gameItem)
        {
            Debug.Log("onIGGSubscriptionShouldMakeRecurringPaymentsInstead:" + gameItem.GetId());
            LoadItems(new OnLoadPaymentItemsCacheOrServerListener(delegate (IGGError error, List<IGGGameItem> gameItems)
            {
                if (error.IsNone())
                {
                    bool isOrderItemExist = false;
                    foreach (var item in gameItems)
                    {
                        if (item.GetAssociatedSubscriptionItemId() == gameItem.GetId())
                        {
                            PayOrSubscribeTo(item);
                            isOrderItemExist = true;
                            break;
                        }
                    }
                    if (!isOrderItemExist)
                    {
                        var message = "订阅商品：" + gameItem.GetId() + "并未设置对应的消耗类商品！！请联系技术部相关人员！！！";
                        Debug.LogError(message);
                        sOnPayOrSubscribeToListener?.shouldMakeRecurringPaymentsInsteadFail?.Invoke(message);
                    }
                }
                else
                {
                    var message = "寻找订阅对应消耗品时，获取商品列表失败。" + error.GetCode();
                    Debug.LogError(message);
                    sOnPayOrSubscribeToListener?.shouldMakeRecurringPaymentsInsteadFail?.Invoke(message);
                }
            }));
        }

        /// <summary>
        /// 购买（订阅）成功回调。
        /// </summary>
        /// <param name="error"></param>
        /// <param name="purchase"></param>
        /// <param name="result"></param>
        public static void OnIGGPurchaseFinished(IGGError error, IGGIAPPurchase purchase, IGGPurchaseResult result)
        {
            Debug.Log("onIGGPurchaseFinished " + purchase.GetOrderId());
            if (result != null)
            {
                IGGPaymentDeliveryState state = result.GetDeliveryState();
                Debug.Log("onIGGPurchaseFinished  state " + state);
                sOnPayOrSubscribeToListener?.onSuccess?.Invoke(purchase, result);
            }
            else
            {
                Debug.LogError("onIGGPurchaseFinished Unknown Error. result is null.");
                sOnPayOrSubscribeToListener?.onFailed?.Invoke(error, IGGPurchaseFailureType.Purchase, purchase);
            }
        }

        /// <summary>
        /// 在初始化过程中将加载到的点卡缓存到PaymentHelper
        /// </summary>
        /// <param name="items"></param>
        public static void SetGameItems(List<IGGGameItem> items)
        {
            PaymentHelper.items = items;

            #region MyRegion InitProductMap

            string tempProductInfo = "";

            int len = items.Count;
            for (int i = 0; i < len; i++)
            {
                IGGGameItem tempItem = items[i];
                if (tempItem != null)
                {
                    tempProductInfo +=" ID:"+ tempItem.GetId() + " Title:" + tempItem.GetTitle();
                    if (shopListMap.ContainsKey(tempItem.GetId()))
                    {
                        shopListMap[tempItem.GetId()] = tempItem;
                    }
                    else
                    {
                        shopListMap.Add(tempItem.GetId(),tempItem);
                    }
                }
            }
            
            Debug.LogError("===product info list==>"+tempProductInfo);

            #endregion
        }

        /// <summary>
        /// 获得商品Item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static IGGGameItem GetGameItem(string id)
        {
            if (shopListMap.ContainsKey(id))
            {
                return shopListMap[id];
            }
            return null;
        }
        
        
        /// <summary>
        /// 获得当前商品列表
        /// </summary>
        /// <returns></returns>
        public static int GetShopListCount()
        {
            return shopListMap.Count;
        }

        /// <summary>
        /// 封装获取商品列表逻辑
        /// </summary>
        /// <param name="listener"></param>
        public static void LoadItems(OnLoadPaymentItemsCacheOrServerListener listener)
        {
            listener.onPaymentItemsLoadFinished(IGGError.NoneError(), items);
        }

        /// <summary>
        /// 封装获取当前账号的限购情况
        /// </summary>
        /// <returns></returns>
        public static int GetPurchaseLimit()
        {
            if (sPayment != null)
            {
                return sPayment.GetPurchaseLimit();
            }

            return 0;
        }
        
        public static void SetPurchaseDialog(IGGMockPurchaseDialog purchaseDialog)
        {
            sPayment.SetPurchaseDialog(purchaseDialog);
        } 
        
        public static void AddMockPaymentMonoBehaviour(GameObject moduleMockPanel)
        {
            Debug.LogError("SetMockPaymentMonoBehaviour");
            GameObject mockPayment = moduleMockPanel.transform.Find("MockPayment").gameObject;
            // 挂载IGGMockPaymentMonoBehaviour脚本到mockPayment gameobject
            mockPayment.AddComponent<IGGMockPaymentMonoBehaviour>();
        } 
        
        public static void SetMockPaymentMonoBehaviour(GameObject moduleMockPanel)
        {
            Debug.LogError("SetMockPaymentMonoBehaviour");
            GameObject mockPayment = moduleMockPanel.transform.Find("MockPayment").gameObject;
      
            // 获取IGGMockPaymentMonoBehaviour脚本实例
            IGGMockPaymentMonoBehaviour mockPaymentMonoBehaviour = mockPayment.GetComponent<IGGMockPaymentMonoBehaviour>();
            KungfuInstance.Get().GetPreparedModuleMockMonoBehaviourPool().SetMockPaymentMonoBehaviour(mockPaymentMonoBehaviour);
        } 

        /// <summary>
        /// 封装购买或订阅的逻辑
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool PayOrSubscribeTo(IGGGameItem item)
        {
            // 当支付模块可用的情况下才能去执行购买或订阅
            if (sPayment == null || !sPayment.IsAvailable())
            {
                //关闭转菊花
                UINetLoadingMgr.Instance.Close2();
                Debug.LogError("当支付模块可用的情况下才能去执行购买或订阅;sPayment:"+sPayment);
                return false;
            }

            // 获取防沉迷信息，用于支付限购，这个限购是政策方面的限购，跟上面的限购情况不一样（如果游戏有需要开启防沉迷）
            IGGPurchaseRestriction purchaseRestriction = null;
            if (null != IGGRealnameVerificationConfig.SharedInstance().GetRestrictions())
            {
                //关闭转菊花
                UINetLoadingMgr.Instance.Close2();
                Debug.LogError("限制配置不为空");
                purchaseRestriction = IGGRealnameVerificationConfig.SharedInstance().GetRestrictions().PurchaseRestriction;
            }
            
            if (IGGGameItem.Type.CONSUMABLE == item.GetTypeValue()) // 消耗类商品调用购买接口
            {
                Debug.Log("消耗类商品调用购买接口：开始拉起支付");
                //ViewUtil.ShowLoading();
                sPayment.Pay(item, purchaseRestriction, GetPaymentPayload());
            }
            else if (IGGGameItem.Type.AUTO_RENEWABLE_SUBSCRIPTION == item.GetTypeValue()) // 订阅类商品调用订阅接口
            {
                Debug.Log("订阅类商品调用订阅接口：开始拉起支付");
                //ViewUtil.ShowLoading();
                sPayment.SubscribeTo(item, purchaseRestriction, GetPaymentPayload());
            }
            else // 其他不做处理（一般不会出现）
            {
                Debug.LogError("出现不支持的商品类型:" + item.GetType());
            }
            return true;
        }

        /// <summary>
        /// 支付模块初始化结果监听
        /// </summary>
        public class OnInitPaymentListener
        {
            public delegate void OnSuccess();  // 初始化成功
            public delegate void OnFailed(IGGError error); // 初始化失败

            public OnSuccess onSuccess;
            public OnFailed onFailed;

            public OnInitPaymentListener(OnSuccess onSuccess, OnFailed onFailed)
            {
                this.onSuccess = onSuccess;
                this.onFailed = onFailed;
            }
        }

        /// <summary>
        /// 购买跟订阅结果监听
        /// </summary>
        public class OnPayOrSubscribeToListener
        {
            public delegate void OnSuccess(IGGIAPPurchase purchase, IGGPurchaseResult result); // 购买或订阅成功
            public delegate void OnFailed(IGGError error, IGGPurchaseFailureType type, IGGIAPPurchase purchase); // 购买或订阅失败

            public delegate void OnStartingFailed(IGGError error); // 购买或订阅准备阶段失败（主要是平台无法支付）

            public delegate void OnFailedForHasSubscribe(); // 当前平台账号已订阅这个商品，请用消耗类商品完成该商品的订阅
            public delegate void OnCancel(IGGIAPPurchase purchase); // 玩家取消购买

            public delegate void OnIGGSubscriptionShouldMakeRecurringPaymentsInsteadFail(string message);

            public OnSuccess onSuccess;

            public OnFailed onFailed;
            public OnStartingFailed onStartingFailed;
            public OnFailedForHasSubscribe onFailedForHasSubscribe;
            public OnCancel onCancel;
            public OnIGGSubscriptionShouldMakeRecurringPaymentsInsteadFail shouldMakeRecurringPaymentsInsteadFail;

            public OnPayOrSubscribeToListener(OnSuccess onSuccess, OnFailed onFailed, OnStartingFailed onStartingFailed
                , OnFailedForHasSubscribe onFailedForHasSubscribe, OnCancel onCancel
                , OnIGGSubscriptionShouldMakeRecurringPaymentsInsteadFail shouldMakeRecurringPaymentsInsteadFail)
            {
                this.onSuccess = onSuccess;
                this.onFailed = onFailed;
                this.onStartingFailed = onStartingFailed;
                this.onFailedForHasSubscribe = onFailedForHasSubscribe;
                this.onCancel = onCancel;
                this.shouldMakeRecurringPaymentsInsteadFail = shouldMakeRecurringPaymentsInsteadFail;
            }
        }

        /// <summary>
        /// 商品列表加载结果监听
        /// </summary>
        public class OnLoadPaymentItemsCacheOrServerListener : IGGPaymentItemsListener
        {
            public delegate void OnPaymentItemsLoadFinished(IGGError error, List<IGGGameItem> gameItems);

            public OnPaymentItemsLoadFinished onPaymentItemsLoadFinished;

            public OnLoadPaymentItemsCacheOrServerListener(OnPaymentItemsLoadFinished onPaymentItemsLoadFinished)
            {
                this.onPaymentItemsLoadFinished = onPaymentItemsLoadFinished;
            }

            void IGGPaymentItemsListener.OnLoadCachePaymentItemsFinished(List<IGGGameItem> gameItems)
            {
                onPaymentItemsLoadFinished?.Invoke(IGGError.NoneError(), gameItems);
            }

            void IGGPaymentItemsListener.OnPaymentItemsLoadFinished(IGGError error, List<IGGGameItem> gameItems)
            {
                onPaymentItemsLoadFinished?.Invoke(error, gameItems);
            }
        }
        
        /// <summary>
        /// 商品列表加载结果监听
        /// </summary>
        public class OnLoadPaymentItemsListener : IGGPaymentItemsListener
        {
            public delegate void OnLoadCachePaymentItemsFinished(List<IGGGameItem> gameItems);

            public delegate void OnPaymentItemsLoadFinished(IGGError error, List<IGGGameItem> gameItems);

            public OnLoadCachePaymentItemsFinished onLoadCachePaymentItemsFinished;

            public OnPaymentItemsLoadFinished onPaymentItemsLoadFinished;

            public OnLoadPaymentItemsListener(OnLoadCachePaymentItemsFinished onLoadCachePaymentItemsFinished, OnPaymentItemsLoadFinished onPaymentItemsLoadFinished)
            {
                this.onLoadCachePaymentItemsFinished = onLoadCachePaymentItemsFinished;
                this.onPaymentItemsLoadFinished = onPaymentItemsLoadFinished;
            }

            void IGGPaymentItemsListener.OnLoadCachePaymentItemsFinished(List<IGGGameItem> gameItems)
            {
                onLoadCachePaymentItemsFinished?.Invoke(gameItems);
            }

            void IGGPaymentItemsListener.OnPaymentItemsLoadFinished(IGGError error, List<IGGGameItem> gameItems)
            {
                onPaymentItemsLoadFinished?.Invoke(error, gameItems);
            }
        }

        public class OnPurchaseListener : IGGPurchaseListener
        {
            public delegate void OnIGGPurchaseFailed(IGGError error, IGGPurchaseFailureType type, IGGIAPPurchase purchase);
            public delegate void OnIGGPurchaseFinished(IGGError error, IGGIAPPurchase purchase, IGGPurchaseResult result);
            public delegate void OnIGGPurchaseStartingFinished(IGGError error);
            public delegate void OnIGGSubscriptionShouldMakeRecurringPaymentsInstead(IGGGameItem gameItem);

            private OnIGGPurchaseFailed onIGGPurchaseFailed;
            private OnIGGPurchaseFinished onIGGPurchaseFinished;
            private OnIGGPurchaseStartingFinished onIGGPurchaseStartingFinished;
            private OnIGGSubscriptionShouldMakeRecurringPaymentsInstead onIGGSubscriptionFailed;

            public OnPurchaseListener(OnIGGPurchaseFailed onIGGPurchaseFailed, OnIGGPurchaseFinished onIGGPurchaseFinished
                , OnIGGPurchaseStartingFinished onIGGPurchaseStartingFinished, OnIGGSubscriptionShouldMakeRecurringPaymentsInstead onIGGSubscriptionFailed)
            {
                this.onIGGPurchaseFailed = onIGGPurchaseFailed;
                this.onIGGPurchaseFinished = onIGGPurchaseFinished;
                this.onIGGPurchaseStartingFinished = onIGGPurchaseStartingFinished;
                this.onIGGSubscriptionFailed = onIGGSubscriptionFailed;
            }

            /// <summary>
            /// 购买（订阅）失败回调。
            /// </summary>
            /// <param name="error"></param>
            /// <param name="type"></param>
            /// <param name="purchase"></param>
            void IGGPurchaseListener.OnIGGPurchaseFailed(IGGError error, IGGPurchaseFailureType type, IGGIAPPurchase purchase)
            {
                onIGGPurchaseFailed?.Invoke(error, type, purchase);
            }
            
            /// <summary>
            /// 购买（订阅）成功回调。
            /// </summary>
            /// <param name="error"></param>
            /// <param name="purchase"></param>
            /// <param name="result"></param>
            void IGGPurchaseListener.OnIGGPurchaseFinished(IGGError error, IGGIAPPurchase purchase, IGGPurchaseResult result)
            {
                onIGGPurchaseFinished?.Invoke(error, purchase, result);
            }

            /// <summary>
            /// 购买（订阅）预处理遇到错误回调（比如订阅过程中发现当前IGGID在后台拥有有效的订阅。）
            /// </summary>
            /// <param name="error"></param>
            void IGGPurchaseListener.OnIGGPurchaseStartingFinished(IGGError error)
            {
                onIGGPurchaseStartingFinished?.Invoke(error);
            }

            /// <summary>
            /// 当前谷歌账号不允许进行订阅该商品操作（当前谷歌账号已订阅过该类订阅商品）。
            /// </summary>
            /// <param name="gameItem"></param>
            void IGGPurchaseListener.OnIGGSubscriptionShouldMakeRecurringPaymentsInstead(IGGGameItem gameItem)
            {
                onIGGSubscriptionFailed?.Invoke(gameItem);
            }
        }
    }
}