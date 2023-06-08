using System.ComponentModel;
using System.Xml.Linq;
using TranslationManagement.Core.Models;

namespace TranslationManagement.Core.Services
{
    public static class TranslationJobFileReader
    {
        public enum FileType
        {
            TXT,
            XML
        }

        public static async Task<TranslationJob?> CreateTranslationJobFromStream(FileType type, Stream fileStream)
        {
            string content, customerName = "";

            var reader = new StreamReader(fileStream);
            switch (type)
            { 
                case FileType.TXT:
                    content = await reader.ReadToEndAsync();
                    break;

                case FileType.XML:  
                    //TODO: Add XML schema validation
                    try
                    {
                        var xdoc = XDocument.Parse(await reader.ReadToEndAsync());
                        if (xdoc != null && xdoc.Root != null)
                        {
                            content = xdoc.Root.Element("Content").Value;
                            customerName = xdoc.Root.Element("Customer").Value.Trim();
                        } else
                        {
                            throw new InvalidDataException();
                        }
                    }
                    catch (Exception)
                    {
                        //probably log exception
                        return null;
                    }
                    break;
                default: 
                    throw new InvalidEnumArgumentException();
            }

            var job = new TranslationJob()
            {
                OriginalContent = content,
                CustomerName = customerName,
                Status = TranslationJobStatus.New,
            };
            return job;
        }
    }
}
