using System.ComponentModel.DataAnnotations;
using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using Devon4Net.Application.WebAPI.Implementation;
using System.Text.Json.Serialization;
namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos
{
    public class ResultCreateSessionDto
    {
        public long Id { get; set; }
    }
}