using System;

namespace ShoppingCart.ApiService.Controllers.Hooks
{
    public class HookResponse
    {
        /// <summary>
        /// Идентификатор веб-хука
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Идентификатор корзины
        /// </summary>
        public Guid CartId { get; set; }
        /// <summary>
        /// Ссылка веб-хук
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTimeOffset CreationDate { get; set; }
    }
}