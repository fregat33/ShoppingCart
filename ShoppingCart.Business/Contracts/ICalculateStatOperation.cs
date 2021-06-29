using System;
using ShoppingCart.Business.Operations;

namespace ShoppingCart.Business.Contracts
{
    public interface ICalculateStatOperation
    {
        CalculateStatOperation Create(DateTimeOffset fromDate);
    }
}