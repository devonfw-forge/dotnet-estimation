namespace Devon4Net.Application.WebAPI.Implementation.Domain.Entities
{
    [Flags]
    public enum Status
    {
        Open = 0,
        Evaluated = 1,
        Suspended = 2,
        Ended = 3,
    }
}