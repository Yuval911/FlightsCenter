using FlightCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace DBGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DBGeneratorVM vm = new DBGeneratorVM();

            DataContext = vm;

            vm.ErrorOccurred += OnErrorOccurred;
        }

        private void OnErrorOccurred(object source, ErrorOccurredEventArgs args)
        {
            MessageBox.Show(args.ErrorMessage);
        }
    }
}
