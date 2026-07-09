namespace NettyFucker
{
    public class Packet
    {
        public int Length { get; set; }
        public int PacketID { get; set; }
        public byte[] Data { get; set; }
    }
}
