using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            SaveSelectedSkin();
        }

        public SkinSo GetSkinSo(int index) => skins[index];

        public async Task<SkinData> GetSkinDataFromServer(int index)
        {
            await Load();
            return _skinData[index];
        }

        public SkinData GetSkinData(int index) => _skinData[index];

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
            #region Unlocked

            var keys = new List<string> { name };

            var request = new GetUserDataRequest
            {
                PlayFabId = _playFabManager.GetUserID, Keys = keys
            };

            var loadSkinDataTask = PlayFabClientAPI.GetUserDataAsync(request);
            await loadSkinDataTask;

            var skin = loadSkinDataTask.Result.Result.Data[name].Value;

            var unlocked = JsonConvert.DeserializeObject<SkinData>(skin);

            #endregion

            #region Price

            name += "Price";

            keys = new List<string> { name };

            var request2 = new GetTitleDataRequest
            {
                Keys = keys,
            };

            var loadSkinDataTask2 = PlayFabClientAPI.GetTitleDataAsync(request2);

            await loadSkinDataTask2;

            var skinPrice = JsonConvert.DeserializeObject<int>(loadSkinDataTask2.Result.Result.Data[name]);

            #endregion

            var data = new SkinData(unlocked.Unlocked, skinPrice);

            _skinData[index] = data;
            
            //if save not found   LoadSkinPrices();
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
            var skinPrice = PlayFabClientAPI.GetTitleDataAsync(new GetTitleDataRequest());

            await skinPrice;

            for (int i = 0; i < skins.Length; i++)
            {
                var status = i == 0;
                var key = $"{skins[i].Title}Price";
                var priceString = skinPrice.Result.Result.Data[key];
                var price = int.Parse(priceString);
                _skinData[i] = new SkinData(status, price);
            }

            for (int i = 0; i < skins.Length; i++)
            {
                var dataJson = JsonUtility.ToJson(_skinData[i]);
                //Debug.LogError(dataJson);
                await _saveManager.Save(skins[i].Title, dataJson);
            }
        }

        private async void SaveSelectedSkin() => await _saveManager.Save(SELECTED_SKIN, selectedSkin.ToString());

        private async void OnApplicationQuit() => await Save();

        private void OnDestroy() => _playFabManager.OnLoginSuccessful -= SaveLoaded;
    }
}