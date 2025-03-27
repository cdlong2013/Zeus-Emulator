using System;
using Plus;
using Fleck;
using System.Threading;
using System.Text.RegularExpressions;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.Security.Authentication;

namespace WebSocket
{
    public class WebManager
    {
        public WebSocketServer WS;
        public WebManager()
        {

            this.WS = new WebSocketServer("ws://0.0.0.0:2096");
            //this.WS.Certificate = new X509Certificate2(@"C:\xampp\htdocs\MyCert.pfx", "lol123lol");
            // this.WS.EnabledSslProtocols = SslProtocols.Ssl3 | SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;
            

        }
        public void Init()
        {
        
            this.WS.Start(ws =>
            {
               
                ws.OnOpen = () => ws.Send("searching");
               // Console.WriteLine("Websocket sent the event 'searching'");
                ws.OnMessage = msg =>
                {
                    //Console.WriteLine("Websocket Init() has been started!");
                    if (String.IsNullOrEmpty(msg))
                        return;
                    string Evnt = Regex.Split(msg, "Event")[1].Split('.')[0];
                    string Name = Regex.Split(msg, "Name")[1].Split('.')[0];
                    string Data = Regex.Split(msg, "Data")[1].Split('.')[0];
                    string ExtraData = Regex.Split(msg, "ExtraData")[1].Split('.')[0];
                    var user = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Name);
                    if (Evnt == "search")
                    {
                   
                        if (user != null && user.GetHabbo() != null && user.GetRolePlay() != null)
                        {
                            if (user.GetRolePlay().AutoLogout > 0 || user.GetRolePlay().ws == ws)
                                ws.Close();
                            else user.GetRolePlay().ws = ws;
                        }
                        else ws.Send("searching");
                    }
                    else if (user != null && user.GetHabbo() != null && user.GetRolePlay() != null && user.GetRolePlay().AutoLogout == 0)
                        user.GetRolePlay().WebHandler.Handle(Evnt, Data, ExtraData);
                    else ws.Close();
                   // Console.WriteLine("Websocket connection closed.");
                };
                ws.OnClose = () => ws.Close();
                ws.OnError = Exception => Console.WriteLine("Error recieved from connection!");
            });
        }
    }
}