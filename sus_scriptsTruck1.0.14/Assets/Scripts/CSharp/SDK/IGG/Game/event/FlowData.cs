using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowData
{
    private FlowType flowType = FlowType.UNKNOWN;
    private bool done = false;

    public FlowType GetFlowType()
    {
        return this.flowType;
    }

    public void SetFlowType(FlowType flowType)
    {
        this.flowType = flowType;
    }

    public bool GetDone()
    {
        return this.done;
    }

    public void IsDone(bool done)
    {
        this.done = done;
    }

    public enum FlowType
    {
        UNKNOWN,
        LOAD_APPCONFIG,
        LOGIN,
        INIT_PAY,
        // TASK_SIGNING,
        // TASK_WEGAMERS_UNREAD_MSG,
        TASK_ACCOUNT_SAFE_LV
    }
}
