using Autofac;
using Autofac.Integration.Mvc;
using Randao.Core;
using System.Reflection;
using System.Web.Mvc;

namespace Randao.Web
{
	public static class AutofacConfig
	{
		/// <summary>
		/// 注册依赖注入
		/// </summary>
		public static void RegisterIOC()
		{
			var builder = new ContainerBuilder();

			builder.RegisterModule(new AutofacWebTypesModule());
			builder.RegisterModule(new ServiceModule());

			builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly());
			builder.RegisterControllers(Assembly.GetExecutingAssembly());
			builder.RegisterModelBinders(Assembly.GetExecutingAssembly());
			builder.RegisterModelBinderProvider();
			builder.RegisterFilterProvider();

			DependencyResolver.SetResolver(new AutofacDependencyResolver(builder.Build()));
		}
	}
}