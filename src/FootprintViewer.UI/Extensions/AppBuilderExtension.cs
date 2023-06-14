﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Dialogs;
using System.Runtime.InteropServices;

namespace FootprintViewer.UI.Extensions;

public static class AppBuilderExtension
{
    public static AppBuilder SetupAppBuilder(this AppBuilder appBuilder)
    {
        bool enableGpu = false;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            appBuilder
                .UseWin32()
                .UseSkia();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            appBuilder.UsePlatformDetect()
               // .UseManagedSystemDialogs<AppBuilder, Window>();
                .UseManagedSystemDialogs<Window>();
        }
        else
        {
            appBuilder.UsePlatformDetect();
        }

        return appBuilder
            .With(new SkiaOptions { MaxGpuResourceSizeBytes = 2560 * 1600 * 4 * 12 })
            .With(new Win32PlatformOptions { AllowEglInitialization = enableGpu, /* UseDeferredRendering = true,*/ UseWindowsUIComposition = true })
            .With(new X11PlatformOptions { UseGpu = enableGpu, WmClass = "FootprintViewer" })
            .With(new AvaloniaNativePlatformOptions { /*UseDeferredRendering = true,*/ UseGpu = enableGpu })
            .With(new MacOSPlatformOptions { ShowInDock = true });
    }
}
