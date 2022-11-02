using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Exceptions
{
    public class NoLongerValid : Exception
    {
        public NoLongerValid(long id) : base($"The Session with the id {id} is no longer Valid") { }
    }
}
