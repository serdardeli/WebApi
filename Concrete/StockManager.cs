using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Concrete
{
    using System.Linq;
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

    using Entities.Concrete;

    public class StockManager : IStockService
    {
        private IStockDal _stockDal;
        //private ICategoryService _categoryService;

        public StockManager(IStockDal stockDal)
        {
            this._stockDal = stockDal;
           
        }

        public IDataResult<Stock> GetById(int stockId)
        {
            return new SuccessDataResult<Stock>(this._stockDal.Get(s => s.Id == stockId));
        }
        public IDataResult<Stock> GetByProductId(int productId)
        {
            return new SuccessDataResult<Stock>(this._stockDal.Get(s => s.ProductId == productId));
        }

        // [PerformanceAspect(5)]
        public IDataResult<List<Stock>> GetList()
        {
           // Thread.Sleep(5000);
            return new SuccessDataResult<List<Stock>>(this._stockDal.GetList().ToList());
        }

        //[SecuredOperation("Product.List,Admin")]
        [LogAspect(typeof(FileLogger))]
        [CacheAspect(duration: 10)]

        [ValidationAspect(typeof(ProductValidator), Priority = 1)]
        [CacheRemoveAspect("IProductService.Get")]
        public IResult Add(Stock stock)
        {
         
            this._stockDal.Add(stock);
            return new SuccessResult(Messages.StockAdded);
        }

        public IResult Delete(Stock stock)
        {
            this._stockDal.Delete(stock);
            return new SuccessResult(Messages.StockDeleted);
        }

        public IResult Update(Stock stock)
        {

            this._stockDal.Update(stock);
            return new SuccessResult(Messages.StockUpdated);
        }
    }
}
