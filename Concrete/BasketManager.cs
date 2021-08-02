using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Concrete
{
    using System.Collections;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;

    using Business.Abstract;
    using Business.Constants;
    using Business.ValidationRules.FluentValidation;

    using Core.Aspects.Autofac.Caching;
    using Core.Aspects.Autofac.Logging;
    using Core.Aspects.Autofac.Performance;
    using Core.Aspects.Autofac.Transaction;
    using Core.Aspects.Autofac.Validation;
    using Core.CrossCuttingConcerns.Logging.Log4Net.Loggers;
    using Core.Utilities.Business;
    using Core.Utilities.Results;

    using DataAccess.Abstract;
    using DataAccess.Concrete.EntityFramework.Contexts;

    using Entities;
    using Entities.Concrete;

    using Microsoft.EntityFrameworkCore;

    public class BasketManager:IBasketService
    {
     //   private IStockDal _stockDal;

        private IProductDal _productDal;
        private IBasketDal _basketDal;
        private IStockService _stockService;

        // private ICategoryService _categoryService;



        public BasketManager(IBasketDal basketDal, IStockService stockService)
        {
            _basketDal = basketDal;
            _stockService = stockService;
            // this._productDal = productDal;
            //this._stockDal = stockDal;

        }

        public IDataResult<Basket> GetById(int basketId)
        {
            return new SuccessDataResult<Basket>(this._basketDal.Get(b => b.Id == basketId));
        }

        [PerformanceAspect(5)]
        public IDataResult<List<Basket>> GetList()
        {
           // Thread.Sleep(5000);
            return new SuccessDataResult<List<Basket>>(this._basketDal.GetList().ToList());
        }

        //[SecuredOperation("Product.List,Admin")]
        [LogAspect(typeof(FileLogger))]
        [CacheAspect(duration: 10)]
        public IDataResult<List<Basket>> GetListByCategory(int basketId)
        {
            return new SuccessDataResult<List<Basket>>(this._basketDal.GetList(b => b.Id == basketId).ToList());
        }
        Stock stock = new Stock();
        Product products = new Product();
     
        Stock stock2 = new Stock();
        List<Product> product2;
        [ValidationAspect(typeof(ProductValidator), Priority = 1)]
        [CacheRemoveAspect("IProductService.Get")]
        public IResult AddProduct(Basket basket)
        {
            this._basketDal.Add(basket);
            var stock2 = _stockService.GetByProductId(basket.ProductId).Data;
            stock2.Quantity = stock2.Quantity - basket.Quantity;
            _stockService.Update(stock2);
            return new SuccessResult(Messages.BasketAddded);
        }
        public IResult Delete(Basket basket)
        {
            this._basketDal.Delete(basket);
            return new SuccessResult(Messages.BasketDeleted);
        }

        public IResult Update(Basket basket)
        {

            this._basketDal.Update(basket);
            return new SuccessResult(Messages.BasketUpdated);
        }
    }
}
