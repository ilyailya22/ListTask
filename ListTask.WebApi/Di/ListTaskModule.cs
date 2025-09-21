using Autofac;
using ListTask.BusinessLogic.Abstract;
using ListTask.BusinessLogic.Concrete;
using ListTask.Data;
using ListTask.Data.Abstract;
using ListTask.Data.Concrete;
using ListTask.Service.Abstract;
using ListTask.Service.Concrete;

namespace ListTask.WebApi.Di;

public sealed class ListTaskModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<ListTaskDbContext>()
            .As<IDbContext>()
            .InstancePerLifetimeScope();
        
        builder.RegisterGeneric(typeof(Repository<>))
            .As(typeof(IRepository<>))
            .InstancePerLifetimeScope();
        
        builder.RegisterType<UnitOfWork>()
            .As<IUnitOfWork>()
            .InstancePerLifetimeScope();
        
        builder.RegisterType<UserLogic>()
            .As<IUserLogic>()
            .InstancePerLifetimeScope();
        
        builder.RegisterType<UserService>()
            .As<IUserService>()
            .InstancePerLifetimeScope();
        
        builder.RegisterType<TaskListLogic>()
            .As<ITaskListLogic>()
            .InstancePerLifetimeScope();
        
        builder.RegisterType<TaskListService>()
            .As<ITaskListService>()
            .InstancePerLifetimeScope();
    }
}