using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEventProcess
{
    void Process<T>(DemoEvent<T> demoEvent);
}