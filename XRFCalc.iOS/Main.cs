using System.Runtime.InteropServices;
using UIKit;

namespace XRFCalc.iOS;

public class Application
{
    // This is the main entry point of the application.
    static void Main(string[] args)
    {
        //NativeLibrary.TryLoad("/System/Library/Frameworks/WebKit.framework/WebKit", out _);   // does not help
        // if you want to use a different Application Delegate class from "AppDelegate"
        // you can specify it here.
        UIApplication.Main(args, null, typeof(AppDelegate));
    }
}