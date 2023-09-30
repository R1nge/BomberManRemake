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
            await SaveSelectedSkin();
            await SaveSkins();
        }

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

        public async void SelectSkin(int index)
        {
            selectedSkin = index;
            OnSkinChanged?.Invoke(selectedSkin, index);
            await Save();
        }

        public SkinSo GetSkinSo(int index) => skins[index];

        public SkinData GetSkinData(int index) => _skinData[index];

        public async Task Load()
        {
            _skinData = new SkinData[skins.Length];

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


            var skin = String.Empty;
            var unlocked = false;


            try
            {
                skin = loadSkinDataTask.Result.Result.Data[name].Value;
                //TODO: redo if the data changes
                unlocked = skin == "True";
            }
            catch (Exception e)
            {
                if (string.IsNullOrEmpty(skin))
                {
                    unlocked = index == 0;
                }
            }

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

            var data = new SkinData(unlocked, skinPrice);

            _skinData[index] = data;
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


            var value = 0;

            try
            {
                value = int.Parse(loadSelectedSkinTask.Result.Result.Data[SELECTED_SKIN].Value);
            }
            catch (Exception e)
            {
                value = 0;
            }


            SelectSkin(value);
        }

        private async Task SaveSkins()
        {
            for (int i = 0; i < skins.Length; i++)
            {
                var unlocked = _skinData[i].Unlocked;
                await _saveManager.Save(skins[i].Title, unlocked.ToString());
            }
        }

        private async Task SaveSelectedSkin() => await _saveManager.Save(SELECTED_SKIN, selectedSkin.ToString());
    }
}