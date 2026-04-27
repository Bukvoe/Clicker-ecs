using System.Collections.Generic;
using System.Linq;
using _Project.CodeBase.Features.BalanceFeature.Components;
using _Project.CodeBase.Features.BusinessFeature.Components;
using _Project.CodeBase.Features.IncomeFeature.Components;
using _Project.CodeBase.Features.UpgradesFeature.Queries;
using _Project.CodeBase.Persistence;
using _Project.CodeBase.Services.Player;
using Leopotam.EcsLite;
using Newtonsoft.Json;
using UnityEngine;

namespace _Project.CodeBase.Services.Persistence
{
    public class SaveService
    {
        private const string SAVE_KEY = "save";

        private readonly IPlayerService _playerService;

        public SaveService(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        public void Save(EcsWorld world)
        {
            var gameSaveData = CreateGameSaveData(world);

            Save(gameSaveData);
        }

        private GameSaveData CreateGameSaveData(EcsWorld world)
        {
            var playerEntity = _playerService.PlayerEntity;

            var balance = world.GetPool<Balance>().Get(playerEntity);

            var ownedBusinessFilter = world.Filter<Business>()
                .Inc<OwnedBusiness>()
                .Inc<IncomeTimer>()
                .End();

            var upgradeQuery = new UpgradeQuery(world);
            var businesses = new List<BusinessSaveData>();

            foreach (var businessEntity in ownedBusinessFilter)
            {
                var business = world.GetPool<Business>().Get(businessEntity);
                var income = world.GetPool<IncomeTimer>().Get(businessEntity);
                var upgrades = upgradeQuery.GetOwnedUpgradeIds(business.Id);

                businesses.Add(new BusinessSaveData
                {
                    Id = business.Id,
                    Level = business.Level,
                    IncomeProgress = income.Time,
                    PurchasedUpgradeIds = upgrades.ToList(),
                });
            }

            return new GameSaveData
            {
                Balance = balance.Value,
                Businesses = businesses,
            };
        }

        private void Save(GameSaveData gameSaveData)
        {
            var json = JsonConvert.SerializeObject(gameSaveData);
            PlayerPrefs.SetString(SAVE_KEY, json);
            PlayerPrefs.Save();
        }
    }
}
