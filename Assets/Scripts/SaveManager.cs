using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
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

        for (int i = 0; i < _savables.Count; i++)
        {
            await _savables[i].Load();
            Debug.Log($"SAVE MANAGER LOADED {i}");
        }

        OnSaveLoaded?.Invoke();
    }

    public void Save(string name, string value)
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

        PlayFabClientAPI.UpdateUserData(request, OnSaveSuccess, OnSaveError);
    }

    private void OnSaveSuccess(UpdateUserDataResult data)
    {
        Debug.Log($"Saved {data.Request.ToJson()}");
    }

    private void OnSaveError(PlayFabError error)
    {
        Debug.LogError($"Error while saving {error.Error}; Message: {error.ErrorMessage}");
    }
}