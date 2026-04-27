namespace _Project.CodeBase.Shared.Interfaces
{
    public interface IRequestWriter
    {
        public void WriteRequest<T>(T request) where T : struct, IRequest;
    }
}
