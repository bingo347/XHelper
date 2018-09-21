using System.Threading;
using System.Diagnostics;
using XHelper.KeyboardLayout;

namespace XHelper.WSL {
    class WSLRunner {
        private readonly Process wslProcess;
        private readonly Mutex stdinMutex;
        private readonly KeyboardLayoutWatcher keyboardLayoutWatcher;

        public WSLRunner(KeyboardLayoutWatcher keyboardLayoutWatcher) {
            wslProcess = Process.Start("wsl", "XHelper");
            stdinMutex = new Mutex();
            this.keyboardLayoutWatcher = keyboardLayoutWatcher;
        }

        public void Run() {
            keyboardLayoutWatcher.KeyboardLayoutChanged += OnKeyboardLayoutChanged;
        }

        private void OnKeyboardLayoutChanged(string layout) {
            SendCommand(layout);
        }

        private void SendCommand(string cmd) {
            stdinMutex.WaitOne();
            wslProcess.StandardInput.Write(cmd + "\n");
            stdinMutex.ReleaseMutex();
        }
    }
}
