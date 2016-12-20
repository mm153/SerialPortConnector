// Use this code inside a project created with the Visual C# > Windows Desktop > Console Application template.
// Replace the code in Program.cs with this code.


using System;
using Newtonsoft.Json.Linq;
using System.IO.Ports;
using System.Threading;
using Quobject.SocketIoClientDotNet.Client;

namespace SerialPortConnector
{
    public class PortChat
    {
        static bool _continue;
        static SerialPort _serialPort;
        static int[] portValues;
        static int lastValue = 2000;
        static int valueRisingCounter = 0;
        static bool detectedAlcohol = false;
        static Socket serverSocket;

        public static void Main()
        {
            string name;
            string message;
            StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
            Thread readThread = new Thread(Read);

            serverSocket = IO.Socket("http://141.62.65.111:8080/");

            SendToServer();


            portValues = new int[10];

            // Create a new SerialPort object with default settings.
            _serialPort = new SerialPort();

            // Allow the user to set the appropriate properties.
            _serialPort.PortName = SetPortName(_serialPort.PortName);
            _serialPort.BaudRate = _serialPort.BaudRate;
            _serialPort.Parity = _serialPort.Parity;
            _serialPort.DataBits = _serialPort.DataBits;
            _serialPort.StopBits = _serialPort.StopBits;
            _serialPort.Handshake = _serialPort.Handshake;

            // Set the read/write timeouts
            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;

            _serialPort.Open();
            _continue = true;
            readThread.Start();

            Console.WriteLine("Type QUIT to exit");

            while (_continue)
            {
                message = Console.ReadLine();

                if (stringComparer.Equals("quit", message))
                {
                    _continue = false;
                }
            }

            readThread.Join();
            _serialPort.Close();
        }

        public static void SendToServer()
        {
            JObject objToSend = new JObject();


            objToSend.Add("value", "test2");
            objToSend.Add("type", "measured");

            serverSocket.Emit("alcohol", objToSend);


        }

        public static void Read()
        {
            while (_continue)
            {
                try
                {
                    int currValue = 0;
                    string message = _serialPort.ReadLine();

                    if (!message.Equals("\r"))
                        int.TryParse(message, out currValue);

                    if (lastValue == 2000)
                        lastValue = currValue;

                    if (currValue > lastValue + 1 && !(currValue > lastValue + 200))
                    { detectedAlcohol = true; }
                    else
                    {
                        valueRisingCounter = 0;
                        detectedAlcohol = false;
                    }

                    lastValue = currValue;
                    //if (currentElement <= portValues.Length - 1)
                    //{
                    //    portValues[currentElement % portValues.Length] = currValue;
                    //}
                    //else
                    //{
                    //    currentElement = 0;


                    //}
                    if (detectedAlcohol)
                        Console.WriteLine(currValue);
                }
                catch (TimeoutException) { }
            }
        }

        // Display Port values and prompt user to enter a port.
        public static string SetPortName(string defaultPortName)
        {
            string portName;

            Console.WriteLine("Available Ports:");
            foreach (string s in SerialPort.GetPortNames())
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("Enter COM port value (Default: {0}): ", defaultPortName);
            portName = Console.ReadLine();

            if (portName == "" || !(portName.ToLower()).StartsWith("com"))
            {
                portName = defaultPortName;
            }
            return portName;
        }
        // Display BaudRate values and prompt user to enter a value.
        public static int SetPortBaudRate(int defaultPortBaudRate)
        {
            string baudRate;

            Console.Write("Baud Rate(default:{0}): ", defaultPortBaudRate);
            baudRate = Console.ReadLine();

            if (baudRate == "")
            {
                baudRate = defaultPortBaudRate.ToString();
            }

            return int.Parse(baudRate);
        }

        // Display PortParity values and prompt user to enter a value.
        public static Parity SetPortParity(Parity defaultPortParity)
        {
            string parity;

            Console.WriteLine("Available Parity options:");
            foreach (string s in Enum.GetNames(typeof(Parity)))
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("Enter Parity value (Default: {0}):", defaultPortParity.ToString(), true);
            parity = Console.ReadLine();

            if (parity == "")
            {
                parity = defaultPortParity.ToString();
            }

            return (Parity)Enum.Parse(typeof(Parity), parity, true);
        }
        // Display DataBits values and prompt user to enter a value.
        public static int SetPortDataBits(int defaultPortDataBits)
        {
            string dataBits;

            Console.Write("Enter DataBits value (Default: {0}): ", defaultPortDataBits);
            dataBits = Console.ReadLine();

            if (dataBits == "")
            {
                dataBits = defaultPortDataBits.ToString();
            }

            return int.Parse(dataBits.ToUpperInvariant());
        }

        // Display StopBits values and prompt user to enter a value.
        public static StopBits SetPortStopBits(StopBits defaultPortStopBits)
        {
            string stopBits;

            Console.WriteLine("Available StopBits options:");
            foreach (string s in Enum.GetNames(typeof(StopBits)))
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("Enter StopBits value (None is not supported and \n" +
             "raises an ArgumentOutOfRangeException. \n (Default: {0}):", defaultPortStopBits.ToString());
            stopBits = Console.ReadLine();

            if (stopBits == "")
            {
                stopBits = defaultPortStopBits.ToString();
            }

            return (StopBits)Enum.Parse(typeof(StopBits), stopBits, true);
        }
        public static Handshake SetPortHandshake(Handshake defaultPortHandshake)
        {
            string handshake;

            Console.WriteLine("Available Handshake options:");
            foreach (string s in Enum.GetNames(typeof(Handshake)))
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("Enter Handshake value (Default: {0}):", defaultPortHandshake.ToString());
            handshake = Console.ReadLine();

            if (handshake == "")
            {
                handshake = defaultPortHandshake.ToString();
            }

            return (Handshake)Enum.Parse(typeof(Handshake), handshake, true);
        }
    }
}
