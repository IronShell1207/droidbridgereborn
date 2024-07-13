using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using CommunityToolkit.Mvvm.ComponentModel;
using SystemHelpers.Models;
using System.Data;
using static System.Formats.Asn1.AsnWriter;

namespace SystemHelpers.Static
{
    public class DisplayService : ObservableObject //,IWndProcHookHandler
    {
        #region Private Fields

        private static readonly Lazy<DisplayService> _instance = new((() => new DisplayService()));
        public static DisplayService Instance => _instance.Value;
        private const string defaultDisplayDeviceName = "DISPLAY";

        private const int MONITOR_DEFAULTTONEAREST = 0x00000002;

        private const int MONITORINFOF_PRIMARY = 0x00000001;
        public enum DpiType
        {
            Effective = 0,
            Angular = 1,
            Raw = 2,
        }


        private const int PRIMARY_MONITOR = unchecked((int)0xBAADF00D);

        /// <summary>
        /// Queries the dots per inch (dpi) of a display.
        /// </summary>
        /// <param name="hmonitor"> Handle of the monitor being queried. </param>
        /// <param name="dpiType"> The type of DPI being queried. </param>
        /// <param name="dpiX"> The value of the DPI along the X axis. </param>
        /// <param name="dpiY"> The value of the DPI along the Y axis. </param>
        /// <returns> Status success </returns>
        /// <remarks>
        /// <see cref="https://learn.microsoft.com/en-us/windows/win32/api/shellscalingapi/nf-shellscalingapi-getdpiformonitor"/>
        /// </remarks>
        [DllImport("Shcore.dll")]
        private static extern IntPtr GetDpiForMonitor([In] IntPtr hmonitor, [In] DpiType dpiType, [Out] out uint dpiX, [Out] out uint dpiY);


        private static bool multiMonitorSupport;

        private Rect virtualScreenBounds = Rect.Empty;

        #endregion Private Fields

        #region Private Constructors

        private DisplayService()
        {
            RefreshDisplayModelList();
        }

        #endregion Private Constructors

        #region Public Events

        public event EventHandler DisplayUpdated;

        #endregion Public Events

        #region Public Properties



        public ObservableCollection<DisplayModel> Displays { get; } = new ObservableCollection<DisplayModel>();

        public DisplayModel PrimaryDisplay => Displays
            .FirstOrDefault(x => x.IsPrimary);

        public Rect VirtualScreenBounds
        {
            get => virtualScreenBounds;
            private set => SetProperty(ref virtualScreenBounds, value);
        }

        #endregion Public Properties

        #region Public Methods

        public DisplayModel GetDisplayModelFromHWnd(IntPtr hWnd)
        {
            IntPtr hMonitor = multiMonitorSupport
                ? PinvokeWindowMethods.MonitorFromWindow(new HandleRef(null, hWnd), MONITOR_DEFAULTTONEAREST)
                : (IntPtr)PRIMARY_MONITOR;

            return GetDisplayModelFromHMonitor(hMonitor);
        }

        public DisplayModel GetDisplayModelFromPoint(Point point)
        {
            IntPtr hMonitor;
            if (multiMonitorSupport)
            {
                PinvokeWindowMethods.POINT pt = new PinvokeWindowMethods.POINT(  //POINTSTRUCT
                    (int)Math.Round(point.X),
                    (int)Math.Round(point.Y));
                hMonitor = PinvokeWindowMethods.MonitorFromPoint(pt, MONITOR_DEFAULTTONEAREST);
            }
            else
                hMonitor = (IntPtr)PRIMARY_MONITOR;

            return GetDisplayModelFromHMonitor(hMonitor);
        }

        public uint OnHwndCreated(IntPtr hWnd, out bool register)
        {
            register = false;
            return 0;
        }

        public IntPtr OnWndProc(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            if (msg == (uint)PinvokeWindowMethods.WM.DISPLAYCHANGE)
            //|| (msg == (uint)PinvokeWindowMethods.WM.SETTINGCHANGE && wParam == ((IntPtr)PinvokeWindowMethods.SPI.SPI_SETWORKAREA)))
            {
                RefreshDisplayModelList();
            }
            return IntPtr.Zero;
        }

