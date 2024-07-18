using System;
using Serilog;
using Utils.Helpers.LocalSettingsHelper.Abstractions;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace Utils.Helpers.LocalSettingsHelper.Models;

internal class ValueTypeStorage<T> : AbstractLocalStorage<T>, ILocalStorage<T>
{
    private readonly ApplicationDataContainer _currentContainer;

    private readonly string _key;
    bool ILocalStorage<T>.Exists => _currentContainer.Values.ContainsKey(_key);

    T ILocalStorage<T>.Value
    {
        get => TryGetValue(_key, out object value) ? (T)value : default;

        set => AddValue(_key, value);
    }

    public ValueTypeStorage(string key)
    {
        _currentContainer = ApplicationData.Current.LocalSettings;

        _key = key;
    }

    void ILocalStorage<T>.Delete()
    {
        _currentContainer.Values.Remove(_key);
    }

    protected internal override void AddValue(string key, T newValue)
    {
        try
        {
            IPropertySet values = _currentContainer.Values;

            if (values.ContainsKey(key))
            {
                if (!values[key].Equals(newValue))
                {
                    values[key] = newValue;
                }
            }
            else
            {
                values.Add(key, newValue);
            }
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, ex.Message);
        }
    }

    protected internal override bool TryGetValue(string key, out object extractedValue)
    {
        try
        {
            IPropertySet values = _currentContainer.Values;
            bool isSuccess = values.TryGetValue(key, out object tempValue);
            if (isSuccess)
            {
                extractedValue = tempValue;
                return true;
            }

            extractedValue = null;
            return false;
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, ex.Message);

            extractedValue = null;
            return false;
        }
    }
}