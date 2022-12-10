namespace Cherry
{
    public class CherryConfiguration
    {
        public List<string> ListenerPrefixes { get; set; } = new List<string>();

        private CherryConfiguration() { }

        //public static CherryConfiguration From(IConfiguration)
    }
}
