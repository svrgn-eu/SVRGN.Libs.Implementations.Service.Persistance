using Microsoft.Extensions.DependencyInjection;
using SVRGN.Libs.Contracts.Base;
using SVRGN.Libs.Contracts.Service.Base;
using SVRGN.Libs.Contracts.Service.Logging;
using SVRGN.Libs.Contracts.Service.Object;
using SVRGN.Libs.Contracts.Service.Persistance;
using SVRGN.Libs.Contracts.Base.IO;
using SVRGN.Libs.Implementations.Base.IO.Windows;
using SVRGN.Libs.Implementations.DependencyInjection;
using SVRGN.Libs.Implementations.Service.ConsoleLogger;
using SVRGN.Libs.Implementations.Service.Object;
using SVRGN.Libs.Implementations.Service.Persistance.Windows;

namespace SVRGN.Libs.Implementations.Service.Persistance.Tests
{
    public static class TestBootStrapper
    {
        //TODO: seperate BootStrapper from Base project and upload into nuget
        #region Properties

        #endregion Properties

        #region Methods

        #region RegisterWithMemoryPersistance
        public static void RegisterWithMemoryPersistance(IServiceCollection services)
        {
            if (!DiContainer.SetServiceCollection(services))
            {
                services = DiContainer.GetServiceCollection();
            }

            services.AddSingleton<ILogService, DebugLogService>();
            services.AddSingleton<IPersistanceService, MemoryPersistanceService>();
            services.AddSingleton<IObjectService, ObjectService>();
            TestBootStrapper.RegisterEntities(services);

            DiContainer.SetServiceProvider(services.BuildServiceProvider());
        }
        #endregion RegisterWithMemoryPersistance

        #region RegisterWithFilePersistance
        public static void RegisterWithFilePersistance(IServiceCollection services)
        {
            if (!DiContainer.SetServiceCollection(services))
            {
                services = DiContainer.GetServiceCollection();
            }

            services.AddSingleton<ILogService, DebugLogService>();
            services.AddSingleton<IPersistanceService, FilePersistanceService>();
            services.AddTransient<IFileIOWrapper>(x => { return FilePersistanceServiceTests.BuildMock().Object; });
            services.AddSingleton<IObjectService, ObjectService>();
            TestBootStrapper.RegisterEntities(services);

            DiContainer.SetServiceProvider(services.BuildServiceProvider());
        }
        #endregion RegisterWithFilePersistance

        #region RegisterEntities: registers special classes, like AI stuff
        /// <summary>
        /// registers special classes, like AI stuff
        /// </summary>
        /// <param name="services"></param>
        private static void RegisterEntities(IServiceCollection services)
        {
            services.AddTransient<IPersistanceContainer, PersistanceContainer>();
            services.AddTransient<IPersistanceEndpoint, PersistanceEndpoint>();
            services.AddTransient<IPersistancePath, PersistancePath>();
            services.AddTransient<IPersistanceValue, PersistanceValue>();
        }
        #endregion RegisterEntities

        #region RegisterEntitiesForWindowsOnly
        private static void RegisterEntitiesForWindowsOnly(IServiceCollection services)
        {
            services.AddTransient<IFileIOWrapper, FileIOWrapper>();
        }
        #endregion RegisterEntitiesForWindowsOnly

        #endregion Methods
    }
}
