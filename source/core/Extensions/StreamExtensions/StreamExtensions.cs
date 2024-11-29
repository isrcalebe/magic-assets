using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MagicAssets.Core.Extensions.StreamExtensions;

internal static class StreamExtensions
{
    public static byte[] ReadAllBytesToArray(this Stream stream)
    {
        if (!stream.CanSeek)
            throw new ArgumentException($"Stream must be seekable to use this function. Consider using {nameof(ReadAllRemainingBytesToArray)} instead.", nameof(stream));

        if (stream.Length >= Array.MaxLength)
            throw new ArgumentException("The stream is too long for an array.", nameof(stream));

        stream.Seek(0, SeekOrigin.Begin);
        return stream.ReadBytesToArray((int)stream.Length);
    }

    public static Task<byte[]> ReadAllBytesToArrayAsync(this Stream stream, CancellationToken cancellationToken = default)
    {
        if (!stream.CanSeek)
            throw new ArgumentException($"Stream must be seekable to use this function. Consider using {nameof(ReadAllRemainingBytesToArray)} instead.", nameof(stream));

        if (stream.Length >= Array.MaxLength)
            throw new ArgumentException("The stream is too long for an array.", nameof(stream));

        stream.Seek(0, SeekOrigin.Begin);
        return stream.ReadBytesToArrayAsync((int)stream.Length, cancellationToken);
    }

    public static byte[] ReadBytesToArray(this Stream stream, int length)
    {
        byte[] bytes = new byte[length];
        stream.ReadExactly(bytes);
        return bytes;
    }

    public static async Task<byte[]> ReadBytesToArrayAsync(this Stream stream, int length, CancellationToken cancellationToken = default)
    {
        byte[] bytes = new byte[length];
        await stream.ReadExactlyAsync(bytes, cancellationToken).ConfigureAwait(false);
        return bytes;
    }

    public static byte[] ReadAllRemainingBytesToArray(this Stream stream)
    {
        using var memory = new MemoryStream();

        stream.CopyTo(memory);
        return memory.ToArray();
    }

    public static async Task<byte[]> ReadAllRemainingBytesToArrayAsync(this Stream stream, CancellationToken cancellationToken = default)
    {
        using var memory = new MemoryStream();

        await stream.CopyToAsync(memory, cancellationToken).ConfigureAwait(false);
        return memory.ToArray();
    }
}
