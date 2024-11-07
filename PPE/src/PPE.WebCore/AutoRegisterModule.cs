using Autofac;
using PPE.Core;
using PPE.Model.Shared;
using System.Reflection;

namespace PPE.WebCore
{
    /// <summary>
    /// 调用 Autofac 依赖注入
    /// </summary>
    public class AutoRegisterModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {

            //builder.RegisterType<ConfigManager>();
            builder.RegisterType<IdentityFactory>();
            builder.RegisterType<OperationErrorDescriber>();

            builder.RegisterAssemblyTypes(Assembly.Load("PPE.IDAL"), Assembly.Load("PPE.DAL"))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope()
            .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

            builder.RegisterAssemblyTypes(Assembly.Load("PPE.BLL"))
            .InstancePerLifetimeScope()
            .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
        }
    }
}
