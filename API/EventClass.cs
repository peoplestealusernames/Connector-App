using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Event
{
    private Dictionary<string, List<System.Action<object>>> EventTab;

    public Event()
    {
        this.EventTab = new Dictionary<string, List<System.Action<object>>>();
    }

    //TODO: off and once
    public void on(string Name, System.Action<object> FNC)
    {
        if (this.EventTab.ContainsKey(Name))
        {
            this.EventTab[Name].Add(FNC);
        }
        else
        {
            this.EventTab[Name] = new List<System.Action<object>>();
            this.EventTab[Name].Add(FNC);
        }
    }

    public void emit(string Name, object Arg)
    {
        if (this.EventTab.ContainsKey(Name))
        {
            for (int i = 0; i < this.EventTab[Name].Count; i++)
            {
                try
                {
                    this.EventTab[Name][i](Arg);
                }
                catch (System.Exception e)
                {
                    Debug.Log(e);
                }
            }
        }
    }
}