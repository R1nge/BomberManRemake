using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using Zenject;

namespace Misc
{
    public class Wallet : IInitializable, IDisposable
    {
        private int _money;
        private const string MoneyString = "Money";
        private bool _firstLaunch = true;
        private PlayFabManager _playFabManager;

        [Inject]
        private void Inject(PlayFabManager playFabManager) => _playFabManager = playFabManager;

        public void Initialize() => _playFabManager.OnLoginSuccessful += Load;

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

        public bool Spend(int amount)
        {
            if (amount < 0)
            {
                Debug.LogWarning("Trying to spend negative value");
                return false;
            }

            if (Money - amount >= 0)
            {
                Money -= amount;
                Save();
                return true;
            }

            Debug.LogWarning("Not enough money");
            
            return false;
        }

        private void Load()
        {
            var keys = new List<string> { MoneyString };

            var request = new GetUserDataRequest
            {
                PlayFabId = _playFabManager.GetUserID, Keys = keys
            };

            PlayFabClientAPI.GetUserData(request, result =>
            {
                if (result.Data.ContainsKey(MoneyString))
                {
                    var moneyAmount = result.Data[MoneyString].Value;
                    Money = int.Parse(moneyAmount);
                    Debug.Log($"Wallet: Loaded save. Money: {Money}");
                }
                else
                {
                    Debug.Log("Wallet: Save not found");
                }

                _firstLaunch = false;
            }, error =>
            {
                Debug.LogError($"Wallet {error.GenerateErrorReport()}");
            });
        }

        private void Save()
        {
            if (_firstLaunch) return;
            
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
        }

        private void OnSaveSuccess(UpdateUserDataResult result)
        {
            Debug.Log($"Wallet: Saved money: {Money}");
        }

        private void OnSaveError(PlayFabError error)
        {
            Debug.LogError($"Wallet: Error while saving money {error.GenerateErrorReport()}");
        }

        public void Dispose() => _playFabManager.OnLoginSuccessful -= Load;
    }
}