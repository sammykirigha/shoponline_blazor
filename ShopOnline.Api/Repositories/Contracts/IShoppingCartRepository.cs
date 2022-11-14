using ShopOnline.Api.Entities;
using ShopOnline.Models.Dtos;

namespace ShopOnline.Api.Repositories.Contracts
{
    public interface IShoppingCartRepository
    {
        Task<CartItem> AddItem(CartItemDto cartItemToAddDto);

        Task<CartItem> UpdateQty(int id,CartItemDto cartItemToAddDto);

        Task DeleteItem(int id);

        Task<CartItem> GetItem(int id);

        Task<IEnumerable<CartItem>> GetItems(int userId);

    }
}
