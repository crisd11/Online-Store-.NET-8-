using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IEmailBusiness
    {
        Task SendAsync(string to, string subject, string body, bool isHtml = true);
    }
}
