using System;
using Windows.Foundation;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SystemHelpers.Models
{
    public class DisplayModel : ObservableObject
    {
        #region Internal Fields

        internal bool isStale;

        #endregion Internal Fields

        #region Private Fields

        private Rect bounds = Rect.Empty;

        private string deviceId = string.Empty;

        private string displayName = string.Empty;

        private IntPtr hMonitor = IntPtr.Zero;

        private int index;

        private bool isPrimary;

        private Rect workingArea = Rect.Empty;

        #endregion Private Fields

        #region Internal Constructors

        public DisplayModel(string deviceName)
        {
            DeviceName = deviceName;
        }

        #endregion Internal Constructors

        #region Public Properties

        public Rect Bounds
        {
            get => bounds;
            internal set => SetProperty(ref bounds, value);
        }

        public string DeviceId
        {
            get => deviceId;
            internal set => SetProperty(ref deviceId, value);
        }

        public string DeviceName { get; }

        public string DisplayName
        {
            get => displayName;
            internal set => SetProperty(ref displayName, value);
        }

        public IntPtr HMonitor
        {
            get => hMonitor;
            internal set => SetProperty(ref hMonitor, value);
        }

        public int Index
        {
            get => index;
            internal set => SetProperty(ref index, value);
        }

        public bool IsPrimary
        {
            get => isPrimary;
            internal set => SetProperty(ref isPrimary, value);
        }

        private double _scaling = 1.0;

        public double Scaling
        {
            get => _scaling;
            set => SetProperty(ref _scaling, value);
        }

        public Rect WorkingArea
        {
            get => workingArea;
            internal set => SetProperty(ref workingArea, value);
        }

        #endregion Public Properties
    }
}
