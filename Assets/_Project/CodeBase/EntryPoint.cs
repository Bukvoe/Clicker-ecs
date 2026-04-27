using _Project.CodeBase.Configs;
using _Project.CodeBase.Features.BalanceFeature.Systems;
using _Project.CodeBase.Features.BalanceFeature.View;
using _Project.CodeBase.Features.BootstrapFeature;
using _Project.CodeBase.Features.BusinessFeature.Systems;
using _Project.CodeBase.Features.BusinessFeature.View;
using _Project.CodeBase.Features.IncomeFeature.Systems;
using _Project.CodeBase.Features.UpgradesFeature.Systems;
using _Project.CodeBase.Infrastructure;
using _Project.CodeBase.Services;
using _Project.CodeBase.Services.Player;
using _Project.CodeBase.UI.Balance;
using _Project.CodeBase.UI.Businesses;
using _Project.CodeBase.UI.Factories;
using _Project.CodeBase.UI.Upgrades;
using Leopotam.EcsLite;
using UnityEngine;

namespace _Project.CodeBase
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private GameConfig _gameConfig;
        [SerializeField] private BalanceView _balanceView;
        [SerializeField] private BusinessesView _businessesView;
        [SerializeField] private BusinessCardView _businessCardViewPrefab;
        [SerializeField] private UpgradeButton _upgradeButtonPrefab;

        private EcsWorld _world;
        private EcsSystems _systems;

        private BalancePresenter _balancePresenter;
        private BusinessesPresenter _businessesPresenter;

        private void Awake()
        {
            _world = new EcsWorld();

            var configService = new ConfigService(_gameConfig);
            var incomeService = new IncomeService(configService);
            var levelUpService = new LevelUpService(configService);
            var businessService = new BusinessService();
            var upgradeService = new UpgradeService();
            var playerService = new PlayerService();

            var requestWriter = new EcsRequestWriter(_world);
            var cardFactory = new BusinessCardFactory(_businessCardViewPrefab);

            var businessViewProvider = new BusinessViewProvider();
            var balanceProvider = new BalanceViewProvider();

            _balancePresenter = new BalancePresenter(_balanceView, balanceProvider);
            _businessesPresenter = new BusinessesPresenter(_businessesView, cardFactory, requestWriter, businessViewProvider, configService);

            _systems = new EcsSystems(_world);
            _systems
                .Add(new BootstrapSystem(configService, businessService, upgradeService, playerService))
                .Add(new IncomeTimerSystem())
                .Add(new IncomeSystem(incomeService, playerService))
                .Add(new BusinessLevelUpSystem(levelUpService, businessService, playerService))
                .Add(new BuyUpgradeSystem(configService, businessService, upgradeService, playerService))
                .Add(new BalanceViewSystem(playerService, balanceProvider))
                .Add(new BusinessViewSystem(businessViewProvider, configService, incomeService, levelUpService));

            _systems.Init();
        }

        public void Update()
        {
            _systems.Run();
        }

        public void OnDestroy()
        {
            _balancePresenter.Dispose();
            _businessesPresenter.Dispose();

            _systems.Destroy();
            _world.Destroy();
        }
    }
}
