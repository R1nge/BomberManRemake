using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Misc;
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

        private void Awake() => _saveManager.OnSaveLoaded += SaveLoaded;

        private void SaveLoaded() => SelectSkin(selectedSkin);

        public bool SkinUnlocked(int index) => _skinData[index].Unlocked;

        public bool UnlockSkin(int index)
        {
            if (_wallet.Spend(skins[index].Price).Result)
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
        }

        public SkinSo GetSkinSo(int index) => skins[index];

        public int SkinsAmount => skins.Length;
        public int SelectedSkinIndex => selectedSkin;

        public NetworkObject GetLobby(int index) => skins[index].LobbyPrefab;
        public NetworkObject GetSkinFPS(int index) => skins[index].PrefabFPS;
        public NetworkObject GetSkinTPS(int index) => skins[index].PrefabTPS;
        public NetworkObject GetEndGame(int index) => skins[index].EndGamePrefab;


        public async Task Save()
        {
            await SaveSkins();
            SaveSelectedSkin();
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
            var loaded = false;

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
                    loaded = true;
                }
                else
                {
                    Debug.Log($"Save not found {name}");
                    for (int i = 0; i < skins.Length; i++)
                    {
                        if (i == 0)
                        {
                            _skinData[i] = new SkinData(i, true);
                        }
                        else
                        {
                            _skinData[i] = new SkinData(i, false);
                        }
                    }

                    loaded = true;
                }
            }, error =>
            {
                Debug.LogError($"Load save error: {error.GenerateErrorReport()}");
                loaded = true;
            });

            await UniTask.WaitUntil(() => loaded);
        }

        private async Task LoadSelectedSkin()
        {
            var keys = new List<string> { SELECTED_SKIN };
            var loaded = false;

            var request = new GetUserDataRequest
            {
                PlayFabId = _playFabManager.GetUserID, Keys = keys
            };

            PlayFabClientAPI.GetUserData(request, result =>
            {
                if (result.Data.ContainsKey(SELECTED_SKIN))
                {
                    var data = result.Data[SELECTED_SKIN].Value;
                    SelectSkin(int.Parse(data));
                    Debug.Log("Loaded last skin index");
                    loaded = true;
                }
                else
                {
                    Debug.Log($"Save not found {SELECTED_SKIN}");
                    loaded = true;
                }
            }, error =>
            {
                Debug.LogError($"Load save error: {error.GenerateErrorReport()}");
                loaded = true;
            });

            await UniTask.WaitUntil(() => loaded);
        }

        private async Task SaveSkins()
        {
            var saved = false;
            for (int i = 0; i < skins.Length; i++)
            {
                var skinTitle = skins[i].Title;
                var skinUnlocked = _skinData[i].Unlocked;
                var data = new SkinData(i, skinUnlocked);
                var dataJson = JsonUtility.ToJson(data);
                await _saveManager.Save(skinTitle, dataJson);
            }

            saved = true;

            await UniTask.WaitUntil(() => saved);
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