using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventBus
{
    public const string FLOW_EVENT = "flow_event";
    static Dictionary<string, List<IEventProcess>> processes = new Dictionary<string, List<IEventProcess>>();
    public static void Register(string name, IEventProcess process)
    {
        List<IEventProcess> list = null;
        if (!processes.ContainsKey(name))
        {
            list = new List<IEventProcess>();
            processes.Add(name, list);
        }
        else
        {
            list = processes[name];
        }

        list.Add(process);
    }

    public static void UnRegister(string name, IEventProcess process)
    {
        List<IEventProcess> list = null;
        if (processes.ContainsKey(name))
        {
            list = processes[name];
            list.Remove(process);
        }
    }

    public static void Post<T>(DemoEvent<T> eventO)
    {
        if (processes.ContainsKey(eventO.GetName()))
        {
            List<IEventProcess> list = processes[eventO.GetName()];
            Debug.Log("deliver event:" + eventO.GetName() + " process size:" + list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                IEventProcess process = list[i];
                process.Process(eventO);

            }
            return;
        }
        Debug.LogError("Not found Process register event:" + eventO.GetName() + "!!");
    }
}
