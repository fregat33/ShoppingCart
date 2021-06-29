using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ShoppingCart.ApiService.Controllers.Hooks
{
    public class HookRequest
    {
        /// <summary>
        /// Ссылка веб-хук
        /// </summary>
        [Required]
        [JsonRequired]
        public string Url { get; set; }
        /// <summary>
        /// Содержимое
        /// </summary>
        [Required]
        [JsonRequired]
        public string Payload { get; set; } 
    }
}