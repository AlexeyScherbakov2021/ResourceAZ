using ResourceAZ.Infrastructure;
using ResourceAZ.ViewModels;
using ResourceAZ.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ResourceAZ
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Window ActivedWindow => Current.Windows?.Cast<Window>().FirstOrDefault(w => w.IsActive);
        public static Window FocusedWindow => Current.Windows?.Cast<Window>().FirstOrDefault(w => w.IsFocused);
        public static Window CurrentAFWindow => FocusedWindow ?? ActivedWindow;
        public static MainWindow mainWindow => Current.MainWindow as MainWindow;


        public DisplayRootRegistry displayRootRegistry = new DisplayRootRegistry();
        //MainWindowViewModel MainVM;

        public App()
        {
            displayRootRegistry.RegisterWindowType<MainWindowViewModel, MainWindow>();
            displayRootRegistry.RegisterWindowType<CalcWindowViewModel, CalcWindow>();
        }

        protected override /*async*/ void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            //await RunProgramLogic();
            //Shutdown();
        }

        //private async Task RunProgramLogic()
        //{
        //    while(true)
        //    {
        //        MainVM = new MainWindowViewModel();
        //        await displayRootRegistry.ShowModalPresentation(MainVM);

        //        //while(true)
        //        //{
        //        //}

        //        displayRootRegistry.HidePresentation(MainVM);

        //    }
        //}
    }
}
