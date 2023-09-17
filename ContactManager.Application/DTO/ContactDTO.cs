using ContactManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactManager.Application.DTO
{
    public class ContactDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string DateOfBirth { get; set; }

        public bool Married { get; set; }
        public string Phone { get; set; }

        public string Salary { get; set; }

        public UserEntity User { get; set; }
    }
}
