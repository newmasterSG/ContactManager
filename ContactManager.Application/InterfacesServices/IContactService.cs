using ContactManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactManager.Application.InterfacesServices
{
    public interface IContactService
    {
        IEnumerable<ContactEntity> GetByUserId(string userId);
        Task<ContactEntity?> GetByIdAsync(int Id);
        Task<IEnumerable<ContactEntity>> GetAllAsync();
        Task<ContactEntity?> CreateAsync(ContactEntity contact);
        Task UpdateUrlAsync(ContactEntity contact);
        Task DeleteAsync(int id);
    }
}
