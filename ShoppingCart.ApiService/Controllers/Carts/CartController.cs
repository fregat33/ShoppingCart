using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Common;

namespace ShoppingCart.ApiService.Controllers.Carts
{
    [ApiController]
    [Route("carts")]
    public class CartController : Controller
    {
        private readonly ICartRepository _cartRepository;
        private readonly ISystemClock _systemClock;

        public CartController(ICartRepository cartRepository, ISystemClock systemClock)
        {
            _cartRepository = cartRepository;
            _systemClock = systemClock;
        }
        
        /// <summary>
        /// Получение коризны за период времени
        /// </summary>
        /// <param name="fromDate">Дата начала поиска, включительно</param>
        /// <param name="toDate">Дата окончания поиска</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ItemsResponse<CartResponse>>> GetByPeriodAsync([FromQuery] DateTimeOffset? fromDate, [FromQuery] DateTimeOffset? toDate)
        {
            var cartEntities = await _cartRepository.ReadCartsByPeriodAsync(fromDate, toDate);
            return new ItemsResponse<CartResponse>(cartEntities.Select(c => c.ToResponse()));
        }

        /// <summary>
        /// Получение существующей коризны
        /// </summary>
        /// <param name="id">Идентификатор корзины</param>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<CartResponse>> GetAsync([FromRoute] Guid id)
        {
            var cartEntity = await _cartRepository.ReadByIdAsync(id);
            if (cartEntity == null)
                return NoContent();

            return cartEntity.ToResponse();
        }

        /// <summary>
        /// Создание новой корзины
        /// </summary>
        /// <param name="request">Продукты в корзине</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<CartResponse>> PostAsync(CartRequest request)
        {
            var (_, items, cost, hasBonus, creationDate) = request.ToEntity(Guid.Empty, _systemClock.UtcNow);
            var savedCartEntity = await _cartRepository.CreateAsync(
                                      cost,
                                      items,
                                      hasBonus,
                                      creationDate);

            return savedCartEntity.ToResponse(); 
        }

        /// <summary>
        /// Обновление содержимого корзины
        /// </summary>
        /// <param name="id">Идентификатор корзины</param>
        /// <param name="request">Продукты в корзине</param>
        /// <returns></returns>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status410Gone)]
        public async Task<ActionResult> PutAsync([FromRoute] Guid id, [FromBody] CartRequest request)
        {
            var cartEntity = request.ToEntity(id, _systemClock.UtcNow);
            var success = await _cartRepository.UpdateAsync(cartEntity);
            return success ? new OkResult() : StatusCode(410);
        }

        /// <summary>
        /// Удаление корзины
        /// </summary>
        /// <param name="id">Идентификатор корзины</param>
        /// <returns></returns>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status410Gone)]
        public async Task<ActionResult> DeleteAsync([FromRoute] Guid id)
        {
            var success = await _cartRepository.DeleteAsync(id);
            return success ? StatusCode(204) : StatusCode(410);
        }
    }
}