using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Web;

namespace PlanillajeColectivos.Tools
{
    public class RealTimeOnlineClient
    {
        //public static RealTimeOnlineClient CreateRealTimeOnlineProxy(string url, string username, string password)
        //{
        //    if (string.IsNullOrEmpty(url)) url = "https://notrealurl.com:443/cows/services/RealTimeOnline";

        //    CustomBinding binding = new CustomBinding();

        //    var security = TransportSecurityBindingElement.CreateUserNameOverTransportBindingElement();
        //    security.IncludeTimestamp = false;
        //    security.DefaultAlgorithmSuite = SecurityAlgorithmSuite.Basic256;
        //    security.MessageSecurityVersion = MessageSecurityVersion.WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10;

        //    var encoding = new TextMessageEncodingBindingElement();
        //    encoding.MessageVersion = MessageVersion.Soap11;

        //    var transport = new HttpsTransportBindingElement();
        //    transport.MaxReceivedMessageSize = 20000000; // 20 megs

        //    binding.Elements.Add(security);
        //    binding.Elements.Add(encoding);
        //    binding.Elements.Add(transport);

        //    RealTimeOnlineClient client = new RealTimeOnlineClient(binding, new EndpointAddress(url));

        //    // to use full client credential with Nonce uncomment this code:
        //    // it looks like this might not be required - the service seems to work without it
        //    client.ChannelFactory.Endpoint.Behaviors.Remove<System.ServiceModel.Description.ClientCredentials>();
        //    client.ChannelFactory.Endpoint.Behaviors.Add(new CustomCredentials());

        //    client.ClientCredentials.UserName.UserName = username;
        //    client.ClientCredentials.UserName.Password = password;

        //    return client;
        //}
    }
}