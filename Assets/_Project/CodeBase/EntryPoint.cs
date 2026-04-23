using _Project.CodeBase.Configs;
using _Project.CodeBase.Features.BalanceFeature;
using _Project.CodeBase.Features.BootstrapFeature;
using _Project.CodeBase.Features.BusinessFeature;
using _Project.CodeBase.Features.IncomeFeature;
using _Project.CodeBase.Infrastructure;
using _Project.CodeBase.Services;
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
        [SerializeField] private BalanceView _balanceView;
        [SerializeField] private BusinessesView _businessesView;
        [SerializeField] private BusinessCardView _businessCardViewPrefab;

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

            var commandWriter = new EcsCommandWriter(_world);
            var cardFactory = new BusinessCardFactory(_businessCardViewPrefab);

            var businessListProvider = new BusinessListProvider();
            var balanceProvider = new BalanceProvider();

            _balancePresenter = new BalancePresenter(_balanceView, balanceProvider);
            _businessesPresenter = new BusinessesPresenter(_businessesView, cardFactory, commandWriter, businessListProvider);

            _systems = new EcsSystems(_world);
            _systems
                .Add(new BootstrapSystem(configService))
                .Add(new IncomeTimerSystem())
                .Add(new IncomeSystem(incomeService))
                .Add(new BusinessLevelUpSystem(levelUpService))
                .Add(new BalanceViewSystem(balanceProvider))
                .Add(new BusinessViewSystem(businessListProvider, configService, incomeService, levelUpService));

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
