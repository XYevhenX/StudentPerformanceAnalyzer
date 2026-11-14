namespace StudentPerformanceAnalyzer
{
    public partial class App : Application
    {
        private bool _isClosingConfirmed;

        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new AppShell());

#if WINDOWS
            window.Created += (s, e) =>
            {
                var nativeWindow = window.Handler?.PlatformView as Microsoft.UI.Xaml.Window;
                if (nativeWindow != null)
                {
                    var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
                    var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
                    var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

                    appWindow.Closing += async (sender, args) =>
                    {
                        if (_isClosingConfirmed)
                        {
                            return;
                        }

                        args.Cancel = true;

                        var page = window.Page;
                        if (page != null)
                        {
                            bool answer = await page.DisplayAlert(
                                "Підтвердження виходу",
                                "Чи дійсно ви хочете завершити роботу з програмою?",
                                "Так",
                                "Ні");

                            if (answer)
                            {
                                _isClosingConfirmed = true;
                                Current?.Quit();
                            }
                        }
                    };
                }
            };
#endif

            return window;
        }
    }
}