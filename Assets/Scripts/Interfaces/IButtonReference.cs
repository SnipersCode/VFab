public interface IButtonReference<T, in T2>
{
    void Initialize(T2 targetDisplay, T targetReference);
    void InvokeAction();
    T GetReference();
}