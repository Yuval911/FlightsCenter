using FlightCenter;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DBGenerator
{
    /// <summary>
    /// This is the view model class of the generator window.
    /// </summary>
    class DBGeneratorVM : INotifyPropertyChanged
    {
        // All data fields were created as User Controls.

        public DataFieldUC CustomersField { get; set; } = new DataFieldUC()
        {
            FieldLabel = "Number of customers:",
            MaxItems = 1000,
            ItemsTypeLabel = "customers"
        };

        public DataFieldUC AirlinesField { get; set; } = new DataFieldUC()
        {
            FieldLabel = "Number of airline companies:",
            MaxItems = 500,
            ItemsTypeLabel = "airlines"
        };
        
        public DataFieldUC FlightsField { get; set; } = new DataFieldUC()
        {
            FieldLabel = "Flights per airline company:",
            MaxItems = 1000,
            ItemsTypeLabel = "flights"
        };

        public DataFieldUC TicketsField { get; set; } = new DataFieldUC()
        {
            FieldLabel = "Tickets per customer:",
            MaxItems = 10,
            ItemsTypeLabel = "tickets"
        };

        // Properties monitored by the UI.

        private string log;
        public string Log
        {
            get
            {
                return log;
            }
            set
            {
                log = value;
                OnPropertyChanged("Log");
            }
        }

        public bool DeleteDBFirst { get; set; }

        private bool isProccessing;
        public bool IsProccessing
        {
            get
            {
                return isProccessing;
            }
            set
            {
                isProccessing = value;
                OnPropertyChanged("IsProccessing");
                AddToDBCommand.RaiseCanExecuteChanged();
            }
        }

        private string proccessingMessage;
        public string ProccessingMessage
        {
            get
            {
                return proccessingMessage;
            }
            set
            {
                proccessingMessage = value;
                OnPropertyChanged("ProccessingMessage");
            }
        }

        /// <summary>
        /// The constructor initializes all the needed properties.
        /// </summary>
        public DBGeneratorVM()
        {
            AddToDBCommand = new DelegateCommand(AddItemsToDatabase, CanExecute);

            Log = "";
            ProccessingMessage = "";
            DeleteDBFirst = false;
            IsProccessing = false;

            InitializeDataCount();

            CustomersField.PropertyChanged += FieldChangedEventHandler;
            AirlinesField.PropertyChanged += FieldChangedEventHandler;
            FlightsField.PropertyChanged += FieldChangedEventHandler;
            TicketsField.PropertyChanged += FieldChangedEventHandler;
        }

        private void InitializeDataCount()
        {
            Task.Run(() =>
            {
                TestFacade facade = new TestFacade();

                CustomersField.CurrentItemsNumber = facade.GetAllCustomers().Count;
                AirlinesField.CurrentItemsNumber = facade.GetAllAirlineCompanies().Count;
                FlightsField.CurrentItemsNumber = facade.GetAllFlights().Count;
                TicketsField.CurrentItemsNumber = facade.GetAllTickets().Count;
            });
        }

        /// <summary>
        /// Every time a field was changed, the "AddToDBCommand" command will determine if it's should be enabled or not.
        /// </summary>
        private void FieldChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            AddToDBCommand.RaiseCanExecuteChanged();
        }

        public DelegateCommand AddToDBCommand { get; set; }

        /// <summary>
        /// This logics defines how the "AddToDBCommand" command enables or disables its operation.
        /// </summary>
        private bool CanExecute()
        {
            if (CustomersField.IsInvalidItemsNum || AirlinesField.IsInvalidItemsNum || FlightsField.IsInvalidItemsNum || TicketsField.IsInvalidItemsNum)
                return false;

            else if (IsProccessing == true)
                return false;

            else
                return true;
        }
        
        /// <summary>
        /// Generating and adding the data to the database.
        /// </summary>
        private void AddItemsToDatabase()
        {
            TestFacade facade = new TestFacade();

            int customersSum = CustomersField.CurrentItemsNumber + CustomersField.ItemsNum;
            int airlinesSum = AirlinesField.CurrentItemsNumber + AirlinesField.ItemsNum;
            int flightsSum = FlightsField.CurrentItemsNumber + FlightsField.ItemsNum;
            int ticketsSum = TicketsField.CurrentItemsNumber + TicketsField.ItemsNum;

            if (airlinesSum * flightsSum < TicketsField.ItemsNum)
            {
                OnErrorOccured($"Total number of flights ({airlinesSum * flightsSum}) cannot be smaller than tickets per customer number ({TicketsField.ItemsNum}).");
                return;
            }
            else if (customersSum == 0 && TicketsField.ItemsNum > 0)
            {
                OnErrorOccured($"Cannot add tickets while there are no customers.");
                return;
            }
            
            else if (airlinesSum == 0 && FlightsField.ItemsNum > 0)
            {
                OnErrorOccured($"Cannot add flights while there are no airline companies.");
                return;
            }
            
            Task.Run(() =>
            {
                IsProccessing = true;

                ProccessingMessageAnimation();

                int totalItems = CustomersField.ItemsNum + AirlinesField.ItemsNum + FlightsField.ItemsNum * AirlinesField.ItemsNum +
                                 CustomersField.ItemsNum * TicketsField.ItemsNum;
                int succeeded = 0;

                if (DeleteDBFirst)
                {
                    facade.DeleteAllTables();
                    Log += $"Removed all items from the database.\n";
                }

                try
                {
                    IList<Customer> customers = DataGenerator.GetRandomCustomersList(CustomersField.ItemsNum);
                    
                    if (CustomersField.ItemsNum > 0)
                    {
                        facade.AddRangeOfCustomers(customers);
                        Log += $"Added {CustomersField.ItemsNum} customers to the database.\n";
                        succeeded += CustomersField.ItemsNum;
                        CustomersField.CurrentItemsNumber += CustomersField.ItemsNum;
                    }
                }
                catch (Exception e)
                {
                    Log += $"An error has occurred while adding customers to the database. Exception: {e.Message}\n";
                }
            
                try
                {
                    IList<AirlineCompany> airlines = DataGenerator.GetRandomAirlineCompaniesList(AirlinesField.ItemsNum);
                    if (AirlinesField.ItemsNum > 0)
                    {
                        facade.AddRangeOfAirlineCompanies(airlines);
                        Log += $"Added {AirlinesField.ItemsNum} airline companies to database.\n";
                        succeeded += AirlinesField.ItemsNum;
                        AirlinesField.CurrentItemsNumber += AirlinesField.ItemsNum;
                    }
                }
                catch (Exception e)
                {
                    Log += $"An error has occurred while adding airline companies to the database. Exception: {e.Message}\n";
                }

                try
                {
                    IList<Flight> flights = DataGenerator.GetRandomFlights(FlightsField.ItemsNum);
                    if (FlightsField.ItemsNum > 0)
                    {
                        facade.AddRangeOfFlights(flights);
                        Log += $"Added {FlightsField.ItemsNum} flights for each airline company to the database (Total: {FlightsField.ItemsNum * airlinesSum}).\n";
                        succeeded += FlightsField.ItemsNum * AirlinesField.ItemsNum;
                        FlightsField.CurrentItemsNumber += FlightsField.ItemsNum;
                    }
                }
                catch (Exception e)
                {
                    Log += $"An error has occurred while adding flights to the database. Exception: {e.Message} # {e.InnerException.Message} # {e.InnerException.InnerException.Message}\n";
                }

                try
                {
                    IList<Ticket> tickets = DataGenerator.GetRandomTickets(TicketsField.ItemsNum);
                    if (TicketsField.ItemsNum > 0)
                    {
                        facade.AddRangeOfTickets(tickets);
                        Log += $"Added {TicketsField.ItemsNum} tickets for each customer to the database (Total: {TicketsField.ItemsNum * customersSum}).\n";
                        succeeded += TicketsField.ItemsNum * CustomersField.ItemsNum;
                        TicketsField.CurrentItemsNumber += TicketsField.ItemsNum;
                    }
                }
                catch (Exception e)
                {
                    Log += $"An error has occurred while adding tickets to the database. Exception: {e.Message}\n";
                }

                if (succeeded == totalItems && succeeded > 0)
                    Log += $"** Added successfully {succeeded} items to the database! (100% success). **\n";
                else if (totalItems > 0)
                    Log += $"** Added {succeeded} out of {totalItems} items to the database ({(succeeded / totalItems) * 100}% success). **\n";
                else
                    Log += "No items has been added to the database.\n";

                IsProccessing = false;
            });
        }

        private void ProccessingMessageAnimation()
        {
            Task.Run(() =>
            {
                while (IsProccessing)
                {
                    ProccessingMessage = "Proccessing.";
                    Thread.Sleep(200);
                    ProccessingMessage = "Proccessing..";
                    Thread.Sleep(200);
                    ProccessingMessage = "Proccessing...";
                    Thread.Sleep(200);
                    ProccessingMessage = "";
                }           
            });
        }

        public event EventHandler<ErrorOccurredEventArgs> ErrorOccurred;

        private void OnErrorOccured(string errorMessage)
        {
            ErrorOccurred?.Invoke(this, new ErrorOccurredEventArgs(errorMessage));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }

    // Event args class

    class ErrorOccurredEventArgs : EventArgs
    {
        public ErrorOccurredEventArgs(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public string ErrorMessage { get; }
    }
}
