using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ShopOnline.Models.Dtos;
using ShopOnline.Web.Services;
using ShopOnline.Web.Services.Contracts;

namespace ShopOnline.Web.Pages
{
    public class ShoppingCartBase: ComponentBase
    {
        [Inject]
        public IJSRuntime JS { get; set; }

        [Inject]
        public IShoppingCartService shoppingCartService { get; set; }

        public List<CartItemDto> ShoppingCartItems { get; set; }

        public string ErrorMessage { get; set; }

        protected string TotalPrice { get; set; }

        protected int TotalQuantity { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                ShoppingCartItems = await shoppingCartService.GetItems(HardCoded.UserId);
                CartChanged();
            }
            catch (Exception ex)
            {

                ErrorMessage = ex.Message;
            }
        }

        protected async Task DeleteCartItem_click(int id)
        {
            var cartItemDto = await shoppingCartService.DeleteItem(id);
            RemoveCartItem(id);
            CartChanged();
        }

        protected async Task UpdateQtyCartItem_Click(int id, int qty)
        {
            try
            {
                if(qty > 0)
                {
                    var updateItemDto = new CartItemQtyUpdateDto
                    {
                        CartItemId = id,
                        Qty = qty
                    };

                    var returnedUpdatedItemDto = await this.shoppingCartService.UpdateQty(updateItemDto);

                    UpdateItemTotalPrice(returnedUpdatedItemDto);

                    CartChanged();

                    await JS.InvokeVoidAsync("MakeUpdateQtyButtonVisible", id, false);
                }
                else
                {
                    var item = this.ShoppingCartItems.FirstOrDefault(x => x.Id == id);

                    if(item != null)
                    {
                        item.Qty = 1;
                        item.TotalPrice = item.Price;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        protected async Task UpdateQty_Input(int id)
        {
            await JS.InvokeVoidAsync("MakeUpdateQtyButtonVisible", id, true);
        }

        private void UpdateItemTotalPrice(CartItemDto cartItemDto)
        {
            var item = GetCartItem(cartItemDto.Id);
            if(item != null)
            {
                item.TotalPrice = cartItemDto.Price * cartItemDto.Qty;
            }
        }

        private void CalculateCartSummaryTotals()
        {
            SetTotalPrice();
            SetTotalQuantity();
        }
        private void SetTotalPrice()
        {
            TotalPrice = this.ShoppingCartItems.Sum(p => p.TotalPrice).ToString("");
        }

        private void SetTotalQuantity()
        {
            TotalQuantity = this.ShoppingCartItems.Sum(p => p.Qty);
        }
        protected CartItemDto GetCartItem(int id)
        {
            return ShoppingCartItems.FirstOrDefault(i => i.Id == id);
        }

        private void RemoveCartItem(int id)
        {
            var cartItemDto = GetCartItem(id);
            ShoppingCartItems.Remove(cartItemDto);
        }

        private void CartChanged()
        {
            CalculateCartSummaryTotals();

            shoppingCartService.RaiseEventOnShoppingCartChanged(TotalQuantity);
    }
    }

    
}
