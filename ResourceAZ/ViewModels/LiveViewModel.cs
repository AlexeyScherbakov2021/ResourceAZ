using Microsoft.Win32;
using ResourceAZ.Infrastructure.Commands;
using ResourceAZ.ViewModels.Base;
using ResourceAZ.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static ResourceAZ.Infrastructure.Reg;

namespace ResourceAZ.ViewModels
{
    internal class LiveViewModel : ViewModel
    {
        string Machine;

        private string _Answer;
        public string Answer { get => _Answer; set { Set(ref _Answer, value); } }

        private string _GetFile;
        public string GetFile { get => _GetFile; set { Set(ref _GetFile, value); } }

        // кнопка регистрации
        public ICommand LiveCommand { get; }
        private bool CanLiveCommand(object p) => File.Exists(Answer);
        private void OnLiveCommandExecuted(object p)
        {
            string saveFile = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\ResourceAZ.lic";
            File.Copy(Answer, saveFile, true);
            WindowCollection wins = App.Current.Windows;
            foreach(Window win in App.Current.Windows)
            {
                if(win.GetType() == typeof(LiveView))
                {
                    win.Close();
                    break;
                }
            }
        }


        // Кнока Найти
        public ICommand BrowseCommand { get; }
        private bool CanBrowseCommand(object p) => true;
        private void OnBrowseCommandExecuted(object p)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Файлы регистрации (*.lic)|*.lic";

            if (dlg.ShowDialog() == false)
                return;

            Answer = dlg.FileName;

        }





        public LiveViewModel()
        {
            LiveCommand = new LambdaCommand(OnLiveCommandExecuted, CanLiveCommand);
            BrowseCommand = new LambdaCommand(OnBrowseCommandExecuted, CanBrowseCommand);

            //получение уникального номера компьютера
            Machine = Encryption.UniqueMachineId() + Encryption.GetMACAddress();
            // зашифровка 
            string s = Encryption.Encrypt(Machine, ProgramNameIn);
            s = Encryption.Encrypt(s, ProgramNameIn);

            string saveFile = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\reginfo.gen";

            File.WriteAllText(saveFile, s);

            GetFile = saveFile;


        }
    }
}
