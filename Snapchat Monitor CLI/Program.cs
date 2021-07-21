using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Snapchat_Monitor.Classes;
using System.Reflection;
using Snapchat_Monitor_CLI.Classes;

namespace Snapchat_Monitor
{
    class Program
    {
        static void Main(string[] args)
        {
            const string REGISTRY_KEY = @"HKEY_CURRENT_USER\Snapchat_Monitor_CLI";
            const string REGISTY_VALUE = "BigPurrr";

            Chilkat.Crypt2 crypt = new Chilkat.Crypt2();

            bool startCheck = false;

            string discordWebhook = string.Empty;
            string embedColour = string.Empty;

            string uuid = crypt.GenerateUuid();
            string username = string.Empty;
            string payload = string.Empty;
            string mac = string.Empty;
            bool debug = false;

            Byte[] bytes = new Byte[8192];
            string data = null;
            int i;

            if (Convert.ToInt32(Microsoft.Win32.Registry.GetValue(REGISTRY_KEY, REGISTY_VALUE, 0)) == 0)
            {
                Console.WriteLine("[Server] I see it's your first time using the monitor. I've invited you to the Discord feel free to join");

                System.Diagnostics.Process.Start("https://discord.com/invite/VTeJ5hhF66");

                Microsoft.Win32.Registry.SetValue(REGISTRY_KEY, REGISTY_VALUE, 1, Microsoft.Win32.RegistryValueKind.DWord);
            }

            Thread formatProgramNameThread = new Thread(() => formatProgramName(username));

            Console.Title = string.Format("Snapchat Monitor | Disconnected");

            try
            {
                if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "config.json"))
                {
                    Console.WriteLine(string.Format("[-] {0} does not exist", AppDomain.CurrentDomain.BaseDirectory + "config.json".Split('\\').Last()));

                    Environment.Exit(1);
                }

                using (StreamReader reader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "config.json"))
                {
                    dynamic config = JsonConvert.DeserializeObject(reader.ReadToEnd());

                    foreach (var item in config)
                    {
                        username = config.snapchat.username;
                        payload = config.snapchat.payload;
                        mac = config.snapchat.mac;

                        embedColour = config.discord.embedColour;
                        discordWebhook = config.discord.webhook;

                        debug = false; // remember this is hard coded to false on release :)

                        // if you're reading this then I hope my skid detection doesn't get you :D
                    }
                }

                TcpClient client = new TcpClient("localhost", 1337);
                NetworkStream stream = client.GetStream();

                Thread pingThread = new Thread(() => Classes.Monitor.ping(stream, uuid, debug));
                Thread checkForUpdatesThread = new Thread(() => checkForUpdates(stream, uuid, debug));

                checkForUpdatesThread.Start();

                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    data = Encoding.ASCII.GetString(bytes, 0, i);

                    if (debug)
                        Console.WriteLine("Received: {0}", data);

                    if (data.Contains("\"c\":\"o\""))
                    {
                        if (startCheck == false)
                        {
                            startCheck = true;

                            Classes.Monitor.sendData(stream, new { t = "connect", s = uuid, u = username, x = payload, o = mac }, debug);
                        }
                    }

                    if (data.Contains("\"c\":\"b\""))
                    {
                        pingThread.Abort();
                        checkForUpdatesThread.Abort();
                        formatProgramNameThread.Abort();

                        stream.Close();

                        Console.Title = "[-] Snapchat Monitor | Disconnected - Update Required";

                        Console.WriteLine("[Server] You're now using an outdated version please update\n");
                    }

                    if (data.Contains("\"le0claEKQi4X9WnS4iMqC0qVlIyWvmHBQ9n78aJ5GOkOKOfydVWc5qOIZ5Njhb3CTvZMZWjZvaXUrIwMzQzY3uVGNPeopjGyPG24ZnTpJ0xfyJnHPiGOl5zEcgChn5mSWrYfEZ4CVbAE3QAhOgX5lHv6nky2BXNEKteS6n37lTLJFzfrNXlrknVqmw57Ao2gm9zBXtUhVPBCUlC91KlMQadC9LPqIrMbxA6M9KqREc49BeJJk2KBFlrD4N\":\"connected\""))
                    {
                        Console.WriteLine("[Server] Welcome back {0}\n", username);

                        formatProgramNameThread.Start();
                        pingThread.Start();
                    }

