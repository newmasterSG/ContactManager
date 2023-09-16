using ContactManager.Domain.Entities;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactManager.Application.Maps
{
    public class ContactCsvMap : ClassMap<ContactEntity>
    {
        public ContactCsvMap()
        {
            Map(m => m.Name).Name("Name").Index(0).Default("");
            Map(m => m.DateOfBirth).Name("DateOfBirth").Index(1).TypeConverterOption.Format("yyyy-MM-dd").Default(DateTime.MinValue);
            Map(m => m.Married).Name("Married").Index(2).Default(false);
            Map(m => m.Phone).Name("Phone").Index(3).Default("");
            Map(m => m.Salary).Name("Salary").Index(4).Default(0);
        }
    }
}
