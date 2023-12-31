﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Misc;
using PlayFab;
using PlayFab.ClientModels;
using Skins.Players;
using UnityEngine;
using Zenject;

public class SaveManager : IInitializable
{
    public event Action OnSaveLoaded;
    private List<ISavable> _savables;
    private PlayFabManager _playFabManager;
    private Wallet _wallet;
    private SkinManager _skinManager;

    [Inject]
    private void Inject(PlayFabManager playFabManager, Wallet wallet, SkinManager skinManager)
    {
        _playFabManager = playFabManager;
        _wallet = wallet;
        _skinManager = skinManager;
    }

    public void Initialize()
    {
        _playFabManager.OnLoginSuccessful += OnLoginSuccessful;
    }

    private async void OnLoginSuccessful()
    {
        _savables = new List<ISavable>
        {
            _wallet,
            _skinManager
        };

        var tasks = new List<UniTask>();

        for (int i = 0; i < _savables.Count; i++)
        {
            tasks.Add(_savables[i].Load());
            Debug.Log($"SAVE MANAGER LOADED {i}");
        }

        await UniTask.WhenAll(tasks);
        
        Debug.Log($"ALL SAVE LOADED");

        OnSaveLoaded?.Invoke();
    }

    public async UniTask Save(string name, string value)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>(1)
        };

        if (request.Data.TryGetValue(name, out _))
        {
            request.Data[name] = value;
        }
        else
        {
            request.Data.Add(name, value);
        }

        var saveTask = PlayFabClientAPI.UpdateUserDataAsync(request);

        await saveTask;
    }
}