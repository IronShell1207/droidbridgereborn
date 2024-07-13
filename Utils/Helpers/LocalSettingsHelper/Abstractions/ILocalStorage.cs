namespace Utils.Helpers.LocalSettingsHelper.Abstractions;

internal interface ILocalStorage<T>
{
    public bool Exists
    {
        get;
    }

    public T Value
    {
        get; set;
    }

    public void Delete();
}