using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private static EventManager instance;

    private Dictionary<Type, List<Action<BasicEvent>>> subscribers;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;

        subscribers = new Dictionary<Type, List<Action<BasicEvent>>>();
    }

    public static void BindEvent(Type eventType, Action<BasicEvent> func)
    {
        if (instance.subscribers.ContainsKey(eventType))
        {
            instance.subscribers[eventType].Add(func);
        }
        else
        {
            instance.subscribers[eventType] = new List<Action<BasicEvent>>();
            instance.subscribers[eventType].Add(func);
        }
    }

    public static void UnbindEvent(Type eventType, Action<BasicEvent> func)
    {
        if (instance.subscribers.ContainsKey(eventType))
        {
            instance.subscribers[eventType].Remove(func);
        }
    }

    public static void InvokeEvent(BasicEvent ev)
    {
        if (instance.subscribers.ContainsKey(ev.GetType()))
        {
            foreach (Action<BasicEvent> action in instance.subscribers[ev.GetType()])
            {
                action.Invoke(ev);
            }
        }
    }
}
