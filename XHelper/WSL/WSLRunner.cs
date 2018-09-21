using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using XHelper.KeyboardLayout;

namespace XHelper.WSL {
    class WSLRunner {
        private delegate bool CommandRunner(string cmd);
        private readonly Process wslProcess;
        private readonly Mutex stdinMutex;
        private readonly KeyboardLayoutWatcher keyboardLayoutWatcher;

        public WSLRunner(KeyboardLayoutWatcher keyboardLayoutWatcher) {
            wslProcess = Process.Start(new ProcessStartInfo("wsl", "XHelper") {
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = false,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            });
            stdinMutex = new Mutex();
            this.keyboardLayoutWatcher = keyboardLayoutWatcher;
        }

        public void Run() {
            SendCommand("term");
            OnKeyboardLayoutChanged(keyboardLayoutWatcher.CurrentLayout);
            keyboardLayoutWatcher.KeyboardLayoutChanged += OnKeyboardLayoutChanged;
            Task.WaitAny(
                ReadOutput(wslProcess.StandardOutput, RunStdoutCommand),
                ReadOutput(wslProcess.StandardError, ShowStderrOut)
            );
            SendCommand("exit");
            Thread.Sleep(500);
        }

        private async Task ReadOutput(StreamReader output, CommandRunner runCommand) {
            char[] buf = new char[256];
            int index = 0;
            while(true) {
                await output.ReadAsync(buf, index, 1);
                if(buf[index] == '\n') {
                    char[] cmd = new char[index];
                    Array.Copy(buf, cmd, index);
                    if(runCommand(new string(cmd))) { return; }
                    index = 0;
                } else {
                    index++;
                }
            }
        }

        private bool RunStdoutCommand(string cmd) {
            bool exit = false;
            switch(cmd) {
            case "exit":
                exit = true;
                break;
            }
            return exit;
        }

        private bool ShowStderrOut(string info) {
            MessageBox.Show(info, "Ошибка WSL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return true;
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
