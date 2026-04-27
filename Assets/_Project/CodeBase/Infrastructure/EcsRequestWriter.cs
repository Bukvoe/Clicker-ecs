using _Project.CodeBase.Shared.Interfaces;
using Leopotam.EcsLite;

namespace _Project.CodeBase.Infrastructure
{
    internal class EcsRequestWriter : IRequestWriter
    {
        private readonly EcsWorld _world;

        public EcsRequestWriter(EcsWorld world)
        {
            _world = world;
        }

        public void WriteRequest<T>(T request) where T : struct, IRequest
        {
            var entity = _world.NewEntity();
            _world.GetPool<T>().Add(entity) = request;
        }
    }
}
