namespace FileWeb.App.FileCore
{
    public class FileSourcePartitionConfig
    {
        public string PartitionName { get; set; }
        public string Path { get; set; }
        //public bool AllowReferer { get; set; }
        //public List<string> AllowRefererUrls { get; set; }
        //public bool AutoDelete { get; set; }
        //public string AutoDeleteTime { get; set; }
        //public bool AllowOrigin { get; set; }
        //public List<string> AllowOriginUrls { get; set; }
        public int MaxFileSize { get; set; }

    }
}
