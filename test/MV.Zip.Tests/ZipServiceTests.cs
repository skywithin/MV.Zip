using FluentAssertions;
using NUnit.Framework;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MV.Zip.Tests
{
    public class ZipServiceTests
    {
        [TestCase("test.txt", "password")]
        [TestCase("C:/temp/test.txt", "pa$$w0rd1")]
        [TestCase("file-name", "")]
        [TestCase(null, null)]
        public async Task CompressData_Should_Return_True(string originalFileName, string password)
        {
            // Arrange
            var srcData = Encoding.UTF8.GetBytes("THIS IS A TEST");
            var inStream = new MemoryStream(srcData);
            var outStream = new MemoryStream();

            var sut = new ZipService();

            // Act
            var actual =
                await sut.CompressFileData(
                    new ZipFileRequest
                    {
                        OutputZippedStream = outStream,
                        SourceDataStream = inStream,
                        OriginalFileName = originalFileName,
                        Password = password
                    });

            // Assert
            actual.IsSuccess.Should().BeTrue();

            var zippedBytes = outStream.ToArray();
            zippedBytes.Should().NotBeNull();
            zippedBytes.Should().NotBeEmpty();

            Assert.IsFalse(srcData.SequenceEqual(zippedBytes));
        }

        [TestCase(null)]
        [TestCase("")]
        public async Task CompressData_With_No_Input_Should_Return_False(string content)
        {
            // Arrange
            MemoryStream inStream = null;

            if (content != null)
            {
                var srcData = Encoding.UTF8.GetBytes(content);
                inStream = new MemoryStream(srcData);
            }

            var outStream = new MemoryStream();
            var sut = new ZipService();

            // Act
            var actual =
                await sut.CompressFileData(
                    new ZipFileRequest
                    {
                        OutputZippedStream = outStream,
                        SourceDataStream = inStream,
                        OriginalFileName = "test.txt",
                        Password = null
                    });

            // Assert
            actual.IsSuccess.Should().BeFalse();
            outStream.ToArray().Should().BeEmpty();
        }

        //Use for zipping real file
        //[Test]
        public async Task CompressPhysicalFile_With_Password_Test()
        {
            string sourceFilePath = "C:/Temp/test.json";
            string zippedFilePath = "C:/Temp/test.zip";
            string? password = "1234";

            using (var srcFileStream = File.OpenRead(sourceFilePath))
            {
                var originalFileName = Path.GetFileName(sourceFilePath);
                var zippedStream = new MemoryStream();

                var sut = new ZipService();

                await sut.CompressFileData(
                    new ZipFileRequest
                    {
                        OutputZippedStream = zippedStream,
                        SourceDataStream = srcFileStream,
                        OriginalFileName = originalFileName,
                        Password = password
                    });

                File.WriteAllBytes(zippedFilePath, zippedStream.ToArray());
            }
        }
    }
}
