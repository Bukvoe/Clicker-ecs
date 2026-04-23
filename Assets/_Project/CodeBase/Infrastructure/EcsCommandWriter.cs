using _Project.CodeBase.Shared.Interfaces;
using Leopotam.EcsLite;

namespace _Project.CodeBase.Infrastructure
{
    internal class EcsCommandWriter : ICommandWriter
    {
        private readonly EcsWorld _world;

        public EcsCommandWriter(EcsWorld world)
        {
            _world = world;
        }

        public void WriteCommand<T>(T command) where T : struct, ICommand
        {
            var entity = _world.NewEntity();
            _world.GetPool<T>().Add(entity) = command;
        }
    }
}
