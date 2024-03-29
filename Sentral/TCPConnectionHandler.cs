﻿using ClassLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sentral
{

    public delegate void NewAccessEntryTry(AccessEntryTry x);
    public delegate void AlarmRaised(AlarmLogEntry x);
    public class TCPConnectionHandler
    {
        public static MainActivity Mainform;                                                                            
        public static Socket comsocket;
        public static event NewAccessEntryTry newAccessEntryTry;
        public static event AlarmRaised AlarmRaised;

        public TCPConnectionHandler(MainActivity x, Socket y)
        {
            /*
             * connectionhandler handles the connection request on tcp pipe on separate thread
             */

            Mainform = x; //passes singleton handle to mainform
            comsocket = y;
            
            ThreadStart ts = new ThreadStart(ConnectionHandler);    //makes a thread start object based on ConnectionHandler
            Thread TCPThread = new Thread(ts);                      //passes thread start object to a thread
            TCPThread.Name = "TCPServerThread";                     //sets the thread name
            TCPThread.IsBackground = true;
            TCPThread.Start();                                      //starts the tcpserver
        }
        /*****************************************************TCPCLIENT*******************************************************/
        public static async void ConnectionHandler()
        {
            /* 
            * The TCPsever in primarely built around what is received from the tcp pipe, this is done by having a constant while loop
            * listeneing to any incomming traffic on connected comsocket.
            * In all the communcation between server and client there is package identifiers in front of receive string that
            * lets us easily identify what class to deserialize the json object into.
            */
            Socket ComSocket = comsocket;
            bool error = false;
            bool complete = false;
            Messages.SendString(ComSocket, PackageIdentifier.ServerACK, out error);                             //sends a acknowledge that connection is established

            while (ComSocket.Connected)                                                                         //loops this function aslong as the connection is open
            {
                if (ComSocket.Available > 0)                                                                    // receive data from connected socket if available
                {
                    string receivedString = Messages.ReceiveString(ComSocket, out error);                       //gets the data available on ComSocket
                    string packageID = Messages.GetPackageIdentifier(ref receivedString, out receivedString);   //each datapacket contains a 6 digit identifier
                    User user = new User();                                                                     //getpackageidentifer isolates the identifer
                    switch (packageID)                                                                          //this identifier is passes to switch 
                    {                                                                                           //identifiers are hardcoded in class.cs 
                        case PackageIdentifier.AlarmEvent:
                            AlarmEvent alarmEvent = JsonSerializer.Deserialize<AlarmEvent>(receivedString);    //deserializes the json string into class
                            Task<User> task = SQLUser_Query.GetUser(alarmEvent.LastUser);                      //gets user from DB 
                            user = await task; //venter på sql spørring
                            AlarmLogEntry alarmLogEntry = new AlarmLogEntry { DoorNumber = alarmEvent.DoorNumber, AlarmType = alarmEvent.Alarm_type.toString(), LastUser = user, TimeStamp = alarmEvent.Time };
                            AlarmRaised?.Invoke(alarmLogEntry);                                                //flags event that new alarm has arrived 
                            break;                                                                             //subscribed on by MainActivity
                        case PackageIdentifier.ClosingDown:
                            //KillMySelf(ComSocket); //not implemented
                            break;
                        case PackageIdentifier.PinValidation:                                                  //this case happends when a new card is swiped
                            CardInfo cardInfo = JsonSerializer.Deserialize<CardInfo>(receivedString);          //deserializes the json string into class
                            Task<User> task2 = SQLUser_Query.GetUser(cardInfo);                                //gets user from DB 
                            user = await task2; //venter på sql spørring
                            bool validation = CheckUserPin(cardInfo,user);                                     //checks if the entered pincode matches registred pin from DB
                            AccessEntryTry tryArgs = new AccessEntryTry(user, cardInfo.Time, validation, cardInfo.DoorNr);
                            newAccessEntryTry?.Invoke(tryArgs);                                                //flags event newAcccesEntryTry
                             
                            ReturnCardComms queryReturn = new ReturnCardComms {Validation=validation};         //generates class for answer to pass back to cardreader
                            string jsonString = JsonSerializer.Serialize(queryReturn);                         //JSON serilizes the class
                            jsonString = Messages.AddPackageIdentifier(PackageIdentifier.PinValidation, jsonString);    //adds packageidentifier to string
                            complete = Messages.SendString(ComSocket, jsonString, out error);                  //sends the complete package with message and identifer
                            break;
                    }
                }
                
            }
        }
        private static bool CheckUserPin(CardInfo data, User user)
        {
            /* 
          * this method check if the pin between recevied carddata and user from SQL db matches.
          */
            bool access;
            if (user.Pin == data.PinEntered)
            {
                access = true;
            }
            else
            {
                access = false;
            }
            return access;
        }
    }
}
