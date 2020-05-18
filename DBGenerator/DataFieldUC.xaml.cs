using FlightCenter;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace DBGenerator
{
    /// <summary>
    /// Interaction logic for DataFieldUC.xaml
    /// </summary>
    public partial class DataFieldUC : UserControl, INotifyPropertyChanged
    {
        public DataFieldUC()
        {
            InitializeComponent();

            DataContext = this;
        }

        public string FieldLabel { get; set; }

        public string ItemsTypeLabel { get; set; }

        private int currentItemsNumber;
        public int CurrentItemsNumber
        {
            get
            {
                return currentItemsNumber;
            }
            set
            {
                currentItemsNumber = value;
                OnPropertyChanged("CurrentItemsNumber");
            }
        }

        private bool isInvalidItemsNum;
        public bool IsInvalidItemsNum
        {
            get
            {
                return isInvalidItemsNum;
            }
            set
            {
                isInvalidItemsNum = value;
                OnPropertyChanged("IsInvalidItemsNum");
            }
        }

        public int MaxItems { get; set; }

        private int itemsNum;
        public int ItemsNum
        {
            get
            {
                return itemsNum;
            }
            set
            {
                itemsNum = value;
                if (itemsNum > MaxItems || itemsNum < 0)
                    IsInvalidItemsNum = true;
                else
                    IsInvalidItemsNum = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
