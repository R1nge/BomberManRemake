using System;
using System.Collections.Generic;
using Misc;
using PlayFab;
using PlayFab.ClientModels;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Skins.Players
{
    public class SkinManager : MonoBehaviour
    {
        [SerializeField] private SkinSo[] skins;
        private SkinData[] _skinData;
        private int selectedSelectedSkin;
        private Wallet _wallet;
        private PlayFabManager _playFabManager;
        private SaveManager _saveManager;
        private const string SELECTED_SKIN = "SelectedSkin";

        [Inject]
        private void Inject(Wallet wallet, PlayFabManager playFabManager, SaveManager saveManager)
        {
            _wallet = wallet;
            _playFabManager = playFabManager;
            _saveManager = saveManager;
        }

        private void Awake()
        {
            _playFabManager.OnLoginSuccessful += PlayFabManagerOnOnLoginSuccessful;
        }

        private void PlayFabManagerOnOnLoginSuccessful()
        {
            Load();
            //UnlockSkin(0);
            //Save();
        }

        public bool SkinUnlocked(int index) => _skinData[index].Unlocked;

        public bool UnlockSkin(int index)
        {
            if (_wallet.Spend(skins[index].Price))
            {
                print("Skin unlocked");
                _skinData[index].Unlock();
                return true;
            }

            print($"Not enough money to unlock this skin {skins[index].Title}");

            return false;
        }

        public void SelectSkin(int index) => selectedSelectedSkin = index;

        public SkinSo GetSkinSo(int index) => skins[index];

        public int SkinsAmount => skins.Length;
        public int SelectedSkinIndex => selectedSelectedSkin;

        public NetworkObject GetLobby(int index) => skins[index].LobbyPrefab;
        public NetworkObject GetSkinFPS(int index) => skins[index].PrefabFPS;
        public NetworkObject GetSkinTPS(int index) => skins[index].PrefabTPS;
        public NetworkObject GetEndGame(int index) => skins[index].EndGamePrefab;


        private void Load()
        {
            _skinData = new SkinData[skins.Length];
            
            print($"LOAD {skins.Length}");

            selectedSelectedSkin = _saveManager.LoadInt(SELECTED_SKIN);
            
            for (int i = 0; i < skins.Length; i++)
            {
                LoadSkinData(skins[i].Title, i);
            }
        }
        
        private void LoadSkinData(string name, int index)
        {
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
                    var skinData = JsonUtility.FromJson<SkinData>(data);
                    _skinData[index] = skinData;
                    Debug.Log($"Loaded save {name} SKIN DATA {data} SKIN DATA JSON {skinData.Unlocked}");
                }
                else
                {
                    Debug.Log($"Save not found {name}");
                }
            }, error =>
            {
                Debug.LogError($"Load save error: {error.GenerateErrorReport()}");
            });
        }

        private void SaveSkins()
        {
            for (int i = 0; i < skins.Length; i++)
            {
                var skinTitle = skins[i].Title;
                var skinUnlocked = _skinData[i].Unlocked;
                var data = new SkinData(i, skinUnlocked);
                var dataJson = JsonUtility.ToJson(data);
                _saveManager.Save(skinTitle, dataJson);
            }
        }

        private void SaveSelectedSkin()
        {
            _saveManager.Save(SELECTED_SKIN, selectedSelectedSkin.ToString());
        }

        private void OnApplicationQuit()
        {
            SaveSelectedSkin();
            SaveSkins();
        }

        private void OnDestroy() => _playFabManager.OnLoginSuccessful -= PlayFabManagerOnOnLoginSuccessful;
    }
}