using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using System.Collections.Generic;
using System.Diagnostics;
using XRFCalc.ViewModels;
using XRFCalc.Views;

namespace XRFCalc;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public static bool IsDesktop { get; private set; } = false;
    public static IStorageProvider? StorageProvider { get; private set; } = null;
    public MainView? MainView { get; private set; }

    private TabControl? mainTab = null;
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
            StorageProvider = desktop.MainWindow.StorageProvider;
            IsDesktop = true;
            IEnumerable<ILogical> test = desktop.MainWindow.GetLogicalChildren();
            foreach (var item in test)
            {
                if (item is MainView mv)
                {
                    mainTab = mv.FindControl<TabControl>("MainTabControl");
                    break;
                }
            }
        }
        else if (ApplicationLifetime is IActivityApplicationLifetime activityLifetime)
            activityLifetime.MainViewFactory = ViewFactory;
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
            mainTab = MainView.FindControl<TabControl>("MainTabControl");
            MainView.Loaded += MainView_Loaded;
            //var top = TopLevel.GetTopLevel(singleViewPlatform.MainView);
            //storageProvider = top?.StorageProvider;

        }
        //Debug.Assert(mainTab != null);
        if (mainTab != null)
            XRFCalcUIDefinition.InitializeApplication(mainTab, StorageProvider);

        base.OnFrameworkInitializationCompleted();
    }

    private MainView ViewFactory()
    {
        MainView = new MainView
        {
            DataContext = new MainViewModel()
        };
        mainTab = MainView.FindControl<TabControl>("MainTabControl");
        MainView.Loaded += MainView_Loaded;
        return MainView;
    }


    private void MainView_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var top = TopLevel.GetTopLevel(MainView);
        StorageProvider = top?.StorageProvider;
        XRFCalcContent.InitializeData();
        if (mainTab != null && ApplicationLifetime is IActivityApplicationLifetime)
            XRFCalcUIDefinition.InitializeApplication(mainTab, StorageProvider);
    }

}