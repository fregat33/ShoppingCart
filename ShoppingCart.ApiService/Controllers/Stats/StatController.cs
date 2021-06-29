using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ShoppingCart.Common;

namespace ShoppingCart.ApiService.Controllers.Stats
{
    [ApiController]
    [Route("stats")]
    public class StatController: Controller
    {
        private readonly IStatsRepository _statsRepository;
        public StatController(IStatsRepository statsRepository)
        {
            _statsRepository = statsRepository;
        }

        /// <summary>
        /// Получить статистики за период
        /// </summary>
        /// <param name="fromDate">Дата начала, включительно</param>
        /// <param name="toDate">Дата завершения</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ItemsResponse<StatResponse>>> ReadStatsAsync([FromQuery, Required, BindRequired] DateTimeOffset? fromDate, 
                                                                                    [FromQuery, Required, BindRequired] DateTimeOffset? toDate)
        {
            var stats = await _statsRepository.ReadByPeriodAsync(fromDate, toDate);
            return new ItemsResponse<StatResponse>(stats.Select(s => s.ToResponse()));
        }
    }
}