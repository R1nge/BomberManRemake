using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
                await Save();
                return true;
            }

            Debug.LogWarning("Not enough money");

            return false;
        }

        private async Task GetMoneyFromServer()
        {
            var keys = new List<string> { MoneyString };

            var request = new GetUserDataRequest
            {
                PlayFabId = _playFabManager.GetUserID, Keys = keys
            };

            var getMoneyTask = PlayFabClientAPI.GetUserDataAsync(request);

            await getMoneyTask;

            var money = "";

            try
            {
                money = getMoneyTask.Result.Result.Data[MoneyString].Value;
                
                Money = int.Parse(money);
            }
            catch (Exception e)
            {
                if (string.IsNullOrEmpty(money))
                {
                    var request2 = new UpdateUserDataRequest
                    {
                        Data = new Dictionary<string, string>(1)
                    };

                    if (request2.Data.TryGetValue(MoneyString, out _))
                    {
                        request2.Data[MoneyString] = Money.ToString();
                    }
                    else
                    {
                        request2.Data.Add(MoneyString, Money.ToString());
                    }

                    var updateMoneyTask = PlayFabClientAPI.UpdateUserDataAsync(request2);

                    await updateMoneyTask;
                }
                else
                {
                    Money = int.Parse(money);
                }
            }


            Debug.Log($"Loaded save {MoneyString}");
        }

        public async Task Save()
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

            var updateMoneyTask = PlayFabClientAPI.UpdateUserDataAsync(request);

            await updateMoneyTask;
        }

        public async Task Load()
        {
            await GetMoneyFromServer();

            _firstLaunch = false;
        }
    }
}