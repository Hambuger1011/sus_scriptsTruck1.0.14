using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoEvent<T>
{
    private string name;
    private T data;

    public string GetName()
    {
        return this.name;
    }

    public void SetName(string name)
    {
        this.name = name;
    }

    public T GetData()
    {
        return this.data;
    }

    public void SetData(T data)
    {
        this.data = data;
    }
}
