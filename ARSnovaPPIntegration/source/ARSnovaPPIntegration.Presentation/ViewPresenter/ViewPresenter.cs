﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ARSnovaPPIntegration.Presentation.Content;
using ARSnovaPPIntegration.Presentation.Window;

namespace ARSnovaPPIntegration.Presentation.ViewPresenter
{
    public class ViewPresenter
    {
        private readonly Dictionary<Type, ViewTypeConfiguration> viewTypeConfigurations =
            new Dictionary<Type, ViewTypeConfiguration>();

        private readonly List<RunningViewModel> runningViewModels = new List<RunningViewModel>();

        public void Add<TViewModel, TView>()
        {
            var viewConfiguration = new ViewTypeConfiguration { ViewModelType = typeof(TViewModel), ViewType = typeof(TView) };

            if (!this.viewTypeConfigurations.ContainsKey(viewConfiguration.ViewModelType))
            {
                this.viewTypeConfigurations.Add(viewConfiguration.ViewModelType, viewConfiguration);
            }
        }

        public void Show<TViewModel>(TViewModel viewModel) where TViewModel : class
        {
            var viewModelType = typeof(TViewModel);

            if (!this.viewTypeConfigurations.ContainsKey(viewModelType))
            {
                throw new ArgumentException($"ViewModel not found: '{viewModelType.FullName}'");
            }

            var viewTypeConfiguration = this.viewTypeConfigurations[viewModelType];

            // there are currently no view constructors with params -> add option for constructor calls with elements when necessary
            var view =
                (Control)viewTypeConfiguration.ViewType.GetConstructors()
                                     .FirstOrDefault(c => !c.GetParameters().Any())
                                     .Invoke(new object[0]);
            view.DataContext = viewModel;

            var window = new WindowContainer() {ShowInTaskbar = true};
            var logoBitmap = Images.ARSnova_Logo;
            var iconBitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                logoBitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromWidthAndHeight(16, 16));
            window.Icon = iconBitmapSource;

            this.SetWindowCommandBindings(viewModel, window);

            window.Content.Children.Clear();
            window.Content.Children.Add(view);

            var runningViewModel = new RunningViewModel { Window = window, ViewModel = viewModel, View = view };

            if (!this.runningViewModels.Contains(runningViewModel))
            {
                this.runningViewModels.Add(runningViewModel);
            }

            // show -> calling prog doesn't wait (and freezes), showDialog() -> caller waits.... do we want to freeze pp?
            window.ShowDialog();
        }

        public void Close<TViewModel>()
            where TViewModel : class
        {
            var viewModelType = typeof(TViewModel);
            var runningViewModel = this.runningViewModels.FirstOrDefault(avm => avm.ViewModel.GetType() == viewModelType);

            if (runningViewModel != null)
            {
                var window = runningViewModel.Window;
                
                window.Close();
                // TODO do I need to clean up event handlers / bindings (-> yes, done)? I think there should be any, check later!
                this.RemoveWindowCommandBindings(runningViewModel.ViewModel, runningViewModel.Window);

                (runningViewModel.ViewModel as IDisposable)?.Dispose();
                (runningViewModel.View as IDisposable)?.Dispose();
            }
        }

        private void SetWindowCommandBindings(object viewModel, WindowContainer window)
        {
            var windowCommandsInViewModel = viewModel as IWindowCommandBindings;

            var windowCommandBindings = windowCommandsInViewModel?.WindowCommandBindings;

            if (windowCommandBindings == null)
            {
                throw new ArgumentException($"IWindowCommandBindings not implemented for ViewModel: '{viewModel.GetType().FullName}'");
            }

            window.SetWindowCommandBindings(windowCommandBindings);
        }
        
        private void RemoveWindowCommandBindings(object viewModel, System.Windows.Window window)
        {
            var windowCommandsInViewModel = viewModel as IWindowCommandBindings;

            var windowCommandBindings = windowCommandsInViewModel?.WindowCommandBindings;

            if (windowCommandBindings != null)
            {
                foreach (var windowCommandBinding in windowCommandBindings)
                {
                    window.CommandBindings.Remove(windowCommandBinding);
                }
            }
        }
    }
}