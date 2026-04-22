using _Project.CodeBase.Configs;
using _Project.CodeBase.Features.BootstrapFeature;
using _Project.CodeBase.Features.BusinessFeature;
using _Project.CodeBase.Features.IncomeFeature;
using _Project.CodeBase.Infrastructure;
using _Project.CodeBase.Services;
using _Project.CodeBase.UI.Businesses;
using _Project.CodeBase.UI.Factories;
using Leopotam.EcsLite;
using UnityEngine;

namespace _Project.CodeBase
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private GameConfig _gameConfig;
        [SerializeField] private BusinessesView _businessesView;
        [SerializeField] private BusinessCardView _businessCardViewPrefab;

        private EcsWorld _world;
        private EcsSystems _systems;

        private BusinessesPresenter _businessesPresenter;

        private void Awake()
        {
            _world = new EcsWorld();

            var configService = new ConfigService(_gameConfig);
            var incomeService = new IncomeService(configService);

            var commandWriter = new EcsCommandWriter(_world);
            var cardFactory = new BusinessCardFactory(_businessCardViewPrefab);
            var businessListProvider = new BusinessListProvider();

            _businessesPresenter = new BusinessesPresenter(_businessesView, cardFactory, commandWriter, businessListProvider);

            _systems = new EcsSystems(_world);
            _systems
                .Add(new BootstrapSystem(configService))
                .Add(new IncomeSystem(incomeService))
                .Add(new BusinessViewSystem(businessListProvider, configService, incomeService));

            _systems.Init();
        }

        public void Update()
        {
            _systems.Run();
        }

        public void OnDestroy()
        {
            _businessesPresenter.Dispose();

            _systems.Destroy();
            _world.Destroy();
        }
    }
}
