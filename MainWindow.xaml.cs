using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Management;

namespace RemoveJava
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer _dispatcherTimer = new DispatcherTimer();
        private Thread _thread;

        public MainWindow()
        {
            InitializeComponent();
        }


        private void findJavaSoftware()
        {
            try
            {
                var mos =
                  new ManagementObjectSearcher("SELECT * FROM Win32_Product WHERE Name Like '%Java%Update%'");
                foreach (ManagementObject mo in mos.Get())
                {
                    try
                    {
                        var ProgramName = mo["Name"].ToString();
                        if (MessageBox.Show("Do you want to uninstall and remove " + ProgramName + "?", "Confirm uninstall", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            try
                            {
                                    var hr = mo.InvokeMethod("Uninstall", null);
                                    MessageBox.Show("Uninstalled and removed " + ProgramName);
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("Could not uninstall or remove " + ProgramName);

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //this program may not have a name property
                    }
                }
                MessageBox.Show("Uninstalled all confirmed Java components", "Success");

            }
            catch (Exception ex)
            {
            }
        }

        private void btnDeleteJava_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            lblPleaseWait.Visibility = Visibility.Visible;
            this.Title = "RemoveJava.com - the program is working";
            
            _thread = new Thread(findJavaSoftware);
            _thread.Start();
            _dispatcherTimer.Tick += new EventHandler(DeleteJavaDispatcherTimerTick);
            _dispatcherTimer.Interval = new TimeSpan(100);
            _dispatcherTimer.Start();
        }

        private void DeleteJavaDispatcherTimerTick(object sender, EventArgs e)
        {
            if (!_thread.IsAlive)
            {
                this.IsEnabled = true;
                lblPleaseWait.Visibility = Visibility.Hidden;
                this.Title = "RemoveJava.com";
                _dispatcherTimer.Tick -= new EventHandler(DeleteJavaDispatcherTimerTick);
                _dispatcherTimer.Stop();
            }
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(_thread != null)
            {
                _thread.Interrupt();
                _thread.Abort();
            }
        }
    }
}
