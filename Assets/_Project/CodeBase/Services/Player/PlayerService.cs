namespace _Project.CodeBase.Services.Player
{
    public class PlayerService : IPlayerService
    {
        public int PlayerEntity { get; private set; }

        public void SetPlayerEntity(int playerEntity)
        {
            PlayerEntity = playerEntity;
        }
    }
}
