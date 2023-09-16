using ContactManager.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactManager.Application.InterfacesServices
{
    public interface ICsvService
    {
        Task<IEnumerable<ContactEntity>> ReadContactsFromCsv(IFormFile file);
    }
}
