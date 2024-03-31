namespace FileWeb.App.FileCore
{
    public class FileSourceConfig
    {
        public string RootPath { get; set; }

        public List<FileSourcePartitionConfig> PartitionConfigs { get; set; }
    }
}
