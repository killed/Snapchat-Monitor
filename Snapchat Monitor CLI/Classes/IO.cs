using Snapchat_Monitor_CLI.Classes;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace Snapchat_Monitor.Classes
{
    class IO
    {
        public static void saveMessage(string username, string message, string webhookUrl, string embedColour)
        {
            JObject parsedMessage = JObject.Parse(message);

            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\logs\\" + username))
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\logs\\" + username);

            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\logs\\" + username + "\\" + parsedMessage["lSBjEQikC24hRmg"]))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\logs\\" + username + "\\" + parsedMessage["lSBjEQikC24hRmg"]);

                File.AppendAllText(@AppDomain.CurrentDomain.BaseDirectory + "\\logs\\" + username + "\\" + parsedMessage["lSBjEQikC24hRmg"] + "\\chat.txt", string.Format("{0} {1}: {2}", DateTime.Now, parsedMessage["lSBjEQikC24hRmg"], parsedMessage["t8G9dQFQeOIPBdm"]) + Environment.NewLine);
            }
            else
                File.AppendAllText(@AppDomain.CurrentDomain.BaseDirectory + "\\logs\\" + username + "\\" + parsedMessage["lSBjEQikC24hRmg"] + "\\chat.txt", string.Format("{0} {1}: {2}", DateTime.Now, parsedMessage["lSBjEQikC24hRmg"], parsedMessage["t8G9dQFQeOIPBdm"]) + Environment.NewLine);

            Console.WriteLine("[Snapchat] Received message from {0} at {1}: {2}", parsedMessage["lSBjEQikC24hRmg"], DateTime.Now, parsedMessage["t8G9dQFQeOIPBdm"]);
            Discord.messageWebhook(webhookUrl, embedColour, parsedMessage["lSBjEQikC24hRmg"].ToString(), parsedMessage["t8G9dQFQeOIPBdm"].ToString());
        }
    }
}