                    if (data.Contains("\"le0claEKQi4X9WnS4iMqC0qVlIyWvmHBQ9n78aJ5GOkOKOfydVWc5qOIZ5Njhb3CTvZMZWjZvaXUrIwMzQzY3uVGNPeopjGyPG24ZnTpJ0xfyJnHPiGOl5zEcgChn5mSWrYfEZ4CVbAE3QAhOgX5lHv6nky2BXNEKteS6n37lTLJFzfrNXlrknVqmw57Ao2gm9zBXtUhVPBCUlC91KlMQadC9LPqIrMbxA6M9KqREc49BeJJk2KBFlrD4N\":\"VwcXoMPwiadrpL0MLuhR\""))
                        IO.saveMessage(username, data, discordWebhook, embedColour);
                    if (data.Contains("\"le0claEKQi4X9WnS4iMqC0qVlIyWvmHBQ9n78aJ5GOkOKOfydVWc5qOIZ5Njhb3CTvZMZWjZvaXUrIwMzQzY3uVGNPeopjGyPG24ZnTpJ0xfyJnHPiGOl5zEcgChn5mSWrYfEZ4CVbAE3QAhOgX5lHv6nky2BXNEKteS6n37lTLJFzfrNXlrknVqmw57Ao2gm9zBXtUhVPBCUlC91KlMQadC9LPqIrMbxA6M9KqREc49BeJJk2KBFlrD4N\":\"YT9KnW7wi6hbCvUseATC\""))
                        Console.WriteLine("[Snapchat] Received a live snapchat from {0}", JObject.Parse(data)["lSBjEQikC24hRmg"]);

                    if (data.Contains("\"le0claEKQi4X9WnS4iMqC0qVlIyWvmHBQ9n78aJ5GOkOKOfydVWc5qOIZ5Njhb3CTvZMZWjZvaXUrIwMzQzY3uVGNPeopjGyPG24ZnTpJ0xfyJnHPiGOl5zEcgChn5mSWrYfEZ4CVbAE3QAhOgX5lHv6nky2BXNEKteS6n37lTLJFzfrNXlrknVqmw57Ao2gm9zBXtUhVPBCUlC91KlMQadC9LPqIrMbxA6M9KqREc49BeJJk2KBFlrD4N\":\"HfqqsAVOURZP7ZeS5kzg\""))
                    {
                        Discord.notificationWebhook(discordWebhook, embedColour, JObject.Parse(data)["lSBjEQikC24hRmg"].ToString(), JObject.Parse(data)["zgWO2YhrTgCVkkMntlHEN8yGzbVeWvtyi0rigd0HsSQCtnfBDTvOWV4Bnrhynq0qbNMIWxp4VWOmKWOpXbEaP8ZHJ43G7fCjIDEM"].ToString());
                        Console.WriteLine("[Snapchat] Received notification {0} {1}", JObject.Parse(data)["lSBjEQikC24hRmg"], JObject.Parse(data)["zgWO2YhrTgCVkkMntlHEN8yGzbVeWvtyi0rigd0HsSQCtnfBDTvOWV4Bnrhynq0qbNMIWxp4VWOmKWOpXbEaP8ZHJ43G7fCjIDEM"]);
                    }

                    if (data.Contains("\"le0claEKQi4X9WnS4iMqC0qVlIyWvmHBQ9n78aJ5GOkOKOfydVWc5qOIZ5Njhb3CTvZMZWjZvaXUrIwMzQzY3uVGNPeopjGyPG24ZnTpJ0xfyJnHPiGOl5zEcgChn5mSWrYfEZ4CVbAE3QAhOgX5lHv6nky2BXNEKteS6n37lTLJFzfrNXlrknVqmw57Ao2gm9zBXtUhVPBCUlC91KlMQadC9LPqIrMbxA6M9KqREc49BeJJk2KBFlrD4N\":\"7HyWnsGUyugRCE5g85dV\""))
                    {
                        Discord.notificationWebhook(discordWebhook, embedColour, JObject.Parse(data)["lSBjEQikC24hRmg"].ToString(), "voice note");
                        Console.WriteLine("[Snapchat] Received voice note from {0}", JObject.Parse(data)["lSBjEQikC24hRmg"]);
                    }

