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
            Mainform = x;
            comsocket = y;

            ThreadStart ts = new ThreadStart(ConnectionHandler);
            Thread TCPThread = new Thread(ts);
            TCPThread.Name = "TCPclientThread";
            TCPThread.IsBackground = true;
            TCPThread.Start();
        } 
        public static async void ConnectionHandler()
        {
            Socket ComSocket = comsocket;
            bool error = false;
            bool complete = false;
            Messages.SendString(ComSocket, PackageIdentifier.ServerACK, out error);

            while (ComSocket.Connected)
            {
                if (ComSocket.Available > 0) // receive data from connected socket if available
                {
                    string receivedString = Messages.ReceiveString(ComSocket, out error);
                    string packageID = Messages.GetPackageIdentifier(ref receivedString, out receivedString);
                    User user = new User();
                    switch (packageID)
                    {
                        case PackageIdentifier.AlarmEvent:
                            AlarmEvent alarmEvent = JsonSerializer.Deserialize<AlarmEvent>(receivedString);
                            Task<User> task = SQLUser_Query.GetUser(alarmEvent.LastUser);
                            user = await task; //venter på sql spørring
                            AlarmLogEntry alarmLogEntry = new AlarmLogEntry { DoorNumber = alarmEvent.DoorNumber, AlarmType = alarmEvent.Alarm_type.toString(), LastUser = user, TimeStamp = alarmEvent.Time };
                            AlarmRaised?.Invoke(alarmLogEntry); //fire alarm event
                            break;
                        case PackageIdentifier.ClosingDown:
                            //KillMySelf(ComSocket);
                            break;
                        case PackageIdentifier.PinValidation:
                            CardInfo cardInfo = JsonSerializer.Deserialize<CardInfo>(receivedString);
                            Task<User> task2 = SQLUser_Query.GetUser(cardInfo);
                            user = await task2; //venter på sql spørring

                            bool validation = CheckUserPin(cardInfo,user);
                            AccessEntryTry tryArgs = new AccessEntryTry(user, cardInfo.Time, validation, cardInfo.DoorNr);

                            newAccessEntryTry?.Invoke(tryArgs);//fire event
                             
                            //return answer
                            ReturnCardComms queryReturn = new ReturnCardComms {Validation=validation};
                            string jsonString = JsonSerializer.Serialize(queryReturn);
                            jsonString = Messages.AddPackageIdentifier(PackageIdentifier.PinValidation, jsonString);
                            complete = Messages.SendString(ComSocket, jsonString, out error);
                            break;
                    }
                }
                
            }
            //KillMySelf(ComSocket);
        }
        private static bool CheckUserPin(CardInfo data, User user)
        {
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
