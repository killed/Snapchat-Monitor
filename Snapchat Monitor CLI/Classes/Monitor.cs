using Newtonsoft.Json;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Snapchat_Monitor.Classes
{
    class Monitor
    {
        public static void sendData(NetworkStream stream, object rawData, bool debug)
        {
            Byte[] data = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(rawData));

            stream.Write(data, 0, data.Length);

            if (debug)
                Console.WriteLine("Sent: {0}", JsonConvert.SerializeObject(rawData));
        }

        public static void ping(NetworkStream stream, string uuid, bool debug)
        {
            while (true)
            {
                sendData(stream, new { t = "ping", s = uuid }, debug);

                Thread.Sleep(1000 * 5);
            }
        }
    }
}
