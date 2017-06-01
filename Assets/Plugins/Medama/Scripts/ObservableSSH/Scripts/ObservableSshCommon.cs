namespace Medama.ObservableSsh
{
    public enum ObservableSshStatus
    {
        DisConnected,
        EndOfStream,
        DataAvailable,
        End
    }

    public static class Const
    {
        public const string host = null;
        public const string user = null;
        public const string password = null;
        public const int port = 22;
        public const string terminalname = "console";
        public const uint columns = 80;
        public const uint rows = 25;
        public const uint width = 1024;
        public const uint height = 1024;
        public const int buffersize = 1024;
    }
}