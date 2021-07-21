using System;
using Snapchat_Monitor.Classes;

namespace Snapchat_Monitor_CLI.Classes
{
    class Discord
    {
        public static void notificationWebhook(string webhookUrl, string embedColour, string username, string notification)
        {
            object[] postDataArgs = { username, DateTime.UtcNow.ToString("o"), embedColour, "Received notification", username, notification };

            string postData = "{{\"embeds\":[{{\"title\":\"Snapchat Monitor\",\"url\":\"https://snapchat.com/add/{0}\",\"timestamp\":\"{1}\",\"color\":\"{2}\",\"fields\":[{{\"name\":\"Type\",\"value\":\"{3}\",\"inline\":false}},{{\"name\":\"Recipient\",\"value\":\"{4}\",\"inline\":false}},{{\"name\":\"Notification\",\"value\":\"{5}\",\"inline\":false}}],\"thumbnail\":{{\"url\":\"https://i.imgur.com/eReDEHN.png\"}},\"footer\":{{\"icon_url\":\"https://i.imgur.com/eReDEHN.png\"}}}}]}}";

            HTTP.Request(webhookUrl, String.Format(postData, postDataArgs));
        }

        public static void messageWebhook(string webhookUrl, string embedColour, string username, string message)
        {
            object[] postDataArgs = { username, DateTime.UtcNow.ToString("o"), embedColour, "Received message", username, message };

            string postData = "{{\"embeds\":[{{\"title\":\"Snapchat Monitor\",\"url\":\"https://snapchat.com/add/{0}\",\"timestamp\":\"{1}\",\"color\":\"{2}\",\"fields\":[{{\"name\":\"Type\",\"value\":\"{3}\",\"inline\":false}},{{\"name\":\"Recipient\",\"value\":\"{4}\",\"inline\":false}},{{\"name\":\"Message\",\"value\":\"{5}\",\"inline\":false}}],\"thumbnail\":{{\"url\":\"https://i.imgur.com/eReDEHN.png\"}},\"footer\":{{\"icon_url\":\"https://i.imgur.com/eReDEHN.png\"}}}}]}}";

            HTTP.Request(webhookUrl, String.Format(postData, postDataArgs));
        }
    }
}
