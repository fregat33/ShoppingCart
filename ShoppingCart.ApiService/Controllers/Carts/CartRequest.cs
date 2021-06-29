using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.ApiService.Controllers.Carts
{
    public class CartRequest
    {
        /// <summary>
        /// Продукты в корзине
        /// </summary>
        [Required]
        [MinLength(1)]
        public IReadOnlyCollection<ProductRequest> Items { get; set; }
    }
}