using FileWeb.App.FileCore;
using FileWeb.App.Models;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace FileWeb.App.Controllers
{
    public class FileController : Controller
    {
        private FileManager _fileManager;
        private ILogger<FileController> _logger;
        public FileController(FileManager fileManager, ILogger<FileController> logger) 
        {
            _fileManager = fileManager;
            _logger = logger;
        }

        [Route("/file/downLoad/{*path}")]
        [HttpGet]
        public IActionResult DownLoad(string path)
        {
            FileStream fileStream = null;
            try
            {
                path = HttpUtility.UrlDecode(path);
                string name = Path.GetFileName(path);
                string p = Path.GetDirectoryName(path) ?? string.Empty;
                if (string.IsNullOrWhiteSpace(name))
                {
                    return NotFound();
                }
                fileStream = _fileManager.GetFileStream(p, name);
                if (fileStream == null)
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogInformation("downLoad" + path);
                    return File(fileStream, "application/octet-stream");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return NotFound();
        }

        [Route("/file/upload")]
        [HttpPost]
        public async Task<ResponseModel<FileUploadResultModel>> Upload(string partitionName, IFormFile file, string fileName, bool coverFile = false)
        {
            ResponseModel<FileUploadResultModel> responseModel = new ResponseModel<FileUploadResultModel>();
            FileUploadResultModel resultModel = new FileUploadResultModel();
            try
            {
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                }
                string path = await _fileManager.SaveFileAsync(partitionName, file.OpenReadStream(), fileName, coverFile);
                resultModel.Path = path;
                responseModel.Code = 200;
                responseModel.Data = resultModel;
            }
            catch (ArgumentException ex)
            {
                responseModel.Code = 500;
                responseModel.Message = ex.Message;
            }
            catch (Exception ex)
            {
                responseModel.Code = 500;
                responseModel.Message = "Unknown Error";
            }
            return responseModel;
        }

        [Route("/file/delete")]
        [HttpPost]
        public ResponseModel Delete(string partitionName, string fileName)
        {
            ResponseModel responseModel = new ResponseModel();
            try
            {
                _fileManager.DeleteFile(partitionName, fileName);
                responseModel.Code = 200;
            }
            catch (ArgumentException ex)
            {
                responseModel.Code = 500;
                responseModel.Message = ex.Message;
            }
            catch (Exception ex)
            {
                responseModel.Code = 500;
                responseModel.Message = "Unknown Error";
            }
            return responseModel;
        }




        public IActionResult GetPubLicFile(string partition, string fileName)
        {
            string path = Path.Combine(Path.GetTempPath(), partition, fileName);
            if (!System.IO.File.Exists(path))
            {
                return NotFound();
            }
            return NotFound();
        }


    }
}
