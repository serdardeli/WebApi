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

    public class CustomerManager:ICustomerService
    {

        private ICustomerDal _customerDal;
        private ICategoryService _categoryService;

        public CustomerManager(ICustomerDal customerDal, ICategoryService categoryService)
        {
            this._customerDal = customerDal;
            _categoryService = categoryService;
        }

        public IDataResult<Customer> GetById(int customerId)
        {
            return new SuccessDataResult<Customer>(this._customerDal.Get(c => c.Id == customerId));
        }

        [PerformanceAspect(5)]
        public IDataResult<List<Customer>> GetList()
        {
            Thread.Sleep(5000);
            return new SuccessDataResult<List<Customer>>(this._customerDal.GetList().ToList());
        }

        //[SecuredOperation("Product.List,Admin")]
        [LogAspect(typeof(FileLogger))]
        [CacheAspect(duration: 10)]
        public IDataResult<List<Customer>> GetListByCategory(int customerId)
        {
            return new SuccessDataResult<List<Customer>>(this._customerDal.GetList(c =>c.Id == customerId).ToList());
        }


        [ValidationAspect(typeof(ProductValidator), Priority = 1)]
        [CacheRemoveAspect("IProductService.Get")]
        public IResult Add(Customer customer)
        {
            IResult result = BusinessRules.Run(CheckIfProductNameExists(customer.FirstName), CheckIfCategoryIsEnabled());

            if (result != null)
            {
                return result;
            }
            this._customerDal.Add(customer);
            return new SuccessResult(Messages.CustomerAdded);
        }

        private IResult CheckIfProductNameExists(string customerName)
        {

            var result = this._customerDal.GetList(c => c.FirstName== customerName).Any();
            if (result)
            {
                return new ErrorResult(Messages.CustomerNameAlreadyExist);
            }

            return new SuccessResult();
        }

        private IResult CheckIfCategoryIsEnabled()
        {
            var result = _categoryService.GetList();
            if (result.Data.Count < 10)
            {
                return new ErrorResult(Messages.ProductNameAlreadyExists);
            }

            return new SuccessResult();
        }

        public IResult Delete(Customer customer)
        {
            this._customerDal.Delete(customer);
            return new SuccessResult(Messages.CustomerDeleted);
        }

        public IResult Update(Customer customer)
        {

            this._customerDal.Update(customer);
            return new SuccessResult(Messages.CustomerUpdated);
        }

        [TransactionScopeAspect]
        public IResult TransactionalOperation(Customer customer)
        {
            this._customerDal.Update(customer);
            //_productDal.Add(product);
            return new SuccessResult(Messages.CustomerUpdated);
        }
    }
}
