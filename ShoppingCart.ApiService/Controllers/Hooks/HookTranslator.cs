using ShoppingCart.Common.Entities;

namespace ShoppingCart.ApiService.Controllers.Hooks
{
    public static class HookTranslator
    {
        public static HookResponse ToResponse(this HookEntity entity)
        {
            if (entity == null)
                return null;

            return new HookResponse
                   {
                       Id = entity.Id,
                       CartId = entity.CartId,
                       Url = entity.Url,
                       CreationDate = entity.CreationDate
                   };
        }
    }
}