using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Input;
using System.Threading;
using System.Threading.Tasks;
using System;
using Avalonia.Threading;

namespace Pong
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            this.KeyDown += MainWindow_KeyDown;
            this.KeyUp += MainWindow_KeyUp;
            this.PositionChanged += (s, e) => ((PongViewModel)DataContext).Pause();
        }

        private async void MainWindow_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up || e.Key == Key.W)
            {
                ((PongViewModel)DataContext).GameStarted = true;
                ((PongViewModel)DataContext).PlayerUp = true;
                ((PongViewModel)DataContext).PlayerDown = false;
            }
            else if (e.Key == Key.Down || e.Key == Key.S)
            {
                ((PongViewModel)DataContext).GameStarted = true;
                ((PongViewModel)DataContext).PlayerUp = false;
                ((PongViewModel)DataContext).PlayerDown = true;
            }
            else if (e.Key == Key.Escape)
            {
                if (((PongViewModel)DataContext).GameStarted)
                    ((PongViewModel)DataContext).Pause();
                else
                    ((PongViewModel)DataContext).GameStarted = true;
            }
        }
        private async void MainWindow_KeyUp(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up || e.Key == Key.W)
            {
                ((PongViewModel)DataContext).PlayerUp = false;
            }
            else if (e.Key == Key.Down || e.Key == Key.S)
            {
                ((PongViewModel)DataContext).PlayerDown = false;
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            
            this.DataContext = new PongViewModel();
        }
    }
}
