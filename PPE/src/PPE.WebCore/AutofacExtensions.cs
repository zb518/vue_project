using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PPE.WebCore
{

    public static class AutofacExtensions
    {

        /// <summary>
        /// 添加 Autofac 依赖注入
        /// </summary>
        /// <param name="host"></param>
        public static void AddAutofacRegister(this IHostBuilder host)
        {
            host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
          .ConfigureContainer<ContainerBuilder>(x => x.RegisterModule(new AutoRegisterModule()));
        }
    }
}