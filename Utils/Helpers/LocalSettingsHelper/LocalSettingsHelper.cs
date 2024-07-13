using System;
using Serilog;
using Utils.Helpers.LocalSettingsHelper.Abstractions;
using Utils.Helpers.LocalSettingsHelper.Models;

namespace Utils.Helpers.LocalSettingsHelper
{
    /// <summary>
    /// Helper локальных настроек.
    /// </summary>
    public sealed class LocalSettingsHelper<T>
    {
        /// <summary>
        /// Хранилище локальных данных.
        /// </summary>
        private readonly ILocalStorage<T> _localStorage;
        public bool Exists => _localStorage.Exists;
        public T Value
        {
            get => _localStorage.Value;

            set => _localStorage.Value = value;
        }
        public LocalSettingsHelper(string key)
        {
           
            if (typeof(T).IsValueType || typeof(T) == typeof(string))
            {
                _localStorage = new ValueTypeStorage<T>(key);
            }
         
        }
        public void Delete()
        {
            try
            {
                _localStorage.Delete();
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, ex.Message);
            }
        }
        public override string ToString() => Value.ToString();
    }
}