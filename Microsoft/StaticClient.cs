namespace Microsoft
{
    internal class StaticClient
    {
        private static GClient client;

        static StaticClient()
        {
            client = new GClient();
        }

        public static bool send(string a, string b)
        {
            return client.send(a, b);
        }

        public static void send(string a)
        {
            client.send(a);
        }

        public static string send_Ret(string a, string b)
        {
            return client.send_Ret(a, b);
        }

        public static string recv(string a, int b)
        {
            return client.recv(a, b);
        }

        public static string toString()
        {
            return "";
        }
    }
}
