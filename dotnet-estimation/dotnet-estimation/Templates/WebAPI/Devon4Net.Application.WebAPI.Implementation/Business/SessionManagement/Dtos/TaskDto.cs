﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;

namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos
{
    public class TaskDto
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public string? Description { get; set; }

        public string? Url { get; set; }

        public Status Status { get; set; }
    }
}
