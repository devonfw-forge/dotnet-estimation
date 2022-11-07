using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;

namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos
{
    public class EstimationDto
    {
        public string VoteBy { get; set; }

        public int Complexity { get; set; }
    }
}
