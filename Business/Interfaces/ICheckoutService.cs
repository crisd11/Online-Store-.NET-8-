namespace Core.Interfaces
{
    public interface ICheckoutService
    {
        Task<string> CreateCheckoutSessionAsync(string userId);
    }
}
