namespace RPG.Player
{
    public interface IPlayerModule
    {
        public void Init(PlayerController controller);
        public void ToggleModule(bool toggle);
    }
}
