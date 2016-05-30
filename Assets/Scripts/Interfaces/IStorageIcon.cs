public interface IStorageIcon
{
    void Initialize(IWaferUserInterface waferUserInterface, IWaferUserInterface lotUserInterface, ISelectSystem<IStorageIcon> selectSystem, IWaferStorage storageReference);
    void AddToUserInterface(Constants.UserInterface level);
    void ResetState();
}
