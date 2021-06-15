using System.IO;
using System.Threading.Tasks;

namespace MV.Zip
{
    /// <summary>
    /// Zip file content in memory
    /// </summary>
    public interface IZipService
    {
        Task<ZipFileResponse> CompressFileData(ZipFileRequest zipRequest);
    }

    public class ZipFileRequest
    {
        public Stream SourceDataStream { get; set; }

        public string OriginalFileName { get; set; }

        public string Password { get; set; }

        public Stream OutputZippedStream { get; set; }
    }

    public class ZipFileResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
