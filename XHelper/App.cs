using System;
using System.Windows.Forms;
using XHelper.XServer;

namespace XHelper {
    static class App {
        static void Main(string[] args) {
            using(XServerManager serverManager = new XServerManager()) {
                if(serverManager.Success) {
                    
                } else {
                    MessageBox.Show("Не удалось запустить XServer", "Ошибка запуска", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}