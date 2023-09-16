using ContactManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactManager.Domain.Interfaces
{
    public interface IContactRepository
    {
        Task<IEnumerable<ContactEntity>> GetAllAsync();
        Task<ContactEntity?> GetByIdAsync(int id);
        Task<ContactEntity?> AddAsync(ContactEntity contact);
        Task UpdateAsync(ContactEntity contact);
        Task DeleteAsync(int id);
        IEnumerable<ContactEntity> GetByUserIdAsync(string userId);
    }
}
