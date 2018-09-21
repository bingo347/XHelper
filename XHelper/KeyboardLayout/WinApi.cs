using System;
using System.Runtime.InteropServices;

namespace XHelper.KeyboardLayout {
    static class WinApi {
        [DllImport("user32.dll")] static public extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")] static public extern uint GetWindowThreadProcessId(IntPtr hwnd, IntPtr proccess);
        [DllImport("user32.dll")] static public extern IntPtr GetKeyboardLayout(uint thread);
    }
}
