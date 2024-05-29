using System.Text;
using System.Net.Sockets;

namespace PlaziatCore
{

    /// <summary>
    /// Simplifies tcp coms: Send and receive simple strings on a stream
    /// Messages are completed with "\x02" and "\x03 at start and end
    /// reception is done one byte after another so the data can be any length
    /// </summary>
    public class TcpCommunications
    {

        NetworkStream stream;

        public TcpCommunications(NetworkStream stream)
        {
            this.stream = stream;
        }

        /// <summary>
        /// Receive data from the stream
        /// </summary>
        /// <returns>Transfered data</returns>
        public async Task<string> ReceiveData()
        {
            string data = string.Empty;
            try
            {
                bool done = false;
                int i;
                byte[] bytes = new byte[1];
                await stream.ReadAsync(bytes, 0, 1);
                if (Encoding.ASCII.GetString(bytes, 0, 1) == "\x02")
                {
                    while (!done)
                    {
                        await stream.ReadAsync(bytes, 0, 1);
                        if (Encoding.ASCII.GetString(bytes, 0, 1) == "\x03") done = true;
                        else data += Encoding.ASCII.GetString(bytes, 0, 1);
                    }
                    return data;
                }
                else return null;
            }
            catch { return null; }
        }

        /// <summary>
        /// Send data on the stream
        /// </summary>
        /// <param name="message">Data to transfer</param>
        public async Task SendData(string message)
        {
            message = "\x02" + message + "\x03";
            try
            {
                byte[] msg = Encoding.ASCII.GetBytes(message);
                await stream.WriteAsync(msg, 0, msg.Length);
            }
            catch (Exception e) { Logger.Log(e); }
        }

    }

}
