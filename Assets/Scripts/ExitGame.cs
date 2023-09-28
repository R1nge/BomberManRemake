using System;
using UnityEngine;

public class ExitGame
{
    public event Action OnGameExit;
    private static bool _canExit;

    public void Exit()
    {
        OnGameExit?.Invoke();
        _canExit = true;
    }
    
    [RuntimeInitializeOnLoadMethod]
    static void RunOnStart()
    {
        Application.wantsToQuit += ApplicationOnwantsToQuit;
    }

    private static bool ApplicationOnwantsToQuit()
    {
        return _canExit;
    }
}