using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Misc;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Skins.Players
{
    public class SkinManager : MonoBehaviour, ISavable
    {
        public event Action<int, int> OnSkinChanged;
        [SerializeField] private SkinSo[] skins;
        private SkinData[] _skinData;
        private int selectedSkin;
        private Wallet _wallet;
        private SaveManager _saveManager;
        private PlayFabManager _playFabManager;
        private const string SELECTED_SKIN = "SelectedSkin";

        [Inject]
        private void Inject(Wallet wallet, SaveManager saveManager, PlayFabManager playFabManager)
        {
            _wallet = wallet;
            _saveManager = saveManager;
            _playFabManager = playFabManager;
        }


        public int SkinsAmount => skins.Length;
        public int SelectedSkinIndex => selectedSkin;

        public NetworkObject GetLobby(int index) => skins[index].LobbyPrefab;
        public NetworkObject GetSkinFPS(int index) => skins[index].PrefabFPS;
        public NetworkObject GetSkinTPS(int index) => skins[index].PrefabTPS;
        public NetworkObject GetEndGame(int index) => skins[index].EndGamePrefab;


        public async Task Save()
        {
            SaveSelectedSkin();
            await SaveSkins();
        }

        private void Awake() => _saveManager.OnSaveLoaded += SaveLoaded;

        private void SaveLoaded() => SelectSkin(selectedSkin);

        public bool SkinUnlocked(int index) => _skinData[index].Unlocked;

        public async Task<bool> UnlockSkin(int index)
        {
            var spend = await _wallet.Spend(_skinData[index].Price);
            
            if (spend)
            {
                print("Skin unlocked");
                _skinData[index].Unlock();
                return true;
            }

            print($"Not enough money to unlock this skin {skins[index].Title}");

            return false;
        }

        public void SelectSkin(int index)
        {
            OnSkinChanged?.Invoke(selectedSkin, index);
            selectedSkin = index;
            SaveSkins();
        }

        public SkinSo GetSkinSo(int index) => skins[index];

        public async Task<SkinData> GetSkinData(int index)
        {
            await Load();
            return _skinData[index];
        }

        public async Task Load()
        {
            _skinData = new SkinData[skins.Length];

            print($"LOAD SKINS {skins.Length}");

            for (int i = 0; i < skins.Length; i++)
            {
                await LoadSkinData(skins[i].Title, i);
            }

            await LoadSelectedSkin();
        }

        private async Task LoadSkinData(string name, int index)
        {
            var keys = new List<string> { name };

            var request = new GetUserDataRequest
            {
                PlayFabId = _playFabManager.GetUserID, Keys = keys
            };

            var loadSkinDataTask = PlayFabClientAPI.GetUserDataAsync(request);

            await loadSkinDataTask;

            print($"SKIN DATA LOADED: {loadSkinDataTask.Result.Result.Data[name].Value}");
            var skinData = JsonConvert.DeserializeObject<SkinData>(loadSkinDataTask.Result.Result.Data[name].Value);

            _skinData[index] = skinData;

            //if save not found   LoadSkinPrices();
        }

        private int _loadSkinIndex = 0;

        private async void LoadSkinPrices()
        {
            for (_loadSkinIndex = 0; _loadSkinIndex < skins.Length; _loadSkinIndex++)
            {
                if (_loadSkinIndex == 0)
                {
                    _skinData[_loadSkinIndex] = new SkinData(true, 0);
                }
                else
                {
                    //load skins
                    // if (data.Data == null || !data.Data.ContainsKey("Prices"))
                    // {
                    //     Debug.LogError("No such skin price");
                    // }
                    //
                    // var prices = new int[_skinData.Length];
                    //
                    // prices[_loadSkinIndex] = JsonUtility.FromJson<RequestData>(data.Data.ToString()).Price;
                    //
                    // _skinData[_loadSkinIndex] = new SkinData(false, prices[_loadSkinIndex]);
                    // print($"PRICE: {prices[_loadSkinIndex]}");
                }
            }
        }
        
        private async Task LoadSelectedSkin()
        {
            var keys = new List<string> { SELECTED_SKIN };

            var request = new GetUserDataRequest
            {
                PlayFabId = _playFabManager.GetUserID, Keys = keys
            };

            var loadSelectedSkinTask = PlayFabClientAPI.GetUserDataAsync(request);

            await loadSelectedSkinTask;

            var value = loadSelectedSkinTask.Result.Result.Data[SELECTED_SKIN].Value;
            
            SelectSkin(int.Parse(value));
            
            print($"SELECTED SKIN DATA: {value}");
        }

        private async Task SaveSkins()
        {
            for (int i = 0; i < skins.Length; i++)
            {
                var dataJson = JsonUtility.ToJson(_skinData[i]);
                await _saveManager.Save(skins[i].Title, dataJson);
            }
        }

        private async void SaveSelectedSkin() => await _saveManager.Save(SELECTED_SKIN, selectedSkin.ToString());

        private async void OnApplicationQuit()
        {
            SaveSelectedSkin();
            await SaveSkins();
        }

        private void OnDestroy() => _playFabManager.OnLoginSuccessful -= SaveLoaded;
    }
}