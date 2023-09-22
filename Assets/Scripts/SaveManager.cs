using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Misc;
using PlayFab;
using PlayFab.ClientModels;
using Skins.Players;
using UnityEngine;
using Zenject;

public class SaveManager
{
    private PlayFabManager _playFabManager;

    [Inject]
    private void Inject(PlayFabManager playFabManager)
    {
        _playFabManager = playFabManager;
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
        Debug.LogError($"Error while saving {error.Error}");
    }

    public int LoadInt(string name)
    {
        var dataInt = -1;
        var keys = new List<string> { name };

        var request = new GetUserDataRequest
        {
            PlayFabId = _playFabManager.GetUserID, Keys = keys
        };

        PlayFabClientAPI.GetUserData(request, result =>
        {
            if (result.Data.ContainsKey(name))
            {
                var data = result.Data[name].Value;
                dataInt = int.Parse(data);
                Debug.Log($"Loaded save {name}");
            }
            else
            {
                Debug.Log($"Save not found {name}");
            }
        }, error =>
        {
            Debug.LogError($"Save error: {error.GenerateErrorReport()}");
        });

        return dataInt;
    }

    public bool LoadBool(string name)
    {
        var dataBool = false;
        var keys = new List<string> { name };

        var request = new GetUserDataRequest
        {
            PlayFabId = _playFabManager.GetUserID, Keys = keys
        };

        PlayFabClientAPI.GetUserData(request, result =>
        {
            if (result.Data.ContainsKey(name))
            {
                var data = result.Data[name].Value;
                dataBool = bool.Parse(data);
                Debug.Log($"Loaded save {name}");
            }
            else
            {
                Debug.Log($"Save not found {name}");
            }
        }, error =>
        {
            Debug.LogError($"Save error: {error.GenerateErrorReport()}");
        });

        return dataBool;
    }
}