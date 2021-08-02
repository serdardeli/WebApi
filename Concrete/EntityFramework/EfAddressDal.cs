using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Concrete.EntityFramework
{
    using System.Linq;

    using Core.DataAccess.EntityFramework;
    using Core.Entities.Concrete;

    using DataAccess.Abstract;
    using DataAccess.Concrete.EntityFramework.Contexts;

    using Entities.Concrete;

    public class EfAddressDal : EfEntityRepositoryBase<Address, NorthwindContext>, IAddressDal
    {

      /*  public List<OperationClaim> GetClaims(Address address)
        {
            using (var context = new NorthwindContext())
            {
                var result = from address in context.Adresses
                             join city in context.Cities on address.Id equals
                                 userOperationClaim.OperationClaimId
                             where userOperationClaim.UserId == user.Id
                             select new OperationClaim { Id = operationClaim.Id, Name = operationClaim.Name };
                return result.ToList();

            }


        }*/
    }


}


