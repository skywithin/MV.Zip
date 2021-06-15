using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Threading.Tasks;

namespace MV.Zip
{
    public class ZipService : IZipService
    {
        private readonly int CompressionLevel = 9; // 0 (store only) to 9 (best compression)

        public ZipService()
        {
        }

        /// <summary>
        /// Zip file content in memory
        /// </summary>
        public async Task<ZipFileResponse> CompressFileData(ZipFileRequest zipRequest)
        {
            if (zipRequest.SourceDataStream == null || zipRequest.SourceDataStream.Length == 0)
            {
                return new ZipFileResponse 
                { 
                    IsSuccess = false, 
                    Message = $"Nothing to compress. Input stream for {zipRequest.OriginalFileName} is empty" 
                };
            }

            using (var outStream = new ZipOutputStream(zipRequest.OutputZippedStream))
            {
                var fileName =
                    string.IsNullOrWhiteSpace(zipRequest.OriginalFileName)
                        ? Guid.NewGuid().ToString()
                        : zipRequest.OriginalFileName.Trim();

                outStream.SetLevel(CompressionLevel);
                outStream.Password = zipRequest.Password;
                outStream.PutNextEntry(new ZipEntry(fileName));

                var buffer = new byte[1024 * 4];
                int bytesRead;
                do
                {
                    bytesRead = await zipRequest.SourceDataStream.ReadAsync(buffer);
                    await outStream.WriteAsync(buffer, 0, bytesRead);
                } while (bytesRead > 0);
            }

            return new ZipFileResponse
            {
                IsSuccess = true,
                Message = "OK"
            };
        }
    }
}
