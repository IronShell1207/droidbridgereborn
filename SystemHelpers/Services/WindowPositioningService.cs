using System;
using System.Runtime.InteropServices;

namespace SystemHelpers.Services;

public class WindowPositioningService
{
    private const int GWL_EXSTYLE = -20;
    private const int WS_EX_NOACTIVATE = 0x08000000;
    
    [DllImport("user32.dll")]
    public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    private static IntPtr HWND_BOTTOM = (IntPtr)1;
    private static IntPtr HWND_TOP = (IntPtr)0;

    public static void SetWindowAtBottom(IntPtr handle)
    {
        var currentState = GetWindowLong(handle, GWL_EXSTYLE);
        SetWindowLong(handle, GWL_EXSTYLE, currentState | WS_EX_NOACTIVATE);
    }

    public static void SetWindowToNormal(IntPtr handle)
    {
        SetWindowLong(handle, GWL_EXSTYLE, 0);
    }
}