using System.Windows.Input;

namespace _Project.CodeBase.Shared.Interfaces
{
    public interface ICommandWriter
    {
        public void WriteCommand<T>(T command) where T : struct, ICommand;
    }
}
