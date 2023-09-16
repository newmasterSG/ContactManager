using ContactManager.Application.InterfacesServices;
using ContactManager.Domain.Entities;
using ContactManager.Domain.Interfaces;
using ContactManager.Infrastructure.Context;
using ContactManager.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactManager.Application.Services
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _context;

        public ContactService(IContactRepository context)
        {
            _context = context;
        }

        public async Task<ContactEntity?> CreateAsync(ContactEntity contact)
        {
            var result =  await _context.AddAsync(contact);

            return result;
        }

        public async Task DeleteAsync(int id)
        {
            await _context.DeleteAsync(id);
        }

        public async Task<IEnumerable<ContactEntity>> GetAllAsync()
        {
            return await _context.GetAllAsync();
        }

        public async Task<ContactEntity?> GetByIdAsync(int Id)
        {
            return await _context.GetByIdAsync(Id);
        }

        public IEnumerable<ContactEntity> GetByUserId(string userId)
        {
            return _context.GetByUserIdAsync(userId);
        }

        public async Task UpdateUrlAsync(ContactEntity contact)
        {
            await _context.UpdateAsync(contact);
        }
    }
}
