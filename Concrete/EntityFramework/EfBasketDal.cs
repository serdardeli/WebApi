using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Concrete.EntityFramework
{
    using System.Linq;
    using System.Linq.Expressions;

    using Core.DataAccess.EntityFramework;
    using Core.Entities.Concrete;

    using DataAccess.Abstract;
    using DataAccess.Concrete.EntityFramework.Contexts;

    using Entities;
    using Entities.Concrete;

    public class EfBasketDal : EfEntityRepositoryBase<Basket, NorthwindContext>, IBasketDal
    {
      /*  NorthwindContext context = new NorthwindContext();
        List<Product> products = context.Products.ToList();
        List<Category> categories = context.Categories.ToList();
        List<Stock> stock = context.Stocks.ToList();
        var query = from produc in products
                    join category in categories on produc.CategoryId equals category.Id into table1
                    from category in table1.DefaultIfEmpty()
                    join stoc in stock on produc.Id equals stoc.ProductId into table2
                    from stoc in table2.DefaultIfEmpty()
                    select new GetAllOfThem { GetProduct = produc, GetCategory = category, GetStock = stoc };*/
    }
}
