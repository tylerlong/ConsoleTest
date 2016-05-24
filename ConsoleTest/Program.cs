using System;
using System.Threading;
using System.Collections.Generic;
using Newtonsoft.Json;
using RingCentral;
using RingCentral.Http;
using RingCentral.Subscription;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var sdk = new SDK(Config.AppKey, Config.AppSecret, Config.Server, "Test App", "1.0.0");
            var platform = sdk.Platform;
            platform.Login(Config.Username, Config.Extension, Config.Password, false);

            var sub = new SubscriptionServiceImplementation() { _platform = platform };
            sub.AddEvent("/restapi/v1.0/account/~/extension/~/presence");
            sub.AddEvent("/restapi/v1.0/account/~/extension/~/message-store");
            sub.Subscribe((message) => {
                Console.WriteLine("Event Received: " + message.ToString());
            }, null, null);

            var dict = new Dictionary<string, dynamic> {
                { "text", "hello world" },
                { "from", new Dictionary<string, string> { { "phoneNumber", Config.Username} } },
                { "to", new Dictionary<string, string>[] { new Dictionary<string, string> { { "phoneNumber", Config.Receiver } } } },
            };
            var request = new Request("/restapi/v1.0/account/~/extension/~/sms", JsonConvert.SerializeObject(dict));
            for (var i = 0; i < 10; i++)
            {
                platform.Post(request);
                Thread.Sleep(30000);
            }
            sub.Remove();

            Thread.Sleep(100000);
        }
    }
}
