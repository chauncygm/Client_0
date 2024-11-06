
using System;
using UnityEngine;

public class ResourceManager
{
    public static readonly ResourceManager Instance = new ResourceManager();

    private ResourceManager()
    {
    }

    public void Init(Action callback)
    {
        // do something
        var success = loadResource();
        if (success)
        {
            callback();
        }
        else
        {
            Debug.LogError("Load resource failed.");
        }
    }

    private bool loadResource()
    {
        return true;
    }
}
