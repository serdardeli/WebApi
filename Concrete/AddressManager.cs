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

    public class AddressManager
    {

        private IAddressDal _addressDal;
        private ICategoryService _categoryService;

        public AddressManager(IAddressDal addressDal, ICategoryService categoryService)
        {
            this._addressDal = addressDal;
            _categoryService = categoryService;
        }

        public IDataResult<Address> GetById(int addressId)
        {
            return new SuccessDataResult<Address>(this._addressDal.Get(a => a.Id == addressId));
        }

        [PerformanceAspect(5)]
        public IDataResult<List<Address>> GetList()
        {
            Thread.Sleep(5000);
            return new SuccessDataResult<List<Address>>(this._addressDal.GetList().ToList());
        }

        //[SecuredOperation("Product.List,Admin")]
        [LogAspect(typeof(FileLogger))]
        [CacheAspect(duration: 10)]
        public IDataResult<List<Address>> GetListByCategory(int addressId)
        {
            return new SuccessDataResult<List<Address>>(this._addressDal.GetList(a => a.Id == addressId).ToList());
        }


        [ValidationAspect(typeof(ProductValidator), Priority = 1)]
        [CacheRemoveAspect("IProductService.Get")]
        public IResult Add(Address address)
        {
            IResult result = BusinessRules.Run(CheckIfProductNameExists(address.title), CheckIfCategoryIsEnabled());

            if (result != null)
            {
                return result;
            }
            this._addressDal.Add(address);
            return new SuccessResult(Messages.AddressAdded);
        }

        private IResult CheckIfProductNameExists(string addressTitle)
        {

            var result = this._addressDal.GetList(p => p.title == addressTitle).Any();
            if (result)
            {
                return new ErrorResult(Messages.ProductNameAlreadyExists);
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

        public IResult Delete(Address address)
        {
            this._addressDal.Delete(address);
            return new SuccessResult(Messages.AddressDeleted);
        }

        public IResult Update(Address address)
        {

            this._addressDal.Update(address);
            return new SuccessResult(Messages.AddressUpdated);
        }

        [TransactionScopeAspect]
        public IResult TransactionalOperation(Address address)
        {
            this._addressDal.Update(address);
            //_productDal.Add(product);
            return new SuccessResult(Messages.ProductUpdated);
        }
    }
}