        #endregion Public Methods

        #region Private Methods

        private static string GetDefaultDisplayDeviceId()
        {
            return PinvokeWindowMethods.GetSystemMetrics((int)PinvokeWindowMethods.SystemMetric.SM_REMOTESESSION) != 0 ?
"\\\\?\\DISPLAY#REMOTEDISPLAY#" : "\\\\?\\DISPLAY#LOCALDISPLAY#";
        }

        private static PinvokeWindowMethods.DISPLAY_DEVICE GetDisplayDevice(string deviceName)
        {
            PinvokeWindowMethods.DISPLAY_DEVICE result = new PinvokeWindowMethods.DISPLAY_DEVICE();

            PinvokeWindowMethods.DISPLAY_DEVICE displayDevice = new PinvokeWindowMethods.DISPLAY_DEVICE();
            displayDevice.cb = Marshal.SizeOf(displayDevice);
            try
            {
                for (uint id = 0; PinvokeWindowMethods.EnumDisplayDevices(deviceName, id, ref displayDevice, PinvokeWindowMethods.EDD_GET_DEVICE_INTERFACE_NAME); id++)
                {
                    if (displayDevice.StateFlags.HasFlag(PinvokeWindowMethods.DisplayDeviceStateFlags.AttachedToDesktop)
                        && !displayDevice.StateFlags.HasFlag(PinvokeWindowMethods.DisplayDeviceStateFlags.MirroringDriver))
                    {
                        result = displayDevice;
                        break;
                    }

                    displayDevice.cb = Marshal.SizeOf(displayDevice);
                }
            }
            catch { }

            if (string.IsNullOrEmpty(result.DeviceID)
                || string.IsNullOrWhiteSpace(result.DeviceID))
            {
                result.DeviceID = GetDefaultDisplayDeviceId();
            }

            return result;
        }

        private static Rect GetVirtualScreenBounds()
        {
            Point location = new Point(PinvokeWindowMethods.GetSystemMetrics(
                (int)PinvokeWindowMethods.SystemMetric.SM_XVIRTUALSCREEN), PinvokeWindowMethods.GetSystemMetrics((int)PinvokeWindowMethods.SystemMetric.SM_YVIRTUALSCREEN));
            Size size = new Size(PinvokeWindowMethods.GetSystemMetrics(
                (int)PinvokeWindowMethods.SystemMetric.SM_CXVIRTUALSCREEN), PinvokeWindowMethods.GetSystemMetrics((int)PinvokeWindowMethods.SystemMetric.SM_CYVIRTUALSCREEN));
            return new Rect(location, size);
        }

        private static Rect GetWorkingArea()
        {
            PinvokeWindowMethods.RECT rc = new PinvokeWindowMethods.RECT();
            PinvokeWindowMethods.SystemParametersInfo((int)PinvokeWindowMethods.SPI.SPI_GETWORKAREA, 0, ref rc, 0);
            return new Rect(rc.Left, rc.Top,
                rc.Right - rc.Left, rc.Bottom - rc.Top);
        }

        private DisplayModel CreateDisplayModelFromMonitorInfo(string deviceName)
        {
            DisplayModel DisplayModel = new DisplayModel(deviceName);

            PinvokeWindowMethods.DISPLAY_DEVICE displayDevice = GetDisplayDevice(deviceName);
            DisplayModel.DeviceId = displayDevice.DeviceID;
            DisplayModel.DisplayName = displayDevice.DeviceString;

            Displays.Add(DisplayModel);

            return DisplayModel;
        }

        private DisplayModel GetDisplayModelByDeviceName(string deviceName)
        {
            return Displays.FirstOrDefault(x => x.DeviceName == deviceName);
        }

