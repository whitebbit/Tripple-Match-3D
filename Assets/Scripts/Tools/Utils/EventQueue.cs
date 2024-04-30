using System;
using System.Collections.Generic;
using UnityEngine;

public class MyEvent
{
    public event Action onStart, onComplete;

    public MyEvent() {}

    public MyEvent(Action onStart, Action onComplete = null)
    {
        this.onStart = onStart;
        this.onComplete = onComplete;
    }

    public MyEvent OnComplete(Action action)
    {
        onComplete = action;
        return this;
    }

    public MyEvent OnStart(Action action)
    {
        onStart = action;
        return this;
    }

    public void Start() => onStart?.Invoke();
    public void Complete() => onComplete?.Invoke();
}

public class EventQueue
{
    Queue<MyEvent> events;
    public int Count => events.Count;
    public event Action onStart, onComplete;

    public EventQueue()
    {
        events = new();
    }

    public EventQueue(Action onStart, Action onComplete = null)
    {
        events = new();
        this.onStart = onStart;
        this.onComplete = onComplete;
    }

    public void Add(MyEvent myEvent)
    {
        myEvent.onComplete += CompleteAction;
        events.Enqueue(myEvent);
        if (events.Count == 1)
        {
            onStart?.Invoke();
            myEvent.Start();
        }
    }

    void CompleteAction()
    {
        if (events.Count == 0) return;
        events.Dequeue().onComplete -= CompleteAction;
        if (events.TryPeek(out MyEvent nextEvent)) nextEvent.Start();
        else onComplete?.Invoke();
    }
}