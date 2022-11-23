using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;

namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos
{
    public class TaskStatusChangeDto
    {
        public string Id { get; set; }

        public Status Status { get; set; }

        public void Deconstruct(out string id, out Status status)
        {
            id = Id;
            status = Status;
        }
    }
}
