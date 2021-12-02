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
