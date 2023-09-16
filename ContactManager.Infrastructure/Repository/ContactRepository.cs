using ContactManager.Domain.Entities;
using ContactManager.Domain.Interfaces;
using ContactManager.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactManager.Infrastructure.Repository
{
    public class ContactRepository : IContactRepository
    {
        private readonly ContactDbContext _dbContext;

        public ContactRepository(ContactDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<ContactEntity?> AddAsync(ContactEntity contact)
        {
            await _dbContext.Contacts.AddAsync(contact);
            await _dbContext.SaveChangesAsync();

            return contact;
        }

        public async Task DeleteAsync(int id)
        {
            var contact = await GetByIdAsync(id);
            if (contact == null)
                return;

            _dbContext.Contacts.Remove(contact);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<ContactEntity>> GetAllAsync()
        {
            return await _dbContext.Contacts.ToListAsync();
        }

        public async Task<ContactEntity?> GetByIdAsync(int id)
        {
            return await _dbContext.Contacts.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task UpdateAsync(ContactEntity contact)
        {
            _dbContext.Entry(contact).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public IEnumerable<ContactEntity> GetByUserIdAsync(string userId)
        {
            return _dbContext.Contacts.Where(u => u.UserId == userId).ToList();
        }
    }
}
