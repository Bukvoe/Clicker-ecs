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
using _Project.CodeBase.Services.Persistence;
using _Project.CodeBase.Services.Player;
using _Project.CodeBase.UI.Balance;
using _Project.CodeBase.UI.Businesses;
using _Project.CodeBase.UI.Factories;
using Leopotam.EcsLite;
using UnityEngine;

namespace _Project.CodeBase
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private GameConfig _gameConfig;
        [SerializeField] private NameConfig _nameConfig;
        [SerializeField] private BalanceView _balanceView;
        [SerializeField] private BusinessesView _businessesView;
        [SerializeField] private BusinessCardView _businessCardViewPrefab;

        private EcsWorld _world;
        private EcsSystems _systems;

        private SaveService _saveService;

        private BalancePresenter _balancePresenter;
        private BusinessesPresenter _businessesPresenter;

        private void Awake()
        {
            _world = new EcsWorld();

            var configService = new ConfigService(_gameConfig, _nameConfig);
            var playerService = new PlayerService();
            var businessService = new BusinessService();
            var upgradeService = new UpgradeService();
            var levelUpService = new LevelUpService(configService);
            var incomeService = new IncomeService(configService);

            var loadService = new LoadService(playerService, businessService, upgradeService);
            _saveService = new SaveService(playerService);

            var requestWriter = new EcsRequestWriter(_world);

            var cardFactory = new BusinessCardFactory(_businessCardViewPrefab);

            var businessViewProvider = new BusinessViewProvider();
            var balanceProvider = new BalanceViewProvider();

            _balancePresenter = new BalancePresenter(_balanceView, balanceProvider);
            _businessesPresenter = new BusinessesPresenter(_businessesView, businessViewProvider, cardFactory, requestWriter, configService);

            _systems = new EcsSystems(_world);

            _systems
                .Add(new BootstrapSystem(configService, loadService, playerService, businessService, upgradeService))
                .Add(new IncomeTimerSystem())
                .Add(new IncomeSystem(incomeService, playerService))
                .Add(new BusinessLevelUpSystem(playerService, levelUpService, businessService))
                .Add(new BuyUpgradeSystem(configService, playerService, businessService, upgradeService))
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

        private void OnApplicationPause(bool pause)
        {
            if (!pause)
            {
                return;
            }

            _saveService.Save(_world);
        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                return;
            }

            _saveService.Save(_world);
        }

        private void OnApplicationQuit()
        {
            _saveService.Save(_world);
        }
    }
}
