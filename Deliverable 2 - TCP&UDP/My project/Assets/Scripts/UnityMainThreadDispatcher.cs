using System;
using System.Collections.Concurrent;
using UnityEngine;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static readonly ConcurrentQueue<Action> actions = new ConcurrentQueue<Action>();
    private static UnityMainThreadDispatcher instance;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        while (actions.TryDequeue(out var action))
        {
            try { action?.Invoke(); }
            catch (Exception ex) { Debug.LogError("Dispatcher error: " + ex); }
        }
    }

    public static void Enqueue(Action action)
    {
        if (action == null) return;
        actions.Enqueue(action);
    }
}
