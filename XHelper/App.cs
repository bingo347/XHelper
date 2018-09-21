using System.Windows.Forms;
using XHelper.XServer;
using XHelper.KeyboardLayout;
using XHelper.WSL;

namespace XHelper {
    static class App {
        static void Main(string[] args) {
            using(XServerManager serverManager = new XServerManager()) {
                if(serverManager.Success) {
                    new WSLRunner(new KeyboardLayoutWatcher()).Run();
                } else {
                    MessageBox.Show("Не удалось запустить XServer", "Ошибка запуска", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}