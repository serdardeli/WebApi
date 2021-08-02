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

    using Entities;
    using Entities.Concrete;

    public class OrderManager:IOrderService
    {
        private IOrderDal _orderDal;
       // private ICategoryService _categoryService;

        public OrderManager(IOrderDal orderDal)
        {
            this._orderDal = orderDal;
          
        }

        public IDataResult<Order> GetById(int orderId)
        {
            return new SuccessDataResult<Order>(this._orderDal.Get(o => o.Id == orderId));
        }

        [PerformanceAspect(5)]
        public IDataResult<List<Order>> GetList()
        {
            Thread.Sleep(5000);
            return new SuccessDataResult<List<Order>>(this._orderDal.GetList().ToList());
        }

        //[SecuredOperation("Product.List,Admin")]
        [LogAspect(typeof(FileLogger))]
        [CacheAspect(duration: 10)]
        public IDataResult<List<Order>> GetListByCategory(int orderId)
        {
            return new SuccessDataResult<List<Order>>(this._orderDal.GetList(o => o.Id ==orderId ).ToList());
        }


        [ValidationAspect(typeof(ProductValidator), Priority = 1)]
        [CacheRemoveAspect("IOrderService.Get")]
        public IResult Add(Order order)
        {
         /*   IResult result = BusinessRules.Run(CheckIfProductNameExists(order.ShipName), CheckIfCategoryIsEnabled());

            if (result != null)
            {
                return result;
            }*/
            this._orderDal.Add(order);
            return new SuccessResult(Messages.OrderAdded);
        }

        public IResult Delete(Order order)
        {
            this._orderDal.Delete(order);
            return new SuccessResult(Messages.OrderDeleted);
        }

        /*  private IResult CheckIfProductNameExists(string shipName)
        {

            var result = this._orderDal.GetList(o => o.ShipName == shipName).Any();
            if (result)
            {
                return new ErrorResult(Messages.ShipNameAlreadyExist);
            }

            return new SuccessResult();
        }

        private IResult CheckIfCategoryIsEnabled()
        {
            var result = _categoryService.GetList();
            if (result.Data.Count < 10)
            {
                return new ErrorResult(Messages.ShipNameAlreadyExist);
            }

            return new SuccessResult();
        }

        public IResult Delete(Order order)
        {
            this._orderDal.Delete(order);
            return new SuccessResult(Messages.OrderDeleted);
        }*/

        public IResult Update(Order order)
        {

            this._orderDal.Update(order);
            return new SuccessResult(Messages.OrderUpdated);
        }

       /* [TransactionScopeAspect]
        public IResult TransactionalOperation(Order order)
        {
            this._orderDal.Update(order);
            //_productDal.Add(product);
            return new SuccessResult(Messages.OrderUpdated);
        }*/
    }
}
