using FinanceHub.Core.Models;
using System.Text;
using System.Text.RegularExpressions;

namespace FinanceHub.Web.Services
{
    public class VcfParserService
    {
        private readonly ILogger<VcfParserService> _logger;

        public VcfParserService(ILogger<VcfParserService> logger)
        {
            _logger = logger;
        }

        public List<Contact> Parse(string fileContent)
        {
            var contacts = new List<Contact>();
            var vcardRegex = new Regex(@"BEGIN:VCARD\s*(.*?)\s*END:VCARD", RegexOptions.Singleline);
            var matches = vcardRegex.Matches(fileContent);

            _logger.LogInformation("Encontrados {count} blocos VCard no ficheiro.", matches.Count);

            foreach (Match match in matches)
            {
                var vcardData = match.Groups[1].Value.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                string? name = null;
                string? phone = null;

                foreach (var line in vcardData)
                {
                    if (line.StartsWith("FN")) // Apanha FN, FN;CHARSET, etc.
                    {
                        name = line.Substring(line.IndexOf(':') + 1);
                        if (line.Contains("QUOTED-PRINTABLE"))
                        {
                            name = DecodeQuotedPrintable(name);
                        }
                    }
                    else if (line.StartsWith("TEL"))
                    {
                        phone = line.Substring(line.IndexOf(':') + 1);
                    }
                }

                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(phone))
                {
                    var cleanedPhone = Regex.Replace(phone, @"[^\d]", "");
                    if (cleanedPhone.Length >= 9)
                    {
                        var finalPhoneNumber = cleanedPhone.Substring(cleanedPhone.Length - 9);
                        contacts.Add(new Contact { Name = name.Trim(), PhoneNumber = finalPhoneNumber });
                    }
                }
            }

            _logger.LogInformation("Processados com sucesso {count} contactos válidos.", contacts.Count);
            return contacts;
        }

        private string DecodeQuotedPrintable(string input)
        {
            var correctedInput = input.Replace("=", "%");
            return Uri.UnescapeDataString(correctedInput);
        }
    }
}