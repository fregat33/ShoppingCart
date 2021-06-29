
using System.Collections.Generic;

namespace ShoppingCart.ApiService.Controllers
{
    public class ItemsResponse<T>
    {
        public IEnumerable<T> Items { get; }

        public ItemsResponse(IEnumerable<T> items)
        {
            Items = items;
        }
    }
}