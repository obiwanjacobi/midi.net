using System.IO;

namespace CannedBytes.Midi
{
    public interface IMidiStream
    {
        long Position { get; set; }
        long Length { get; }
        bool CanWrite { get; }
        bool CanSeek { get; }
        bool CanRead { get; }
        long Seek(long offset, SeekOrigin origin);
        int Read(byte[] buffer, int offset, int count);
        void Write(byte[] buffer, int offset, int count);
    }
}
