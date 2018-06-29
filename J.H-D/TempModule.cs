using Discord.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace J.H_D
{
    class TempModule : ModuleBase
    {
        Program p = Program.p;

        [Command("Send reality")]
        public async Task parsedsbml()
        {
            TcpClient client = new TcpClient(IPAddress.Parse("82.66.248.63").ToString(), 2121);
            Byte[] data = Encoding.Unicode.GetBytes("Test jeremia's side");
            NetworkStream stream = client.GetStream();
            stream.Write(data, 0, data.Length);

            data = new byte[256];
            string response = null;

            Int32 bytes = stream.Read(data, 0, data.Length);
            response = Encoding.Unicode.GetString(data, 0, bytes);
            await ReplyAsync(response);

            stream.Close();
            client.Close();
        }
        
        [Command("Prepare Google API call")]
        public async Task Requestmaker()
        {
            string jsonrequester = null;
            Byte[] imagememstream = File.ReadAllBytes("test.png");

            jsonrequester = "{\"requests\"[{\"image\":{\"content\":" + Convert.ToBase64String(imagememstream) + "}}]}";
            File.WriteAllText("apirequest.txt", jsonrequester);
            await Context.Channel.SendFileAsync("apirequest.txt");
        }

        [Command("Shutdown local server")]
        public async Task killserv()
        {
            TcpClient client = new TcpClient(IPAddress.Loopback.ToString(), 1234);
            Byte[] data = Encoding.Unicode.GetBytes("shutdown server");
            NetworkStream stream = client.GetStream();
            stream.Write(data, 0, data.Length);

            data = new byte[256];
            string response = null;

            Int32 bytes = stream.Read(data, 0, data.Length);
            response = Encoding.Unicode.GetString(data, 0, bytes);
            await ReplyAsync(response);

            stream.Close();
            client.Close();
        }
    }
}
