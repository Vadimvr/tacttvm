namespace IconInTheTaskbar
{
    public interface INotifyIconParameters
    {
        public string ApplicationNameInTheTaskbar { get; }
        public string ApplicationIconInTheTaskbarOSX { get; }
        public string ApplicationIconInTheTaskbarWin { get; }
        public string ApplicationIconInTheTaskbarLin { get; }
        public string LocalIp { get; }
        public int LocalPort { get; }
    }
}