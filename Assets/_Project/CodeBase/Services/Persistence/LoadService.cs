using _Project.CodeBase.Features.BalanceFeature.Components;
using _Project.CodeBase.Features.BusinessFeature.Components;
using _Project.CodeBase.Features.IncomeFeature.Components;
using _Project.CodeBase.Features.UpgradesFeature.Components;
using _Project.CodeBase.Persistence;
using _Project.CodeBase.Services.Player;
using _Project.CodeBase.Shared;
using Leopotam.EcsLite;
using Newtonsoft.Json;
using UnityEngine;

namespace _Project.CodeBase.Services.Persistence
{
    public class LoadService
    {
        private const string SAVE_KEY = "save";

        private readonly IPlayerService _playerService;
        private readonly BusinessService _businessService;
        private readonly UpgradeService _upgradeService;

        public LoadService(
            IPlayerService playerService,
            BusinessService businessService,
            UpgradeService upgradeService)
        {
            _playerService = playerService;
            _businessService = businessService;
            _upgradeService = upgradeService;
        }

        public void ApplySave(EcsWorld world)
        {
            var gameSaveData = LoadGameSaveData();

            if (gameSaveData == null)
            {
                return;
            }

            ApplyBalance(world, gameSaveData);
            ApplyBusinesses(world, gameSaveData);
        }

        private GameSaveData LoadGameSaveData()
        {
            if (!PlayerPrefs.HasKey(SAVE_KEY))
            {
                return null;
            }

            var json = PlayerPrefs.GetString(SAVE_KEY);
            return JsonConvert.DeserializeObject<GameSaveData>(json);
        }

        private void ApplyBalance(EcsWorld world, GameSaveData gameSaveData)
        {
            var playerEntity = _playerService.PlayerEntity;

            ref var balance = ref world.GetPool<Balance>().Get(playerEntity);
            balance.Value = gameSaveData.Balance;

            world.GetPool<BalanceDirty>().SetDirty(playerEntity);
        }

        private void ApplyBusinesses(EcsWorld world, GameSaveData gameSaveData)
        {
            var businessPool = world.GetPool<Business>();
            var ownedBusinessPool = world.GetPool<OwnedBusiness>();
            var incomePool = world.GetPool<IncomeTimer>();
            var ownedUpgradePool = world.GetPool<OwnedUpgrade>();

            foreach (var data in gameSaveData.Businesses)
            {
                if (!_businessService.TryGetEntity(data.Id, out var businessEntity))
                {
                    continue;
                }

                ref var business = ref businessPool.Get(businessEntity);
                business.Level = data.Level;

                if (!ownedBusinessPool.Has(businessEntity))
                {
                    ownedBusinessPool.Add(businessEntity);
                }

                if (incomePool.Has(businessEntity))
                {
                    ref var income = ref incomePool.Get(businessEntity);
                    income.Time = data.IncomeProgress;
                }

                foreach (var upgradeId in data.PurchasedUpgradeIds)
                {
                    if (!_upgradeService.TryGetEntity(upgradeId, out var upgradeEntity))
                    {
                        continue;
                    }

                    if (!ownedUpgradePool.Has(upgradeEntity))
                    {
                        ownedUpgradePool.Add(upgradeEntity);
                    }
                }
            }
        }
    }
}
