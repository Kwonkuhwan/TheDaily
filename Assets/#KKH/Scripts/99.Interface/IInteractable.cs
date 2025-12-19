namespace KKH
{
    public interface IInteractable
    {
        string Prompt { get; }
        void Interact(Interactor interactor);
    }
}