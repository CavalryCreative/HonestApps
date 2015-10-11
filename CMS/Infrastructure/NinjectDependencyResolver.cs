using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Dependencies;
using System.Web.Mvc;
using Ninject;
using CMS.Infrastructure.Abstract;
using CMS.Infrastructure.Concrete;
using CMS.Infrastructure.Entities;

namespace CMS.Infrastructure
{
    public class NinjectDependencyResolver : System.Web.Mvc.IDependencyResolver
    {
        private IKernel kernel;

        public NinjectDependencyResolver(IKernel kernalParam)
        {
            kernel = kernalParam;
            AddBindings();
        }

        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }

        private void AddBindings()
        {
            //kernel.Bind<ICV>().To<EFCV>();
            //kernel.Bind<IDiscipline>().To<EFDiscipline>();
            //kernel.Bind<IJobTitle>().To<EFJobTitle>();
            //kernel.Bind<IJobVacancy>().To<EFJobVacancy>();
            //kernel.Bind<INews>().To<EFNews>();
            //kernel.Bind<IRegion>().To<EFRegion>();
            //kernel.Bind<ISiteImage>().To<EFSiteImage>();
            //kernel.Bind<ISitePage>().To<EFSitePage>();
            //kernel.Bind<ISiteSetting>().To<EFSiteSetting>();
            //kernel.Bind<ISubRegion>().To<EFSubRegion>();
            //kernel.Bind<ITestimonial>().To<EFTestimonial>();
            //kernel.Bind<IBlog>().To<EFBlog>();
            //kernel.Bind<IFunctions>().To<Functions>();
            //kernel.Bind<ICountry>().To<EFCountry>();
        }

        public IDependencyScope BeginScope()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}