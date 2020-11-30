using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataCenter
{
    static Dictionary<FlowData.FlowType, FlowData> flowStatus = new Dictionary<FlowData.FlowType, FlowData>();
    static FlowEventProcess flowEventProcess;
    static DataCenter()
    {
        InitFlowData();
    }

    private static void InitFlowData()
    {
        //初始化所有 Flow 的状态(包括加载appconf、自动登录、初始化支付、后台任务-加载实名认证状态)
        List<FlowData.FlowType> types = new List<FlowData.FlowType>() { FlowData.FlowType.LOAD_APPCONFIG, FlowData.FlowType.LOGIN, FlowData.FlowType.INIT_PAY, FlowData.FlowType.TASK_ACCOUNT_SAFE_LV };
        foreach (var item in types)
        {
            FlowData flowData = new FlowData();
            flowData.SetFlowType(item);
            flowData.IsDone(false);
            flowStatus.Add(item, flowData);
        }

        flowEventProcess = new FlowEventProcess();
        EventBus.Register(EventBus.FLOW_EVENT, flowEventProcess);
    }

    public static ICollection<FlowData> GetAllFlow()
    {
        return flowStatus.Values;
    }

    public static bool IsAllFlowDone()
    {
        foreach (var value in flowStatus.Values)
        {
            if (!value.GetDone())
            {
                return false;
            }
        }
        return true;
    }

    public static FlowData GetFlow(FlowData.FlowType type) {
       if (flowStatus.ContainsKey(type)) {
            return flowStatus[type];
       }
       return new FlowData();
    }

    public class FlowEventProcess : IEventProcess
    {
        void IEventProcess.Process<T>(DemoEvent<T> demoEvent)
        {
            if (demoEvent.GetData() is FlowData)
            {
                FlowData flowData = (FlowData)System.Convert.ChangeType(demoEvent.GetData(), typeof(FlowData));
                flowStatus[flowData.GetFlowType()] = flowData;
            }
        }
    }

    public static void OnDestory()
    {
        EventBus.UnRegister(EventBus.FLOW_EVENT, flowEventProcess);
        flowStatus.Clear();
        InitFlowData();
    }
}
