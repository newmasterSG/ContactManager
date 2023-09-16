using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactManager.Domain.Interfaces
{
    public abstract class EmailSender
    {
        public abstract Task SendEmailAsync(string to, string from, string subject, string body, bool htmlBody);
    }
}
