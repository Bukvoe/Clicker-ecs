using _Project.CodeBase.Configs;
using _Project.CodeBase.Features.Bootstrap;
using _Project.CodeBase.Features.Businesses;
using _Project.CodeBase.Infrastructure;
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

            var commandWriter = new EcsCommandWriter(_world);
            var cardFactory = new BusinessCardFactory(_businessCardViewPrefab);
            var businessListProvider = new BusinessListProvider();

            _businessesPresenter = new BusinessesPresenter(_businessesView, cardFactory, commandWriter, businessListProvider);

            _systems = new EcsSystems(_world);
            _systems
                .Add(new BootstrapSystem(_gameConfig))
                .Add(new UpdateBusinessesSystem(businessListProvider));

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
