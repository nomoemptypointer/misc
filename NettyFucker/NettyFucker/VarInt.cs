using System.Net.Sockets;

namespace NettyFucker
{
    public static class VarInt
    {
        public static int ReadVarInt(NetworkStream stream)
        {
            int numRead = 0;
            int result = 0;
            byte read;
            do
            {
                read = (byte)stream.ReadByte();
                int value = (read & 0b01111111);
                result |= (value << (7 * numRead));

                numRead++;
                if (numRead > 5)
                {
                    throw new InvalidDataException("VarInt is too big");
                }
            } while ((read & 0b10000000) != 0);
            return result;
        }

        public static void WriteVarInt(Stream stream, int value)
        {
            while ((value & -128) != 0)
            {
                stream.WriteByte((byte)(value & 127 | 128));
                value >>= 7;
            }
            stream.WriteByte((byte)value);
        }
    }
}
