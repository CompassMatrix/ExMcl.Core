using System;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Microsoft
{
    internal class GClient
    {
        private Socket socket;

        public Socket getSocket()
        {
            connect();
            return socket;
        }

        private bool connect()
        {
            try
            {
                if (socket == null)
                {
                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                }
                while (!socket.Connected)
                {
                    try
                    {
                        socket.Connect("127.0.0.1", 20500);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch
            {
                return false;
            }
            return socket.Connected;
        }

        public bool send(byte[] bytes)
        {
            try
            {
                if (connect())
                {
                    socket.Blocking = false;
                    socket.SendTimeout = 5000;
                    try
                    {
                        socket.Send(bytes, bytes.Length, SocketFlags.None);
                    }
                    catch (Exception)
                    {
                        try
                        {
                            socket.Send(bytes, bytes.Length, SocketFlags.None);
                        }
                        catch (Exception)
                        {
                            return false;
                        }
                    }
                    try
                    {
                        socket.Shutdown(SocketShutdown.Send);
                        socket.Disconnect(reuseSocket: false);
                        socket.Close();
                        socket = null;
                    }
                    catch
                    {
                    }
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }

        [Obfuscation(Feature = "virtualization", Exclude = false)]
        public bool send(string str)
        {
            try
            {
                if (str == null || str.Length == 0)
                {
                    return true;
                }
                return send(Encoding.UTF8.GetBytes(str));
            }
            catch
            {
                return false;
            }
        }

        public bool send(string a, string b)
        {
            try
            {
                if ((a == null || a.Length == 0) && (b == null || b.Length == 0))
                {
                    return false;
                }
                return send("|" + a + "|" + b);
            }
            catch
            {
            }
            return false;
        }

        public string send_Ret(string a, string b)
        {
            try
            {
                if (a == null || a.Length == 0)
                {
                    return "";
                }
                send("RETN " + a, b);
                string path = Tool.getMainPath() + "\\" + a + ".cach";
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                return recv(path, 0);
            }
            catch
            {
                return "";
            }
        }

        public string sendByFile(string name, string content, bool ret)
        {
            if (ret)
            {
                try
                {
                    if (name == null || name.Length == 0)
                    {
                        return "";
                    }
                    string text = Tool.getMainPath() + "\\" + name + ".src";
                    if (File.Exists(text))
                    {
                        File.Delete(text);
                    }
                    StreamWriter streamWriter = File.CreateText(text);
                    streamWriter.Write(content);
                    streamWriter.Flush();
                    streamWriter.Close();
                    send("RETN FILE " + name, text);
                    string path = Tool.getMainPath() + "\\" + name + ".cach";
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                    return recv(path, 0);
                }
                catch
                {
                    return "";
                }
            }
            return "";
        }

        public string recv(string path, int outTime = 15)
        {
            int num = 0;
            int num2 = 100;
            send("needRecvFile:", path);
            while (!File.Exists(path))
            {
                if (outTime > 0)
                {
                    num += num2;
                }
                Thread.Sleep(num2);
                if (outTime * 1000 < num)
                {
                    send("RecvTimeOut", path);
                    break;
                }
            }
            try
            {
                StreamReader streamReader = new StreamReader(path, Encoding.UTF8);
                string text = "";
                string text2 = "";
                while ((text2 = streamReader.ReadLine()) != null)
                {
                    text += text2;
                }
                streamReader.Close();
                send("Recv", text);
                File.Delete(path);
                return text;
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
