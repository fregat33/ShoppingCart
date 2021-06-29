using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace ShoppingCart.ApiService.Controllers.Carts
{
    public class ProductRequest
    {
        /// <summary>
        /// Идентификатор продукта
        /// </summary>
        [Required]
        [BindRequired]
        [JsonRequired]
        public int? Id { get; set; }
        /// <summary>
        /// Название продукта
        /// </summary>
        [Required]
        [BindRequired]
        [JsonRequired]
        public string Name { get; set; }
        /// <summary>
        /// Стоимость продукта
        /// </summary>
        [Required]
        [BindRequired]
        [JsonRequired]
        public decimal? Cost { get; set; }
        /// <summary>
        /// За бонусные очки
        /// </summary>
        [Required]
        [BindRequired]
        [JsonRequired]
        public bool? ForBonusPoints { get; set; }
        /// <summary>
        /// Кол-во в корзине
        /// </summary>
        [Required]
        [BindRequired]
        [JsonRequired]
        public int? Count { get; set; }
    }
}