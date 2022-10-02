using ClassLibrary;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.IO.Ports;
class CardReader
{
    static void Main(string[] args)
    {
        bool error = false;
        Socket comSocket;
        SerialPort doorPort;

        //FOR DEBUGGING
        CardInfo lastCardUsed = new CardInfo { CardID=1222, PinEntered = "1333",Number=1,Time = DateTime.Now};
        CardComms comms = new CardComms{Time=DateTime.Now,Number=1,CardID=1234,Pin="1111",Alarm_bool=false,Alarm_type =0,Lastuser = 4432, Need_validation = true};
        CardInfo cardInfo = new CardInfo { CardID = 1234, Number = 1, PinEntered = "1111" };
        AlarmEvent alarmEvent = new AlarmEvent { Alarm_bool = true,Alarm_type = 2, LastCardUsed = lastCardUsed};

        while (true)
        {
            comSocket = Connect2Server(); //no need for 
            doorPort = Connect2Door();
            while (comSocket.Connected) 
                {
                    if (comSocket.Available > 0)
                    {
                        string? receivedString = Central.ReceiveString(comSocket, out error);
                        string packageID = Central.GetPackageIdentifier(ref receivedString, out receivedString);
                        switch (packageID)
                        {
                            case PackageIdentifier.ServerACK:
                                Console.WriteLine("{0} received from: {1}", receivedString, comSocket.RemoteEndPoint);
                                break;
                            case PackageIdentifier.PinValidation:
                                ReturnCardComms returnCardComms = JsonSerializer.Deserialize<ReturnCardComms>(receivedString);
                                //FORSTEETT HER
                                Console.WriteLine("{0} received from: {1}", receivedString, comSocket.RemoteEndPoint);
                                break;
                            case PackageIdentifier.RequestNumber:
                            default:
                                break;
                        }
                    }

                    var someDoorEvent = true;
                    if (someDoorEvent = true)
                    {
                        string jsonString = JsonSerializer.Serialize(cardInfo, cardInfo.GetType());
                        jsonString = Central.AddPackageIdentifier(PackageIdentifier.CardInfo, jsonString);
                        error = Central.SendString(comSocket, jsonString, out error);
                        if (error) break;
                        //if (receivedString != "Server_ACK")
                        //    {
                        //        Console.WriteLine("{0} received from: {1}", receivedString, comSocket.RemoteEndPoint);
                        //    }
                        //    else
                        //    {
                        //        ReturnCardComms returnCard = JsonSerializer.Deserialize<ReturnCardComms>(receivedString);
                        //    }
                        if (error) break;

                    }


            }
            Thread.Sleep(1000); //Wait 1000MS before trying to reconnect
        }

        //shall never be disconnected
        //if (!noConnect)
        //{
        //    comSocket.Shutdown(SocketShutdown.Both);
        //    comSocket.Close();
        //}
    }

    private static SerialPort Connect2Door()
    {
        SerialPort serialPort = new SerialPort("COM4", 152000);
        serialPort.Close();
        //adds an event when data is received from serialPort
        serialPort.DataReceived += new SerialDataReceivedEventHandler(serial_data_received);
        serialPort.Open();
        serialPort.ReadTimeout = 1000;
        return serialPort;

    }

    private static void serial_data_received(object sender, SerialDataReceivedEventArgs e)
    {
        string? temp = sender.ToString();
        Console.WriteLine(temp);
        int a = 1;
    }

    private static Socket Connect2Server()
    {
        Socket clientSocket = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream,
            ProtocolType.Tcp);
        IPEndPoint serverEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);
        try
        {
            clientSocket.Connect(serverEP);

        }
        catch (Exception e)
        {
            Console.WriteLine("Link failed to establish");
        }
        return clientSocket;
    }
}
