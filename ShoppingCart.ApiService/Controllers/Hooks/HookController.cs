using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ShoppingCart.Common;
using ShoppingCart.Common.Entities;

namespace ShoppingCart.ApiService.Controllers.Hooks
{
    [ApiController]
    [Route("hooks")]
    public class HookController : Controller
    {
        private readonly IHooksRepository _hooksRepository;
        private readonly ISystemClock _systemSystemClock;

        public HookController(IHooksRepository hooksRepository, ISystemClock systemSystemClock)
        {
            _hooksRepository = hooksRepository;
            _systemSystemClock = systemSystemClock;
        }
        
        /// <summary>
        /// Получить все веб-хуки для корзины
        /// </summary>
        /// <param name="cartId">Идентификатор корзины</param>
        /// <returns></returns>
        [HttpGet("{cartId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ItemsResponse<HookResponse>>> GetByCartIdAsync([FromRoute] Guid cartId)
        {
            var hookEntities = await _hooksRepository.ReadByCartIdAsync(cartId);
            return new ItemsResponse<HookResponse>(hookEntities.Select(h => h.ToResponse()));
        }
        
        /// <summary>
        /// Получить все веб-хуки по состоянию
        /// </summary>
        /// <param name="state">Состояние веб-хука</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ItemsResponse<HookResponse>>> GetByStateAsync([FromQuery, Required, BindRequired] HookState? state)
        {
            if (!state.HasValue)
                return BadRequest($"'{nameof(state)}' - required!");
            
            var hookEntities = await _hooksRepository.ReadByStateAsync(state.Value);
            return new ItemsResponse<HookResponse>(hookEntities.Select(h => h.ToResponse()));
        }
        
        /// <summary>
        /// Создать вебхук для корзины
        /// </summary>
        /// <param name="сartId">Идентификатор корзины</param>
        /// <param name="request">Веб хук</param>
        /// <returns></returns>
        [HttpPost]
        [Route("{cartId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<HookResponse>> CreateAsync([FromRoute] Guid cartId, [FromBody] HookRequest request)
        {
            var createdEntity = await _hooksRepository.CreateAsync(cartId, request.Url, request.Payload, _systemSystemClock.UtcNow);
            return createdEntity.ToResponse();
        }
        
        /// <summary>
        /// Удалить веб-хук
        /// </summary>
        /// <param name="id">Идентификатор веб-хука</param>
        /// <returns></returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> CreateAsync([FromRoute] int id)
        {
            var success = await _hooksRepository.DeleteAsync(id);
            return success ? new OkResult() : StatusCode(204);
        }
    }
}