        private DisplayModel GetDisplayModelFromHMonitor(IntPtr hMonitor)
        {
            DisplayModel DisplayModel = null;

            if (!multiMonitorSupport || hMonitor == (IntPtr)PRIMARY_MONITOR)
            {
                DisplayModel = GetDisplayModelByDeviceName(defaultDisplayDeviceName);

                if (DisplayModel == null)
                {
                    DisplayModel = new DisplayModel(defaultDisplayDeviceName);
                    Displays.Add(DisplayModel);
                }

                DisplayModel.Bounds = GetVirtualScreenBounds();
                DisplayModel.DeviceId = GetDefaultDisplayDeviceId();
                DisplayModel.DisplayName = "Display";
                DisplayModel.HMonitor = hMonitor;
                DisplayModel.IsPrimary = true;
                DisplayModel.WorkingArea = GetWorkingArea();
                UpdateScalingForDisplay(DisplayModel);

                DisplayModel.isStale = false;
            }
            else
            {
                PinvokeWindowMethods.MONITORINFOEX info = new PinvokeWindowMethods.MONITORINFOEX();// MONITORINFOEX();
                PinvokeWindowMethods.GetMonitorInfo(new HandleRef(null, hMonitor), info);

                string deviceName = new string(info.szDevice).TrimEnd((char)0);

                DisplayModel = GetDisplayModelByDeviceName(deviceName);

                DisplayModel ??= CreateDisplayModelFromMonitorInfo(deviceName);

                DisplayModel.HMonitor = hMonitor;
                UpdateDisplayModel(DisplayModel, info);
                UpdateScalingForDisplay(DisplayModel);
            }

            return DisplayModel;
        }
        public static void UpdateScalingForDisplay(DisplayModel display)
        {
            uint dpiX, dpiY;
            GetDpiForMonitor(display.HMonitor, DpiType.Effective, out dpiX, out dpiY);
            display.Scaling= (double)dpiX / 96.0;
        }

        private IList<IntPtr> GetHMonitors()
        {
            if (multiMonitorSupport)
            {
                List<IntPtr> hMonitors = new List<IntPtr>();

                bool callback(IntPtr monitor, IntPtr hdc, IntPtr lprcMonitor, IntPtr lParam)
                {
                    hMonitors.Add(monitor);
                    return true;
                }

                PinvokeWindowMethods.EnumDisplayMonitors(new HandleRef(null, IntPtr.Zero), null, callback, IntPtr.Zero);

                return hMonitors;
            }

            return new[] { (IntPtr)PRIMARY_MONITOR };
        }

        public void RefreshDisplayModelList()
        {
            multiMonitorSupport = PinvokeWindowMethods.GetSystemMetrics((int)PinvokeWindowMethods.SystemMetric.SM_CMONITORS) != 0;

            IList<IntPtr> hMonitors = GetHMonitors();

            foreach (DisplayModel DisplayModel in Displays)
            {
                DisplayModel.isStale = true;
            }

            for (int i = 0; i < hMonitors.Count; i++)
            {
                DisplayModel DisplayModel = GetDisplayModelFromHMonitor(hMonitors[i]);
                DisplayModel.Index = i + 1;
            }

            List<DisplayModel> staleDisplayModels = Displays
                .Where(x => x.isStale).ToList();
            foreach (DisplayModel DisplayModel in staleDisplayModels)
            {
                Displays.Remove(DisplayModel);
            }

            staleDisplayModels.Clear();
            staleDisplayModels = null;

            VirtualScreenBounds = GetVirtualScreenBounds();

            DisplayUpdated?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateDisplayModel(DisplayModel DisplayModel, PinvokeWindowMethods.MONITORINFOEX info)
        {
            DisplayModel.Bounds = new Rect(
                info.rcMonitor.Left, info.rcMonitor.Top,
                info.rcMonitor.Right - info.rcMonitor.Left,
                info.rcMonitor.Bottom - info.rcMonitor.Top);

            DisplayModel.IsPrimary = (info.dwFlags & MONITORINFOF_PRIMARY) != 0;

            DisplayModel.WorkingArea = new Rect(
                info.rcWork.Left, info.rcWork.Top,
                info.rcWork.Right - info.rcWork.Left,
                info.rcWork.Bottom - info.rcWork.Top);

            DisplayModel.isStale = false;
        }

        #endregion Private Methods
    }
}
