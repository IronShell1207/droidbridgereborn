namespace Utils.Helpers.LocalSettingsHelper.Abstractions;

internal abstract class AbstractLocalStorage<T>
{
    protected internal abstract void AddValue(string key, T newValue);

    protected internal abstract bool TryGetValue(string key, out object extractedValue);
}