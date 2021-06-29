using System;
using ShoppingCart.Business.Operations;

namespace ShoppingCart.Business.Contracts
{
    public interface ICleanOperationFactory
    {
        CleanOperation Create(DateTimeOffset? toDate = null);
    }
}