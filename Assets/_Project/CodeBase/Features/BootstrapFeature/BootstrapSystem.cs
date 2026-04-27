using System.Linq;
using System.Numerics;
using _Project.CodeBase.Configs;
using _Project.CodeBase.Features.BalanceFeature.Components;
using _Project.CodeBase.Features.BusinessFeature.Components;
using _Project.CodeBase.Features.IncomeFeature.Components;
using _Project.CodeBase.Features.PlayerFeature;
using _Project.CodeBase.Features.UpgradesFeature.Components;
using _Project.CodeBase.Services;
using _Project.CodeBase.Services.Player;
using _Project.CodeBase.Shared;
using Leopotam.EcsLite;

namespace _Project.CodeBase.Features.BootstrapFeature
{
    internal class BootstrapSystem : IEcsInitSystem
    {
        private readonly ConfigService _configService;
        private readonly BusinessService _businessService;
        private readonly UpgradeService _upgradeService;
        private readonly PlayerService _playerService;

        public BootstrapSystem(
            ConfigService configService,
            BusinessService businessService,
            UpgradeService upgradeService,
            PlayerService playerService)
        {
            _configService = configService;
            _businessService = businessService;
            _upgradeService = upgradeService;
            _playerService = playerService;
        }

        public void Init(EcsSystems systems)
        {
            var world = systems.GetWorld();

            CreatePlayer(world);

            foreach (var businessDefinition in _configService.AllBusinesses)
            {
                CreateBusiness(world, businessDefinition.Id);
                CreateUpgrades(world, businessDefinition);
            }
        }

        private void CreatePlayer(EcsWorld world)
        {
            var playerEntity = world.NewEntity();

            world.GetPool<Player>().Add(playerEntity);

            ref var balance = ref world.GetPool<Balance>().Add(playerEntity);
            balance.Value = BigInteger.Zero;

            _playerService.SetPlayerEntity(playerEntity);
        }

        private void CreateBusiness(EcsWorld world, int businessId)
        {
            var businessEntity = world.NewEntity();

            ref var business = ref world.GetPool<Business>().Add(businessEntity);
            business.Id = businessId;
            business.Level = 0;

            world.GetPool<IncomeTimer>().Add(businessEntity);
            world.GetPool<BusinessDirty>().SetDirty(businessEntity);

            if (IsStarterBusiness(businessId))
            {
                world.GetPool<OwnedBusiness>().Add(businessEntity);
                business.Level = 1;
            }

            _businessService.Register(businessId, businessEntity);
        }

        private bool IsStarterBusiness(int businessId)
        {
            return _configService.BusinessIdsOnStart.Contains(businessId);
        }

        private void CreateUpgrades(EcsWorld world, BusinessDefinition businessDefinition)
        {
            var pool = world.GetPool<Upgrade>();
            var upgradeIds = businessDefinition.UpgradeIds;

            foreach (var upgradeId in upgradeIds)
            {
                if (!_configService.TryGetUpgrade(upgradeId, out _))
                {
                    continue;
                }

                var entity = world.NewEntity();

                ref var upgrade = ref pool.Add(entity);
                upgrade.BusinessId = businessDefinition.Id;
                upgrade.UpgradeId = upgradeId;

                _upgradeService.Register(upgradeId, entity);
            }
        }
    }
}
