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

        [Command("Test aysnc local server")]
        public async Task parsedsbml()
        {
            TcpClient client = new TcpClient(IPAddress.Loopback.ToString(), 1234);
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
