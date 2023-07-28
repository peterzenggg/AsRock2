using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        UIHelper Helper;
        DispatcherTimer timer;



        public MainWindow()
        {
            InitializeComponent();
            Helper = UIHelper.Instance;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += _timer_Tick;
            timer.Start();
            CH2TargetLed.Text = Helper.LedController.GetMaxLed(2);
            CH6TargetLed.Text = Helper.LedController.GetMaxLed(6);
            CH2MaxLed.Content = Helper.LedController.GetMaxLed(2);
            CH6MaxLed.Content = Helper.LedController.GetMaxLed(6);
        }
        void _timer_Tick(object sender, EventArgs e)
        {
            CPUTemp.Content = Helper.FanController.CpuTemp.ToString();
            MBTemp.Content = Helper.FanController.MBTemp.ToString();
            CPUFan1Speed.Content = Helper.FanController.CpuFan1Speed.ToString();
            CPUFan2Speed.Content = Helper.FanController.CpuFan2Speed.ToString();
            CH1Speed.Content = Helper.FanController.ChassisFan1Speed.ToString();
            CH2Speed.Content = Helper.FanController.ChassisFan2Speed.ToString();
            CH3Speed.Content = Helper.FanController.ChassisFan3Speed.ToString();
            CH4Speed.Content = Helper.FanController.ChassisFan4Speed.ToString();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Helper.Dispose();
        }

        private void CPUFan1Apply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int Target = Convert.ToInt16(CPU1Fan1Targer.Text);
                Helper.FanController.SetFanConfig(ESCORE_FAN_ID.ESCORE_FANID_CPU_FAN1, Target);
            }
            catch
            {
                
            }
        }

        private void CPUFan2Apply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int Target = Convert.ToInt16(CPU1Fan2Targer.Text);
                Helper.FanController.SetFanConfig(ESCORE_FAN_ID.ESCORE_FANID_CPU_FAN2, Target);
            }
            catch
            {

            }
        }

        private void CH1Apply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int Target = Convert.ToInt16(CH1Targer.Text);
                Helper.FanController.SetFanConfig(ESCORE_FAN_ID.ESCORE_FANID_CHASSIS_FAN1, Target);
            }
            catch
            {

            }

        }

        private void CH2Apply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int Target = Convert.ToInt16(CH2Targer.Text);
                Helper.FanController.SetFanConfig(ESCORE_FAN_ID.ESCORE_FANID_CHASSIS_FAN2, Target);
            }
            catch
            {

            }
        }

        private void CH3Apply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int Target = Convert.ToInt16(CH3Targer.Text);
                Helper.FanController.SetFanConfig(ESCORE_FAN_ID.ESCORE_FANID_CHASSIS_FAN3, Target);
            }
            catch
            {

            }
        }

        private void CH4Apply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int Target = Convert.ToInt16(CH4Targer.Text);
                Helper.FanController.SetFanConfig(ESCORE_FAN_ID.ESCORE_FANID_CHASSIS_FAN4, Target);
            }
            catch
            {

            }
        }

        private void CH2LedApply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int Target = Convert.ToInt16(CH2TargetLed.Text);
                Helper.LedController.ChangeLight(2, Target);
            }
            catch 
            {

            }

        }

        private void CH6LedApply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int Target = Convert.ToInt16(CH6TargetLed.Text);
                Helper.LedController.ChangeLight(6, Target);
            }
            catch
            {

            }
        }


        private void CH2LedCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selecd = CH2LedCombobox.SelectedIndex;
            if (selecd >= 0 && selecd < CH2LedCombobox.Items.Count)
            {
                Helper?.LedController?.ChnageChMode(2,(RockMode)selecd);
            }
        }

        private void CH6LedCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selecd = CH6LedCombobox.SelectedIndex;
            if (selecd >= 0 && selecd < CH6LedCombobox.Items.Count)
            {
                Helper?.LedController?.ChnageChMode(6, (RockMode)selecd);
            }
        }
    }
}