                    if (data.Contains("\"le0claEKQi4X9WnS4iMqC0qVlIyWvmHBQ9n78aJ5GOkOKOfydVWc5qOIZ5Njhb3CTvZMZWjZvaXUrIwMzQzY3uVGNPeopjGyPG24ZnTpJ0xfyJnHPiGOl5zEcgChn5mSWrYfEZ4CVbAE3QAhOgX5lHv6nky2BXNEKteS6n37lTLJFzfrNXlrknVqmw57Ao2gm9zBXtUhVPBCUlC91KlMQadC9LPqIrMbxA6M9KqREc49BeJJk2KBFlrD4N\":\"v5SNbEBn2yh38gw8Vddo\""))
                    {
                        Console.WriteLine("[Snapchat] Received {0} from {1}", JObject.Parse(data)["wY7o3PqqdUaaAYj"], JObject.Parse(data)["lSBjEQikC24hRmg"]);

                        Discord.notificationWebhook(discordWebhook, embedColour, JObject.Parse(data)["lSBjEQikC24hRmg"].ToString(), JObject.Parse(data)["wY7o3PqqdUaaAYj"].ToString());
                    }

                    if (data.Contains("\"le0claEKQi4X9WnS4iMqC0qVlIyWvmHBQ9n78aJ5GOkOKOfydVWc5qOIZ5Njhb3CTvZMZWjZvaXUrIwMzQzY3uVGNPeopjGyPG24ZnTpJ0xfyJnHPiGOl5zEcgChn5mSWrYfEZ4CVbAE3QAhOgX5lHv6nky2BXNEKteS6n37lTLJFzfrNXlrknVqmw57Ao2gm9zBXtUhVPBCUlC91KlMQadC9LPqIrMbxA6M9KqREc49BeJJk2KBFlrD4N\":\"Lt7sejxshlpSKpRqAZVn\""))
                    {
                        Console.WriteLine("[Snapchat] Received notification {0} from {1}", JObject.Parse(data)["zgWO2YhrTgCVkkMntlHEN8yGzbVeWvtyi0rigd0HsSQCtnfBDTvOWV4Bnrhynq0qbNMIWxp4VWOmKWOpXbEaP8ZHJ43G7fCjIDEM"], JObject.Parse(data)["lSBjEQikC24hRmg"]);

                        Discord.notificationWebhook(discordWebhook, embedColour, JObject.Parse(data)["lSBjEQikC24hRmg"].ToString(), JObject.Parse(data)["zgWO2YhrTgCVkkMntlHEN8yGzbVeWvtyi0rigd0HsSQCtnfBDTvOWV4Bnrhynq0qbNMIWxp4VWOmKWOpXbEaP8ZHJ43G7fCjIDEM"].ToString());
                    }

                    if (data.Contains("\"le0claEKQi4X9WnS4iMqC0qVlIyWvmHBQ9n78aJ5GOkOKOfydVWc5qOIZ5Njhb3CTvZMZWjZvaXUrIwMzQzY3uVGNPeopjGyPG24ZnTpJ0xfyJnHPiGOl5zEcgChn5mSWrYfEZ4CVbAE3QAhOgX5lHv6nky2BXNEKteS6n37lTLJFzfrNXlrknVqmw57Ao2gm9zBXtUhVPBCUlC91KlMQadC9LPqIrMbxA6M9KqREc49BeJJk2KBFlrD4N\":\"eDuUk0K0jiKjY3U6QCGE\""))
                    {
                        Console.WriteLine("[Snapchat] Received sticker from {0}", JObject.Parse(data)["lSBjEQikC24hRmg"]);

                        Discord.notificationWebhook(discordWebhook, embedColour, JObject.Parse(data)["lSBjEQikC24hRmg"].ToString(), "sticker");
                    }

                    if (data.Contains("\"le0claEKQi4X9WnS4iMqC0qVlIyWvmHBQ9n78aJ5GOkOKOfydVWc5qOIZ5Njhb3CTvZMZWjZvaXUrIwMzQzY3uVGNPeopjGyPG24ZnTpJ0xfyJnHPiGOl5zEcgChn5mSWrYfEZ4CVbAE3QAhOgX5lHv6nky2BXNEKteS6n37lTLJFzfrNXlrknVqmw57Ao2gm9zBXtUhVPBCUlC91KlMQadC9LPqIrMbxA6M9KqREc49BeJJk2KBFlrD4N\":\"6VbCwfaUTPFtjAkhrPND\""))
                    {
                        Console.WriteLine("[Snapchat] Received notification {0} {1}", JObject.Parse(data)["lSBjEQikC24hRmg"], JObject.Parse(data)["zgWO2YhrTgCVkkMntlHEN8yGzbVeWvtyi0rigd0HsSQCtnfBDTvOWV4Bnrhynq0qbNMIWxp4VWOmKWOpXbEaP8ZHJ43G7fCjIDEM"]);

                        Discord.notificationWebhook(discordWebhook, embedColour, JObject.Parse(data)["lSBjEQikC24hRmg"].ToString(), JObject.Parse(data)["zgWO2YhrTgCVkkMntlHEN8yGzbVeWvtyi0rigd0HsSQCtnfBDTvOWV4Bnrhynq0qbNMIWxp4VWOmKWOpXbEaP8ZHJ43G7fCjIDEM"].ToString());
                    }

