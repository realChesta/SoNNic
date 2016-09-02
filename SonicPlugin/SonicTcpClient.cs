using System.IO;
using System.Net.Sockets;

namespace SoNNic
{
    public class SonicTcpClient
    {
        public readonly TcpClient Client;
        public BinaryWriter Writer;

        public SonicTcpClient(TcpClient client)
        {
            this.Client = client;
            this.Writer = new BinaryWriter(client.GetStream());
        }

        public void Close()
        { this.Client.Close(); }
    }
}
