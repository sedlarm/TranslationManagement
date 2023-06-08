using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TranslationManagement.Core.Models;

namespace TranslationManagement.Core.Services
{
    public static class TranslationJobFileReader
    {
        public enum TranslationJobFileType
        {
            TXT,
            XML
        }
        public static async Task<TranslationJob> CreateFromStream(TranslationJobFileType type, StreamReader reader)
        {
            string content = "", customerName = "";

            if (type == TranslationJobFileType.TXT)
            {
                content = await reader.ReadToEndAsync();
            }
            else if (type == TranslationJobFileType.XML)
            {
                //TODO: Add XML schema validation
                try
                {
                    var xdoc = XDocument.Parse(await reader.ReadToEndAsync());
                    if (xdoc != null)
                    {
                        content = xdoc.Root.Element("Content").Value;
                        customerName = xdoc.Root.Element("Customer").Value.Trim();
                    }
                }
                catch (Exception)
                {
                    return null;
                }
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
