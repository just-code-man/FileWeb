using FileWeb.App.Models;
using Microsoft.Extensions.Options;
using System.IO;

namespace FileWeb.App.FileCore
{
    public class FileManager
    {

        private IOptions<FileSourceConfig> _options;
        
        private ILogger _logger;

        private Dictionary<string, string> _partitionMap;

        public FileManager(IOptions<FileSourceConfig> options, ILogger<FileManager> logger)
        {
            _options = options;
            _logger = logger;
        }

        private FileSourceConfig _fileSourceConfig => _options.Value;

        private string RootPath => Path.IsPathFullyQualified(_fileSourceConfig.RootPath) ? _fileSourceConfig.RootPath : Path.Combine(AppContext.BaseDirectory, _fileSourceConfig.RootPath);

        public void LoadPartitionMap()
        {
            string rootPath = RootPath;
            _partitionMap = new Dictionary<string, string>();
            if (_fileSourceConfig.PartitionConfigs == null)
            {
                throw new ArgumentNullException(nameof(_fileSourceConfig.PartitionConfigs));
            }
            foreach (var item in _fileSourceConfig.PartitionConfigs)
            {
                if (_partitionMap.ContainsKey(item.PartitionName))
                {
                    throw new ArgumentException("Same PartitionName:" + item.PartitionName);
                }
                string path = Path.Combine(RootPath, item.Path);
                if (_partitionMap.Any(p => p.Value.Trim(Path.PathSeparator) == path.Trim(Path.PathSeparator)))
                {
                    throw new ArgumentException("Same Path:" + item.Path);
                }
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                _partitionMap.Add(item.PartitionName, item.Path);
            }
        }

        public bool CheckFilePath(string path)
        {
            if (_partitionMap.Any(p => p.Value == path))
            {
                return true;
            }
            return false;
        }

        public FileStream GetFileStream(string path, string name)
        {
            if (CheckFilePath(path))
            {
                string filePath =  Path.Combine(RootPath, path, name);
                if (File.Exists(filePath))
                {
                    return new FileStream(filePath, FileMode.Open, FileAccess.Read);
                }
            }
            return null;
        }

        public async Task<string> SaveFileAsync(string partitionName, Stream stream, string fileName, bool coverFile = false)
        {
            if (!_partitionMap.ContainsKey(partitionName)) 
            {
                throw new ArgumentException("PartitionName Not Found");
            }

            string path = Path.Combine(RootPath, _partitionMap[partitionName], fileName);
            if (coverFile && File.Exists(path))
            {
                throw new ArgumentException("File Exist");
            }
            using (var st = new FileStream(path, coverFile ? FileMode.OpenOrCreate : FileMode.Create))
            {
                await stream.CopyToAsync(st);
                await st.FlushAsync();
            }
            return Path.Combine(_partitionMap[partitionName], fileName);
        }

        public void DeleteFile(string partitionName, string fileName)
        {
            if (!_partitionMap.ContainsKey(partitionName))
            {
                throw new ArgumentException("PartitionName Not Found");
            }
            string path = Path.Combine(RootPath, _partitionMap[partitionName], fileName);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

    }
}
