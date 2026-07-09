using System.Net.Sockets;
using System.Text;

namespace NettyFucker
{
    public class Client
    {
        private readonly TcpClient _client;
        private readonly NetworkStream _stream;

        public Client(TcpClient client)
        {
            _client = client;
            _stream = client.GetStream();
        }

        public void SendHandshake(string serverAddress, ushort serverPort, int protocolVersion, int nextState)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                VarInt.WriteVarInt(ms, protocolVersion);
                WriteString(ms, serverAddress);
                WriteUShort(ms, serverPort);
                VarInt.WriteVarInt(ms, nextState);

                byte[] packetData = ms.ToArray();
                SendPacket(0x00, packetData); // Handshake packet ID is 0x00
            }
        }

        public void SendLoginStart(string playerName)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                WriteString(ms, playerName);

                byte[] packetData = ms.ToArray();
                SendPacket(0x00, packetData); // Login Start packet ID is 0x00
            }
        }

        private void SendPacket(int packetID, byte[] data)
        {
            using (MemoryStream ms = new())
            {
                VarInt.WriteVarInt(ms, data.Length + 1); // Packet length
                VarInt.WriteVarInt(ms, packetID); // Packet ID
                ms.Write(data, 0, data.Length);

                byte[] packet = ms.ToArray();
                _stream.Write(packet, 0, packet.Length);
            }
        }

        private void WriteString(Stream stream, string value)
        {
            byte[] data = Encoding.UTF8.GetBytes(value);
            VarInt.WriteVarInt(stream, data.Length);
            stream.Write(data, 0, data.Length);
        }

        private void WriteUShort(Stream stream, ushort value)
        {
            byte[] data = BitConverter.GetBytes(value);
            Array.Reverse(data); // Minecraft protocol uses big-endian
            stream.Write(data, 0, data.Length);
        }
    }
}
