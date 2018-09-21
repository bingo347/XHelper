using System;
using System.Threading;

namespace XHelper.KeyboardLayout {
    public delegate void KeyboardLayoutChangedEventHandler(string layout);

    class KeyboardLayoutWatcher : IDisposable {
        public event KeyboardLayoutChangedEventHandler KeyboardLayoutChanged;

        private readonly Timer timer;
        private int currentLayout;

        public KeyboardLayoutWatcher() {
            timer = new Timer(new TimerCallback(CheckKeyboardLayout), null, 0, 500);
            currentLayout = RequestCurrentLayout();
        }

        public string CurrentLayout {
            get {
                switch(currentLayout) {
                case 1049:
                    return "ru";
                case 1033:
                default:
                    return "us";
                }
            }
        }

        public void Dispose() {
            DisposeTimer();
            GC.SuppressFinalize(this);
        }

        private void CheckKeyboardLayout(object sender) {
            int newLayout = RequestCurrentLayout();
            if(currentLayout == newLayout) { return; }
            currentLayout = newLayout;
            if(KeyboardLayoutChanged == null) { return; }
            KeyboardLayoutChanged(CurrentLayout);
        }

        private int RequestCurrentLayout() {
            try {
                IntPtr foregroundWindow = WinApi.GetForegroundWindow();
                uint foregroundProcess = WinApi.GetWindowThreadProcessId(foregroundWindow, IntPtr.Zero);
                int keyboardLayout = WinApi.GetKeyboardLayout(foregroundProcess).ToInt32() & 0xFFFF;
                if(keyboardLayout == 0) {
                    keyboardLayout = 1033;
                }
                return keyboardLayout;
            } catch(Exception) {
                return 1033;
            }
        }

        private void DisposeTimer() {
            timer.Dispose();
        }

        ~KeyboardLayoutWatcher() {
            DisposeTimer();
        }
    }
}
