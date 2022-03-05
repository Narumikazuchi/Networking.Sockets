namespace Narumikazuchi.Networking.Sockets;

[Browsable(false)]
[EditorBrowsable(EditorBrowsableState.Never)]
#pragma warning disable CS1591
public interface IServerWithPort
#pragma warning restore
{
    /// <summary>
    /// Sets the size of the <see cref="Byte"/>[] buffer in which incoming raw messages are temporarily stored. 
    /// </summary>
    /// <param name="bufferSize">The size of the temporary buffer.</param>
    public IServerWithoutSerialization WithBufferSize(in Int32 bufferSize);
}