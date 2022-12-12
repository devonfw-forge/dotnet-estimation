using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public partial class WithAuthorizedUser<T>
{
    T Payload { get; set; }
    AuthenticatedUserDto User { get; set; }

    public void Deconstruct(out AuthenticatedUserDto user, out T payload)
    {
        user = User;
        payload = Payload;
    }
}