using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using Zenject;

namespace Misc
{
    public class Wallet : ISavable
    {
        private int _money;
        private const string MoneyString = "Money";
        private bool _firstLaunch = true;
        private PlayFabManager _playFabManager;

        [Inject]
        private void Inject(PlayFabManager playFabManager)
        {
            _playFabManager = playFabManager;
        }

        public int Money
        {
            get => _money;
            private set
            {
                _money = value;
                OnMoneyAmountChanged?.Invoke(_money);
            }
        }

        public event Action<int> OnMoneyAmountChanged;

        public void Earn(int amount)
        {
            if (amount < 0)
            {
                Debug.LogWarning("Trying to earn negative value");
                return;
            }

            Money += amount;
        }

        public async Task<bool> Spend(int amount)
        {
            if (amount < 0)
            {
                Debug.LogWarning("Trying to spend negative value");
                return false;
            }

            await GetMoneyFromServer();

            if (Money - amount >= 0)
            {
                Money -= amount;
                Save();
                return true;
            }

            Debug.LogWarning("Not enough money");

            return false;
        }

        private async Task GetMoneyFromServer()
        {
            var keys = new List<string> { MoneyString };
            var loaded = false;

            var request = new GetUserDataRequest
            {
                PlayFabId = _playFabManager.GetUserID, Keys = keys
            };

            PlayFabClientAPI.GetUserData(request, result =>
            {
                if (result.Data.ContainsKey(MoneyString))
                {
                    var data = result.Data[MoneyString].Value;
                    Money = int.Parse(data);
                    loaded = true;
                    Debug.Log($"Loaded save {MoneyString}");
                }
                else
                {
                    Debug.Log($"Save not found {MoneyString}");
                    loaded = true;
                }
            }, error =>
            {
                Debug.LogError($"Save error: {error.GenerateErrorReport()}");
                loaded = true;
            });

            await UniTask.WaitUntil(() => loaded);
        }

        public async Task Save()
        {
            if (_firstLaunch) return;
            var loaded = false;

            var request = new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>(1)
            };

            if (request.Data.TryGetValue(MoneyString, out _))
            {
                request.Data[MoneyString] = Money.ToString();
            }
            else
            {
                request.Data.Add(MoneyString, Money.ToString());
            }

            PlayFabClientAPI.UpdateUserData(request, OnSaveSuccess, OnSaveError);
            loaded = true;

            await UniTask.WaitUntil(() => loaded);
        }

        private void OnSaveSuccess(UpdateUserDataResult result)
        {
            Debug.Log($"Wallet: Saved money: {Money}");
        }

        private void OnSaveError(PlayFabError error)
        {
            Debug.LogError($"Wallet: Error while saving money {error.GenerateErrorReport()}");
        }

        public async Task Load()
        {
            await GetMoneyFromServer();

            _firstLaunch = false;
        }
    }
}