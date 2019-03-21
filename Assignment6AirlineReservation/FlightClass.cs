using Assignment6AirlineReservation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;        //bindingList
using System.Data;                  //DataSet
using System.Linq;
using System.Reflection;            //Method Info for try catch
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Flights
{
    class FlightClass
    {
        /// <summary>
        /// String that holds the database query.
        /// </summary>
        private string databaseQuery;
        /// <summary>
        /// Integer that represents the number of rows returned from our query
        /// </summary>
        private int numOfReturnVal = 0;
        /// <summary>
        /// Dataset ds. Imported with Using Systen.Data
        /// </summary>
        private DataSet ds = new DataSet();
        /// <summary>
        /// clsPassenger object
        /// </summary>
        private clsPassenger Passenger;
        /// <summary>
        /// clsDataAccess object -- Code provided by Shawn Cowder
        /// </summary>
        private clsDataAccess db = new clsDataAccess();
        /// <summary>
        /// ObservableCollection that holds all passengers in the boeing flight.
        /// </summary>
        private ObservableCollection<clsPassenger> boeingList;
        /// <summary>
        /// ObservableCollection that holds all passengers in the airbus flight
        /// </summary>
        private ObservableCollection<clsPassenger> airbusList;
        /// <summary>
        /// ObservableCollection holds all of the flights from the database.
        /// </summary>
        private ObservableCollection<string> flightList = new ObservableCollection<string>();


        /// <summary>
        /// Constructor -- populates the lists from our database
        /// Want to change for 6.2 to where I call in form 1 to populate
        /// This will make sure a list will add/remove and display properly. 
        /// </summary>
        public FlightClass()
        {
            PopulateBoeingList();
            PopulateAirbusList();
            ExecuteSQLFlightList();
        }

        /// <summary>
        /// Sets the query up for our database, executes our query, then populates our list with passengers.
        /// Used to populate all boeing passengers.
        /// </summary>
        public void PopulateBoeingList()
        {
            try
            {
                boeingList = new ObservableCollection<clsPassenger>();

                //This will pull all of the passengers on with flight 2 id
                databaseQuery = "SELECT Passenger.Passenger_ID, First_Name, Last_Name, Flight_ID, Seat_Number " +
                    "FROM Passenger, Flight_Passenger_Link " +
                    "WHERE Passenger.Passenger_ID = Flight_Passenger_Link.Passenger_ID AND " +
                    "Flight_id = 2";

                ds = db.ExecuteSQLStatement(databaseQuery, ref numOfReturnVal);
                GeneratePassengerList(boeingList);
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }


        }

        /// <summary>
        /// Sets the query up for our database, executes our query, then populates our list with passengers.
        /// Used to populate the airbus passengers list.
        /// </summary>
        public void PopulateAirbusList()
        {
            try
            {
               airbusList = new ObservableCollection<clsPassenger>();

                //This will pull all of the passengers on with flight 1 id
                databaseQuery = "SELECT Passenger.Passenger_ID, First_Name, Last_Name, Flight_ID, Seat_Number " +
                    "FROM Passenger, Flight_Passenger_Link " +
                    "WHERE Passenger.Passenger_ID = Flight_Passenger_Link.Passenger_ID AND " +
                    "Flight_id = 1";

                //qeries the database then calls to populate our list
                ds = db.ExecuteSQLStatement(databaseQuery, ref numOfReturnVal);
                GeneratePassengerList(airbusList);
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
        }

        /// <summary>
        /// Sets up query, then exucutes query to get all of the flights in the Database.
        /// </summary>
        private void ExecuteSQLFlightList()
        {
            try
            {
                //This will pull all of the passengers on with flight 2 id
                databaseQuery = "SELECT Flight.Flight_Number, Aircraft_Type " +
                    "FROM Flight ";

               
                ds = db.ExecuteSQLStatement(databaseQuery, ref numOfReturnVal);
                GenerateFlightList();
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
           
        }

        /// <summary>
        /// Quries the database for a passenger_ID that belongs to a name from the passener table.
        /// </summary>
        /// <param name="fName">Passengers first name that you want an id for</param>
        /// <returns>an integer that is the integer number</returns>
        public int GetPassengerID(string fName)
        {
            try
            {
                //This will pull all of the passengers on with flight 2 id
                databaseQuery = "SELECT Passenger.Passenger_ID " +
                    "FROM Passenger WHERE First_Name = '" + fName + "'";

                //Passenger id string
                string pID = "";
                ds = db.ExecuteSQLStatement(databaseQuery, ref numOfReturnVal);
                for (int i = 0; i < numOfReturnVal; i++)
                {
                    pID = ds.Tables[0].Rows[i][0].ToString();    
                }

                //parse string to int 
                int newpID;
                Int32.TryParse(pID, out newpID);

                return newpID;
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }

        }

        /// <summary>
        /// Deletes a passenger from the passenger and flight_Passenger_Link table 
        /// After a given id is given
        /// </summary>
        /// <param name="id">Passenger's id that is to be deleted</param>
        public void DeletePassenger(int id)
        {
            try
            {
                //deltes the from the link table first
                databaseQuery = "DELETE " +
                    "FROM Flight_Passenger_Link WHERE Passenger_ID = " + id + "";

                db.ExecuteNonQuery(databaseQuery);

                //then deletes the passenger
                databaseQuery = "DELETE " +
                    "FROM Passenger WHERE Passenger_ID = " + id + "";

                db.ExecuteNonQuery(databaseQuery);
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }

        }

        /// <summary>
        /// Adds a passenger's first and last name into the passenger table
        /// </summary>
        /// <param name="fName">Passenger's first name</param>
        /// <param name="lName">Passegner's last name</param>
        public void AddPassenger(string fName, string lName)
        {
            try
            {
                databaseQuery = "INSERT INTO Passenger (First_Name, Last_Name) " +
              "VALUES ('" + fName + "', '" + lName + "')";

                db.ExecuteNonQuery(databaseQuery);
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }

        }

        /// <summary>
        /// Updates a passengers seat
        /// </summary>
        /// <param name="oldSeat">old seat string</param>
        /// <param name="newSeat">new seat string</param>
        public void UpdatePassengerSeat(string oldSeat, string newSeat)
        {
            try
            {
                databaseQuery = "UPDATE Flight_Passenger_Link " +
               "SET Seat_Number = '" + newSeat + "' " +
               "WHERE Seat_Number = '" + oldSeat + "'";

                db.ExecuteNonQuery(databaseQuery);

            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
           
        }

        /// <summary>
        /// Adds a flight link for a given passenger 
        /// </summary>
        /// <param name="flightID">the flightID</param>
        /// <param name="passID">passenger's ID</param>
        /// <param name="seatNum">Seat number</param>
        public void AddFlightLink(int flightID, int passID, string seatNum)
        {
            try
            {
                databaseQuery = "INSERT INTO Flight_Passenger_Link (Flight_ID, Passenger_ID, Seat_Number) " +
                    "VALUES ('" + flightID + "', '" + passID + "', '" + seatNum + "')";

                db.ExecuteNonQuery(databaseQuery);
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
          
        }

        /// <summary>
        /// Getter that retrieves the beoing ObservableCollection
        /// </summary>
        /// <returns>beoing ObservableCollection</returns>
        public ObservableCollection<clsPassenger> GetBoeingList()
        {
            try
            {
                return boeingList;
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
           
        }

        /// <summary>
        /// Getter that returns the airbus ObservableCollection
        /// </summary>
        /// <returns>Airbus ObservableCollection</returns>
        public ObservableCollection<clsPassenger> GetAirbusList()
        {
            try
            {
                return airbusList;
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
        }

        /// <summary>
        /// Getter that returns all of the flights in our database
        /// </summary>
        /// <returns>flightList ObservableCollection</returns>
        public ObservableCollection<string> GetFlightList()
        {
            try
            {
                return flightList;

            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
        }

        /// <summary>
        /// Generates parses the return query and creates a new passenger for each row and adds them to our list.
        /// </summary>
        /// <param name="listName">Takes in a BindingList of type clsPassenger</param>
        private void GeneratePassengerList(ObservableCollection<clsPassenger> listName)
        {
            try
            {
                for (int i = 0; i < numOfReturnVal; i++)
                {
                    string pID = ds.Tables[0].Rows[i][0].ToString();
                    string pFirst = ds.Tables[0].Rows[i]["First_Name"].ToString();
                    string pLast = ds.Tables[0].Rows[i]["Last_Name"].ToString();
                    string pFlight = ds.Tables[0].Rows[i][3].ToString();
                    string pSeat = ds.Tables[0].Rows[i][4].ToString();

                    //Adds a new passenger for every row returned from the query.
                    Passenger = new clsPassenger(pID, pFirst, pLast, pSeat, pFlight);
                    listName.Add(Passenger);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
            
        }

        /// <summary>
        /// Generates the flightList BindingList after query to database.
        /// </summary>
        private void GenerateFlightList()
        {
            try
            {
                for (int i = 0; i < numOfReturnVal; i++)
                {
                    string fNumber = ds.Tables[0].Rows[i][0].ToString();
                    string fType = ds.Tables[0].Rows[i][1].ToString();

                    flightList.Add(fNumber + " - " + fType);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
            
        }



    }
}
