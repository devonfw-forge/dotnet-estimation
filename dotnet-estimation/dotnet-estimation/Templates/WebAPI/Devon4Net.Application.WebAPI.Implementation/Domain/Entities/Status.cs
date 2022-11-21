namespace Devon4Net.Application.WebAPI.Implementation.Domain.Entities
{
    [Flags]
    public enum Status
    {
        Open = 0b_0000_0000,
        Evaluated = 0b_0000_0001,
        Suspended = 0b_0000_0010,
        Ended = 0b_0000_0100,
    }
}