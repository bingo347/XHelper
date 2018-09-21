using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using XHelper.Properties;

namespace XHelper.XServer {
    class XServerManager : IDisposable {
        private readonly Process XServerProcess;
        private readonly string ConfigFile;

        public XServerManager() {
            Process[] startedXServers = Process.GetProcessesByName("vcxsrv");
            if(startedXServers.Length > 0) {
                XServerProcess = startedXServers[0];
                return;
            }
            ConfigFile = Environment.CurrentDirectory + @"\config.xlaunch";
            if(WriteConfigFile()) {
                XServerProcess = Process.Start(ConfigFile);
                return;
            }
            XServerProcess = null;
        }

        public bool Success {
            get {
                return XServerProcess != null;
            }
        }

        public void Dispose() {
            KillServer();
            GC.SuppressFinalize(this);
        }

        private bool WriteConfigFile() {
            try {
                byte[] data = Resources.config;
                using(FileStream fs = File.Create(ConfigFile, data.Length, FileOptions.None)) {
                    using(BinaryWriter bw = new BinaryWriter(fs)) {
                        bw.Write(data);
                        bw.Close();
                    }
                    fs.Close();
                }
                return true;
            } catch(Exception e) {
                MessageBox.Show(e.Message, "Ошибка записи файла конфигурации", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void KillServer() {
            if(XServerProcess == null) { return; }
            XServerProcess.Kill();
            Thread.Sleep(500);
        }

        ~XServerManager() {
            KillServer();
        }
    }
}
