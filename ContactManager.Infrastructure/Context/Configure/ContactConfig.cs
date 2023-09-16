using ContactManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ContactManager.Infrastructure.Context.Configure
{
    public class ContactConfig : IEntityTypeConfiguration<ContactEntity>
    {
        public void Configure(EntityTypeBuilder<ContactEntity> builder)
        {
            builder
                 .HasOne(c => c.User)
                 .WithMany(u => u.Contacts)
                 .HasForeignKey(c => c.UserId)
                 .OnDelete(DeleteBehavior.Cascade);

            builder
                .Property(c => c.Salary)
                .HasColumnType("decimal(18, 2)");
        }
    }
}
