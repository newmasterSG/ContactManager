using ContactManager.Application.InterfacesServices;
using ContactManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using CsvHelper;
using ContactManager.Application.Maps;

namespace ContactManager.Application.Services
{
    public class CsvService : ICsvService
    {
        public async Task<IEnumerable<ContactEntity>> ReadContactsFromCsv(IFormFile file)
        {
            List<ContactEntity> contacts = new List<ContactEntity>();

            if (file == null || file.Length <= 0)
            {
                return contacts;
            }

            using (var reader = new StreamReader(file.OpenReadStream()))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {

                csv.Context.RegisterClassMap<ContactCsvMap>();

                while (csv.Read())
                {
                    contacts.Add(csv.GetRecord<ContactEntity>());
                }
            }

            return contacts;
        }
    }
}
