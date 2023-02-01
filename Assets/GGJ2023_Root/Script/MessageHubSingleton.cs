using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Easy.MessageHub;

public class MessageHubSingleton : MonoBehaviour
{
    public static MessageHubSingleton Instance;

    IMessageHub _messageHub = new MessageHub();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }

        Instance = this;
        DontDestroyOnLoad(this);
    }

    public void Publish<T>(T eventClass)
    {
        _messageHub.Publish<T>(eventClass);
    }

    public Guid Subscribe<T>(Action<T> action)
    {
        return _messageHub.Subscribe<T>(action);
    }

    public void Unsubscribe(Guid guid)
    {
        _messageHub.Unsubscribe(guid);
    }

    public void Unsubscribe(List<Guid> tokenList)
    {
        foreach (Guid token in tokenList)
        {
            Unsubscribe(token);
        }
    }
}