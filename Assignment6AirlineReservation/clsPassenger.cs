using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Flights
{
    class clsPassenger
    {
        /// <summary>
        /// Holds the passenger ID for the passsenger
        /// </summary>
        private string passengerID;
        /// <summary>
        /// Holds the passenger first name for the passsenger
        /// </summary>
        private string passengerFirstName;
        /// <summary>
        /// Holds the passenger last name for the passsenger
        /// </summary>
        private string passengerLastName;
        /// <summary>
        /// Holds the passenger seat number for the passsenger
        /// </summary>
        private string passengerSeat;
        /// <summary>
        /// Holds the passenger seat for the passsenger
        /// </summary>
        private string passengerFlight;

        /// <summary>
        /// Ctor for passenger
        /// </summary>
        /// <param name="pID"></param>
        /// <param name="pFirst"></param>
        /// <param name="pLast"></param>
        /// <param name="pSeat"></param>
        /// <param name="pFlight"></param>
        public clsPassenger(string pID, string pFirst, string pLast, string pSeat, string pFlight)
        {
            this.passengerID = pID;
            this.passengerFirstName = pFirst;
            this.passengerLastName = pLast;
            this.passengerSeat = pSeat;
            this.passengerFlight = pFlight;
        }

        /// <summary>
        /// returns the passenger ID
        /// </summary>
        /// <returns>returns a string of passenger ID</returns>
        public string GetPassengerID()
        {
            try
            {
                return passengerID;
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
            
        }

        /// <summary>
        /// returns the passenger firstname
        /// </summary>
        /// <returns>returns a string of the first NAme</returns>
        public string GetPassengerFirstName()
        {
            try
            {
                return passengerFirstName;
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
           
        }

        /// <summary>
        /// returns the passenger last Naem
        /// </summary>
        /// <returns>returns a string of passenger last name</returns>
        public string GetPassengerLastName()
        {
            try
            {
                return passengerLastName;
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
            
        }

        /// <summary>
        /// returns the passenger seat number
        /// </summary>
        /// <returns>returns a string of passenger seat</returns>
        public string GetPassengerSeat()
        {
            try
            {
                return passengerSeat;
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }

        }

        /// <summary>
        /// returns the passenger flight
        /// </summary>
        /// <returns>returns a string of passenger flight</returns>
        public string GetPassengerFlight()
        {
            try
            {
                return passengerFlight;

            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
        }

        /// <summary>
        /// returns an overloaded toString of object passenger
        /// </summary>
        /// <returns>returns first and last name of the passenger</returns>
        public override string ToString()
        {
            try
            {
                return passengerFirstName + " " + passengerLastName;

            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
        }
    }
}