                    if (data.Contains("\"le0claEKQi4X9WnS4iMqC0qVlIyWvmHBQ9n78aJ5GOkOKOfydVWc5qOIZ5Njhb3CTvZMZWjZvaXUrIwMzQzY3uVGNPeopjGyPG24ZnTpJ0xfyJnHPiGOl5zEcgChn5mSWrYfEZ4CVbAE3QAhOgX5lHv6nky2BXNEKteS6n37lTLJFzfrNXlrknVqmw57Ao2gm9zBXtUhVPBCUlC91KlMQadC9LPqIrMbxA6M9KqREc49BeJJk2KBFlrD4N\":\"3AABIXNQfmDWzYOuyy1L\""))
                    {
                        Console.WriteLine("[Snapchat] Received notification {0} {1}", JObject.Parse(data)["lSBjEQikC24hRmg"], JObject.Parse(data)["zgWO2YhrTgCVkkMntlHEN8yGzbVeWvtyi0rigd0HsSQCtnfBDTvOWV4Bnrhynq0qbNMIWxp4VWOmKWOpXbEaP8ZHJ43G7fCjIDEM"]);

                        Discord.notificationWebhook(discordWebhook, embedColour, JObject.Parse(data)["lSBjEQikC24hRmg"].ToString(), JObject.Parse(data)["zgWO2YhrTgCVkkMntlHEN8yGzbVeWvtyi0rigd0HsSQCtnfBDTvOWV4Bnrhynq0qbNMIWxp4VWOmKWOpXbEaP8ZHJ43G7fCjIDEM"].ToString());
                    }

                    if (data.Contains("\"le0claEKQi4X9WnS4iMqC0qVlIyWvmHBQ9n78aJ5GOkOKOfydVWc5qOIZ5Njhb3CTvZMZWjZvaXUrIwMzQzY3uVGNPeopjGyPG24ZnTpJ0xfyJnHPiGOl5zEcgChn5mSWrYfEZ4CVbAE3QAhOgX5lHv6nky2BXNEKteS6n37lTLJFzfrNXlrknVqmw57Ao2gm9zBXtUhVPBCUlC91KlMQadC9LPqIrMbxA6M9KqREc49BeJJk2KBFlrD4N\":\"l0f9uRAMu2K8RsQVUvUv\""))
                    {
                        Console.WriteLine("[Snapchat] Received notification {0} {1}", JObject.Parse(data)["lSBjEQikC24hRmg"], JObject.Parse(data)["zgWO2YhrTgCVkkMntlHEN8yGzbVeWvtyi0rigd0HsSQCtnfBDTvOWV4Bnrhynq0qbNMIWxp4VWOmKWOpXbEaP8ZHJ43G7fCjIDEM"]);

                        Discord.notificationWebhook(discordWebhook, embedColour, JObject.Parse(data)["lSBjEQikC24hRmg"].ToString(), JObject.Parse(data)["zgWO2YhrTgCVkkMntlHEN8yGzbVeWvtyi0rigd0HsSQCtnfBDTvOWV4Bnrhynq0qbNMIWxp4VWOmKWOpXbEaP8ZHJ43G7fCjIDEM"].ToString());
                    }
                }
            }
            catch (Exception e)
            {
                formatProgramNameThread.Abort();
                Console.Title = "[-] Snapchat Monitor | Disconnected";

                //Console.WriteLine(e.ToString());
                Console.WriteLine("[Server] Connection killed\n\nPress enter to exit...");

                Console.Read();
            }
        }

        public static void checkForUpdates(NetworkStream stream, string uuid, bool debug)
        {
            while (true)
            {
                Classes.Monitor.sendData(stream, new { t = "checkVersion", s = uuid, v = Assembly.GetExecutingAssembly().GetName().Version.ToString() }, debug);

                Thread.Sleep(1000 * 60 * 1);
            }
        }

        public static void formatProgramName(string username)
        {
            while (true)
            {
                string[] spinners = { "/", "-", "\\", "|" };

                for (int i = 0; i < spinners.Length; i++)
                {
                    Console.Title = string.Format("{0} Snapchat Monitor | Connected as {1}", spinners[i], username);

                    Thread.Sleep(250);
                }
            }
        }
    }
}
