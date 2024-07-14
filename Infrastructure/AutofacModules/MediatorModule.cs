using Amazon.Util.Internal.PlatformServices;
using Application.Mappings;
using Application.Users.Commands;
using AutoMapper;
using System.Reflection;
namespace Infrastructure.AutofacModules
{
    public class MediatorModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly)
            .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(typeof(InsertUserCommand).GetTypeInfo().Assembly)
                   .AsClosedTypesOf(typeof(IRequestHandler<,>));

            builder.Register(context => new MapperConfiguration(cfg =>
            {
                //Register Mapper Profile
                cfg.AddProfile<AutoMapping>();
            }
            )).AsSelf().SingleInstance();

            builder.Register(c =>
            {
                //This resolves a new context that can be used later.
                var context = c.Resolve<IComponentContext>();
                var config = context.Resolve<MapperConfiguration>();
                return config.CreateMapper(context.Resolve);
            })
            .As<IMapper>()
            .InstancePerLifetimeScope();

            builder.Register<ServiceFactory>(context =>
                {
                    var componentContext = context.Resolve<IComponentContext>();
                    return t => { object o; return componentContext.TryResolve(t, out o) ? o : null; };
                });
        }
    }
}
