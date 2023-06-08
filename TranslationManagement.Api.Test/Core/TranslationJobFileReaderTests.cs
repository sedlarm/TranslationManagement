using System.Text;
using TranslationManagement.Core.Models;
using TranslationManagement.Core.Services;

namespace TranslationManagement.Tests.Core
{
    public class TranslationJobFileReaderTests
    {
        [Fact]
        public async Task CreateTranslationJobFromStream_XMLFile()
        { 
            string xmlData = "<?xml version='1.0' encoding='utf-8'?><root><Content>Content123</Content><Customer>Customer123</Customer></root>";
            var stream = new MemoryStream(Encoding.ASCII.GetBytes(xmlData));
            
            var job = await TranslationJobFileReader.CreateTranslationJobFromStream(
                TranslationJobFileReader.FileType.XML, stream);
            
            Assert.NotNull(job);
            Assert.Equal(TranslationJobStatus.New, job.Status);
            Assert.Equal("Customer123", job.CustomerName);
            Assert.Equal("Content123", job.OriginalContent);
        }

        [Fact]
        public async Task CreateTranslationJobFromStream_TXTFile()
        {
            string data = "Content123";
            var stream = new MemoryStream(Encoding.ASCII.GetBytes(data));

            var job = await TranslationJobFileReader.CreateTranslationJobFromStream(
                TranslationJobFileReader.FileType.TXT, stream);

            Assert.NotNull(job);
            Assert.Equal(TranslationJobStatus.New, job.Status);
            Assert.Equal("",job.CustomerName);
            Assert.Equal(data, job.OriginalContent);
        }
    }
}
