using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreEstoreMvc.Services
{
    public interface IMailMessageService
    {
        Task<bool> Send(string to, string subject , string body);
    }
}
