using ClassLibrary;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

class CardReader
{
    static void Main(string[] args)
    {
        bool noConnect;
        bool error = false;
        Socket comSocket;
        CardComms comms = new CardComms{Time=DateTime.Now,Number=1,CardID=1234,Pin="1111",Alarm_bool=false,Alarm_type =0,Lastuser = 4432, Need_validation = true};

        while (true)
        {
            comSocket = Connect2Server(out noConnect);
            
            if (!noConnect)
            {    
                var someDoorEvent = true;
                if (someDoorEvent = true)
                {
                    string jsonString = JsonSerializer.Serialize(comms, comms.GetType());
                    error = SharedMethod.SendString(comSocket, jsonString, out error);
                    while (true) // wait for reply
                    {
                        string receivedString = SharedMethod.ReceiveString(comSocket, out error);
                        if (receivedString != "Server_ACK")
                        {
                            Console.WriteLine("{0} received from: {1}", receivedString, comSocket.RemoteEndPoint);
                        }
                        else
                        {
                            ReturnCardComms returnCard = JsonSerializer.Deserialize<ReturnCardComms>(receivedString);
                        }
                        if (error) break;
                    }
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

    private static Socket Connect2Server(out bool noConnect)
    {
        noConnect = false;
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
            noConnect = true;
        }
        return clientSocket;
    }
}
