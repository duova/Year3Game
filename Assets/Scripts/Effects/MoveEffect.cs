namespace Effects
{
    public interface IMoveEffect
    {
        void BeginMove();

        void MoveTick();

        void EndMove();
    }
}