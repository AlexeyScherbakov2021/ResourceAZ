using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ResourceAZ.Infrastructure
{
    public class DisplayRootRegistry
    {
        Dictionary<Type, Type> vmToWindowMapping = new Dictionary<Type, Type>();

        public void RegisterWindowType<VM, Win>() where Win: Window, new() where VM: class
        {
            var vmType = typeof(VM);
            if (vmType.IsInterface)
                throw new ArgumentException("cannot register interface");

            if (vmToWindowMapping.ContainsKey(vmType))
                throw new InvalidOperationException($"Type {vmType.FullName} is already registered");

            vmToWindowMapping[vmType] = typeof(Win);
        }

        public void UnregisterWindowType<VM>()
        {
            var vmType = typeof(VM);
            if (vmType.IsInterface)
                throw new ArgumentException("cannot register interface");

            if (!vmToWindowMapping.ContainsKey(vmType))
                throw new InvalidOperationException($"Type {vmType.FullName} is not registered");

            vmToWindowMapping.Remove(vmType);
        }

        public Window CreateWindowWithVM(object vm)
        {
            if(vm == null)
                throw new ArgumentNullException("vm");

            Type WindowType = null;
            var vmType = vm.GetType();

            while(vmType != null && !vmToWindowMapping.TryGetValue(vmType, out WindowType))
                vmType = vmType.BaseType;

            if (WindowType == null)
                throw new ArgumentException($"No registered window type for arguent type {vm.GetType().FullName}");

            var window = (Window)Activator.CreateInstance(WindowType);

            window.DataContext = vm;

            return window;
        }



        //Dictionary<object, Window> openWindows = new Dictionary<object, Window>();
        //public void ShowPresentation(object vm)
        //{
        //    if (vm == null)
        //        throw new ArgumentNullException("vm");
        //    if (openWindows.ContainsKey(vm))
        //        throw new InvalidOperationException("UI for this VM is already displayed");
        //    var window = CreateWindowWithVM(vm);
        //    window.Show();
        //    openWindows[vm] = window;
        //}

        //public void HidePresentation(object vm)
        //{
        //    Window window;
        //    if (!openWindows.TryGetValue(vm, out window))
        //        throw new InvalidOperationException("UI for this VM is not displayed");
        //    window.Close();
        //    openWindows.Remove(vm);
        //}

        //public async Task ShowModalPresentation(object vm)
        //{
        //    var window = CreateWindowWithVM(vm);
        //    window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        //    await window.Dispatcher.InvokeAsync(() => window.ShowDialog());
        //}

    }
}
