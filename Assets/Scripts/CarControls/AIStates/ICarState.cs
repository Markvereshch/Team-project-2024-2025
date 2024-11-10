public interface ICarState
{
    void EnterState(AICarControl carControl);

    void UpdateState();

    void ExitState();
}
