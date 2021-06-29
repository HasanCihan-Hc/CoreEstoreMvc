using System.Threading.Tasks;

namespace CoreEstoreMvc.Services
{
    public interface IShoppingCartService
    {
        Task AddToCartAsync(int id, int quantity = 1);
        Task RemoveFromCartAsync(int id);
        Task<int> ItemCountAsync();
        Task ClearCookieAsync();
        Task TransferCookieToDatabaseAsync();
        Task ClearShoppingCartAsync();

    }
}