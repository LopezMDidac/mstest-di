﻿using Microsoft.Extensions.DependencyInjection;
using mstest_di.InjectableDependencies;

namespace mstest_di;

[TestClass]
internal class Hooks
{
    internal static ServiceProvider? serviceProvider;
    [AssemblyInitialize]
    public static void AssemblyInitialize(TestContext _)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<IDependencyA, SingletonDependencyA>();
        serviceCollection.AddScoped<IDependencyB, ScopedDependencyB>();
        serviceCollection.AddTransient<IDependencyC, TransientDependencyC>();
        serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [AssemblyCleanup]
    public static void AssemblyCleanup()
    {
        serviceProvider?.Dispose();
    }
}
