using System;

namespace _Project.CodeBase.Shared.Interfaces
{
    public interface IListener<T>
    {
        event Action<T> Changed;
    }
}
