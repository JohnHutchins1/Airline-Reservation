using Flights;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Collections;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace Assignment6AirlineReservation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// holds the wndAddPassenger form globally
        /// </summary>
        private wndAddPassenger wndAddPass;
        /// <summary>
        /// Holds the reference to the previously used varible, used to change seat and delete passengers
        /// </summary>
        private Label prevLabel;
        /// <summary>
        /// Used to tell which of the two panels is active
        /// </summary>
        private Canvas activeSeatCanvas;
        /// <summary>
        /// holds the flight ID globally
        /// </summary>
        private int flightID;
        /// <summary>
        /// Holds the passengerName globally
        /// </summary>
        private string passengerName;
        /// <summary>
        /// Holds the passenger ID which is used to query a new passenger into the DB
        /// </summary>
        private int passID;
        /// <summary>
        /// Booleon value that tell the code if we are in the process of adding a pasenger seat to our db
        /// </summary>
        private bool addingSeat = false;
        /// <summary>
        /// Boolean value that tells code if we are in the process of changing seats
        /// </summary>
        private bool changingSeat = false;
        /// <summary>
        /// Holds the old seat number given from a label.Content
        /// </summary>
        private string oldSeat = "";
        /// <summary>
        /// hold and initializes the flightClass object
        /// </summary>
        private FlightClass fClass = new FlightClass();
        /// <summary>
        /// BindingList that holds all passengers in the boeing flight.
        /// </summary>
        private ObservableCollection<clsPassenger> boeingList = new ObservableCollection<clsPassenger>();
        /// <summary>
        /// BindingList that holds all passengers in the airbus flight
        /// </summary>
        private ObservableCollection<clsPassenger> airbusList = new ObservableCollection<clsPassenger>();
        /// <summary>
        /// holds all of the flights from the database.
        /// </summary>
        private ObservableCollection<string> flightList = new ObservableCollection<string>();

        /// <summary>
        /// Constructor for main window
        /// </summary>
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

                //initializes flight list
                flightList = fClass.GetFlightList();
                cbChooseFlight.ItemsSource = flightList;

                //disables all controls other than choose flight combobox
                DisableAllControls();
                cbChooseFlight.IsEnabled = true;

                //hiding both panels until the user selects desired panel
                CanvasA380.Visibility = Visibility.Hidden;
                Canvas767.Visibility = Visibility.Hidden;

            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                    MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }

        /// <summary>
        /// Chooses the proper flight according to the selected index
        /// called everytime the choose flight combobox index is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbChooseFlight_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                int selection = cbChooseFlight.SelectedIndex;
                lblPassengersSeatNumber.Content = "";

                //if 1, then load the boeing. 0 is airbus
                if (selection == 1)
                {
                    //hide Airbus, show boeing
                    CanvasA380.Visibility = Visibility.Hidden;
                    Canvas767.Visibility = Visibility.Visible;
                    activeSeatCanvas = c767_Seats;
                    flightID = 2;

                    //hide all controls other than the addpassenger button.
                    EnableAllControls();
                    cmdChangeSeat.IsEnabled = false;
                    cmdDeletePassenger.IsEnabled = false;

                    PopulateBoeingColors();
                }
                else
                {
                    //show airbus, hide boeing
                    Canvas767.Visibility = Visibility.Hidden;
                    CanvasA380.Visibility = Visibility.Visible;
                    activeSeatCanvas = cA380_Seats;
                    flightID = 1;


                    //hide all controls other than the addpassenger button.
                    EnableAllControls();
                    cmdChangeSeat.IsEnabled = false;
                    cmdDeletePassenger.IsEnabled = false;

                    PopulateAirbusColors();
                }

                
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                    MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }

        /// <summary>
        /// Starts the process to add a new passenger
        /// Opens a new wpf form when clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdAddPassenger_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                wndAddPass = new wndAddPassenger();
                wndAddPass.ShowDialog();

                //gets the passenger first name from the wndAddPass wpf form
                passengerName = wndAddPass.txtFirstName.Text;
                
                //only hits if the user clicked on save with correct information
                if(passengerName != "")
                {
                    //globally set the passengerID so we can query it into the link table when the user clicks a tile
                    passID = fClass.GetPassengerID(passengerName);
                    addingSeat = true;

                    //refreshing list
                    if (activeSeatCanvas == c767_Seats)
                    {
                        PopulateBoeingColors();
                    }
                    else
                    {
                        PopulateAirbusColors();
                    }

                    //disable all form except for addPassenger BUtton
                    DisableAllControls();
                }
               
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                    MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }

        /// <summary>
        /// Chooses the proper passenger depending on which one was selected.
        /// Highlights the selected passenger in the box and updates the passengerSeat
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbChoosePassenger_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                cmdDeletePassenger.IsEnabled = true;
                cmdChangeSeat.IsEnabled = true;

                //cast object into combobox
                ComboBox cbo = (ComboBox)sender;
                //initialize passenger with first passenger in list
                clsPassenger Passenger = (clsPassenger)cbChoosePassenger.Items[0];

                //used to change back the previous label back to red after being selected
                if (prevLabel != null)
                {
                    prevLabel.Background = Brushes.Red;
                }

                //loop through the passenger combo box to find the selected passenger
                for (int i = 0; i < cbChoosePassenger.Items.Count; i++)
                {
                    //Extract the passenger from the cbo
                    if (i == cbo.SelectedIndex)
                    {
                        Passenger = (clsPassenger)cbChoosePassenger.Items[i];
                    }

                }

                //temp string that holds the seatNumber
                string seatString;
                //loops through the active panel and finds the passenger's seat's label
                foreach (var lb in activeSeatCanvas.Children.OfType<System.Windows.Controls.Label>())
                {
                    Label MyLabel = (Label)lb;

                    //appending to find the proper seat
                    if(activeSeatCanvas == c767_Seats)
                    {
                        seatString = "Seat" + Passenger.GetPassengerSeat();
                    }else
                    {
                        seatString = "SeatA" + Passenger.GetPassengerSeat();
                    }

                    //highlighting the selected passenger
                    if (MyLabel.Name == seatString)
                    {
                        prevLabel = MyLabel;
                        MyLabel.Background = Brushes.Lime;
                        lblPassengersSeatNumber.Content = Passenger.GetPassengerSeat();
                    
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
        }  

        /// <summary>
        /// On left mouse click and if the label is red, it will highlight the passenger label
        /// Will also change the passenger box to the appropriate passenger
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SpaceClick_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                cmdDeletePassenger.IsEnabled = true;
                cmdChangeSeat.IsEnabled = true;

                /// <summary>
                /// MyLabel holds the label clicked on
                /// </summary>
                Label MyLabel = (Label)sender;
                /// <summary>
                /// holds the seat number (MyLabel.Text)
                /// </summary>
                string seatNumber;
                /// <summary>
                /// declaring a clsPassenger object to later cast into.
                /// </summary>
                clsPassenger Passenger;

                //executed if we are in the process of adding a seat to a new passenger
                if (addingSeat && MyLabel.Background != Brushes.Red)
                {
                    //Here I link the new passenger record to the link_passenger table
                    fClass.AddFlightLink(flightID, passID, MyLabel.Content.ToString());

                    passengerName = "";
                    addingSeat = false;

                    if (activeSeatCanvas == c767_Seats)
                    {
                        PopulateBoeingColors();
                    }
                    else
                    {
                        PopulateAirbusColors();
                    }

                    EnableAllControls();
                   
                }

                //If we are in the process of changing the seat
                if (changingSeat && MyLabel.Background != Brushes.Red)
                {

                    ChangeNewSeat(MyLabel.Content.ToString());
                    prevLabel = null;

                    EnableAllControls();

                }

                //If we haven't clicked on more than 1 label
                //then don't do anything but if we have, change the
                //Other clicked on label back to red
                if (prevLabel != null)
                {
                    prevLabel.Background = Brushes.Red;
                }

                //if the backColor is red, then change it green
                if (MyLabel.Background == Brushes.Red)
                {
                    prevLabel = MyLabel;
                    //Turn the seat green
                    MyLabel.Background = Brushes.Lime;
    
                    //loops through the passenger combobox and gets checks which of the
                    //passengers sits in the clicked on red label.
                    for (int i = 0; i < cbChoosePassenger.Items.Count; i++)
                    {

                        //Extract the passenger from the cbo
                        Passenger = (clsPassenger)cbChoosePassenger.Items[i];

                        if (activeSeatCanvas == c767_Seats)
                        {
                            seatNumber = "Seat" + Passenger.GetPassengerSeat();
                        }
                        else
                        {
                            seatNumber = "SeatA" + Passenger.GetPassengerSeat();
                        }

                        //If the seat number then select the passenger in the combo box
                        if (seatNumber == MyLabel.Name)
                        {
                            cbChoosePassenger.SelectedIndex = i;
                            lblPassengersSeatNumber.Content = Passenger.GetPassengerSeat();
                        }
                    }
                }

                EnableAllControls();
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }

        }

        /// <summary>
        /// This will delete a passenger after prompted.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmdDeletePassenger_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //holds passenger object
                clsPassenger pass;
                //holds the passsengers first name
                string passFName = "";

                //Loop through to find the passenger that sits in the seat
                for (int i = 0; i < cbChoosePassenger.Items.Count; i++)
                {
                    pass = (clsPassenger)cbChoosePassenger.Items[i];
                    if (prevLabel.Content.ToString() == pass.GetPassengerSeat())
                    {
                        passFName = pass.GetPassengerFirstName();
                    }
                }

                prevLabel = null;

                //Prompts user if they really want to delete the passenger
                MessageBoxResult mbResult = MessageBox.Show("Do you wish to delete this passenger, this process cannot be undone",
                    "Delete Passenger", MessageBoxButton.YesNo);

                if (mbResult == MessageBoxResult.Yes)
                {
                    //get the passenger id and delete individual
                    int id = fClass.GetPassengerID(passFName);
                    fClass.DeletePassenger(id);
                }

                //repopulating the appropriate comboboxes and getting updated lists.
                if (activeSeatCanvas == c767_Seats)
                {
                    PopulateBoeingColors();
                }
                else
                {
                    PopulateAirbusColors();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
           
        }

        /// <summary>
        /// Gets a new list object and assigns it to the airbusList
        /// Then repopulates the label colors
        /// </summary>
        private void PopulateAirbusColors()
        {
            try
            {
                fClass.PopulateAirbusList();
                airbusList = fClass.GetAirbusList();
                cbChoosePassenger.ItemsSource = airbusList;

                foreach (var lb in activeSeatCanvas.Children.OfType<System.Windows.Controls.Label>())
                {
                    Label MyLabel = (Label)lb;
                    MyLabel.Background = Brushes.Blue;
                }

                foreach (var lb in activeSeatCanvas.Children.OfType<System.Windows.Controls.Label>())
                {
                    Label MyLabel = (Label)lb;

                    foreach (clsPassenger pass in airbusList)
                    {
                        string passSeat = "SeatA" + pass.GetPassengerSeat();
                        if (passSeat == MyLabel.Name)
                        {
                            MyLabel.Background = Brushes.Red;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
        }

        /// <summary>
        /// Gets a new list object and assigns it to the boeing list
        /// Then repopulates the label colors
        /// </summary>
        private void PopulateBoeingColors()
        {
            try
            {
                fClass.PopulateBoeingList();
                boeingList = fClass.GetBoeingList();
                cbChoosePassenger.ItemsSource = boeingList;

                foreach (var lb in activeSeatCanvas.Children.OfType<System.Windows.Controls.Label>())
                {
                    Label MyLabel = (Label)lb;
                    MyLabel.Background = Brushes.Blue;
                }

                foreach (var lb in activeSeatCanvas.Children.OfType<System.Windows.Controls.Label>())
                {
                    Label MyLabel = (Label)lb;

                    foreach (clsPassenger pass in boeingList)
                    {
                        //forming the passenger seat name
                        string passSeat = "Seat" + pass.GetPassengerSeat();
                        if (passSeat == MyLabel.Name)
                        {
                            MyLabel.Background = Brushes.Red;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
            
        }

        /// <summary>
        /// registers the first button click and then assigns that seat to the assenger
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmdChangeSeat_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //holds the passenger object
                clsPassenger pass;

                //Loop through to find the passenger that sits in the seat
                for (int i = 0; i < cbChoosePassenger.Items.Count; i++)
                {
                    pass = (clsPassenger)cbChoosePassenger.Items[i];
                    if (prevLabel.Content.ToString() == pass.GetPassengerSeat())
                    {
                        oldSeat = pass.GetPassengerSeat();
                    }
                }

                changingSeat = true;

                //disable all form except for labels
                DisableAllControls();
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
            
        }

        /// <summary>
        /// registers second button click on the form and changes the passenger seat
        /// </summary>
        /// <param name="newSeat"></param>
        private void ChangeNewSeat(string newSeat)
        {
            try
            {
                //MessageBox.Show("Old seat: " + oldSeat + " New Seat: " + newSeat);
                fClass.UpdatePassengerSeat(oldSeat, newSeat);

                changingSeat = false;
                prevLabel.Background = Brushes.Blue;

                //repopulating the appropriate comboboxes and getting updated lists.
                if (activeSeatCanvas == c767_Seats)
                {
                    PopulateBoeingColors();
                }
                else
                {
                    PopulateAirbusColors();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
            
        }

        /// <summary>
        /// Disables the combobox and buttons in the main wpf
        /// </summary>
        private void DisableAllControls()
        {
            try
            {
                cbChooseFlight.IsEnabled = false;
                cbChoosePassenger.IsEnabled = false;
                cbChooseFlight.IsDropDownOpen = false;
                cbChoosePassenger.IsDropDownOpen = false;

                cmdAddPassenger.IsEnabled = false;
                cmdChangeSeat.IsEnabled = false;
                cmdDeletePassenger.IsEnabled = false;
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }

        }

        /// <summary>
        /// Enables the combobox and buttons in the main wpf
        /// </summary>
        private void EnableAllControls()
        {
            try
            {
                cmdAddPassenger.IsEnabled = true;
                cmdChangeSeat.IsEnabled = true;
                cmdDeletePassenger.IsEnabled = true;

                cbChooseFlight.IsEnabled = true;
                cbChoosePassenger.IsEnabled = true;
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }

        }


        /// <summary>
        /// Handles errors of code, helps out try catches
        /// </summary>
        /// <param name="sClass"></param>
        /// <param name="sMethod"></param>
        /// <param name="sMessage"></param>
        private void HandleError(string sClass, string sMethod, string sMessage)
        {
            try
            {
                MessageBox.Show(sClass + "." + sMethod + " -> " + sMessage);
            }
            catch (System.Exception ex)
            {
                System.IO.File.AppendAllText(@"C:\Error.txt", Environment.NewLine + "HandleError Exception: " + ex.Message);
            }
        }

        

        
    }

    
}
