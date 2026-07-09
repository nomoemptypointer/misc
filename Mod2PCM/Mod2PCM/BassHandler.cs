using KohakowWavWriter;
using ManagedBass;

namespace Mod2PCM
{
    public class BassHandler
    {
        private int stream;
        private bool _write;
        private const int bufferSize = 1024 * 2;

        public BassHandler(bool write = true)
        {
            _write = write;
            if (!Bass.Init())
            {
                throw new Exception("BASS initialization error occurred!");
            }
        }

        public void Load(string path)
        {
            BassFlags flag = _write ? BassFlags.Decode : BassFlags.Default;
            stream = Bass.MusicLoad(path, 0, 0, flag);
            if (stream == 0)
            {
                throw new Exception($"Failed to load stream from {path}. Error code: {Bass.LastError}");
            }
        }

        public void Play()
        {
            if (!Bass.ChannelPlay(stream))
            {
                throw new Exception("Failed to play stream.");
            }
        }

        public void Write(string path = "output.wav")
        {
            if (!_write)
            {
                throw new Exception("Cannot write in play mode.");
            }

            using (var writer = new Writer())
            {
                int bytesRead;
                byte[] buffer = new byte[bufferSize * 4]; // 4 bytes per sample (16-bit stereo)
                while ((bytesRead = Bass.ChannelGetData(stream, buffer, buffer.Length)) > 0)
                {
                    writer.WriteSamples(ByteArrayToShortArray(buffer, bytesRead));
                }
                writer.SaveToFile(path);
            }
        }

        public static short[] ByteArrayToShortArray(byte[] byteArray, int bytesRead)
        {
            if (bytesRead % 2 != 0)
            {
                throw new ArgumentException("byteArray length must be even.");
            }
            short[] shortArray = new short[bytesRead / 2];
            Buffer.BlockCopy(byteArray, 0, shortArray, 0, bytesRead);
            return shortArray;
        }
    }
}
