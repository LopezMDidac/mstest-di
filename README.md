# MSTest with Dependency Injection
This project demonstrates how to use dependency injection in a test project with MSTest in 3 steps.

## How to
### Pre-conditions
* An MSTest project
* Objects to be injected and their interfaces
* Added the NuGet packages:
    - Microsoft.Extensions.DependencyInjection
    - Microsoft.Extensions.DependencyInjection.Abstractions

### Step 1. Registering the injectable elements
You need to create the dependencies provider and register them at some place early in the test execution life cycle.

This code uses the DependencyInjection package from Microsoft. 
```xml
<ItemGroup>
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="8.0.0" />
    <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="8.0.1" />
    ...
</ItemGroup>
```

This code makes service creation and registration at the very beginning and very end of test execution. Using MSTest hooks (Hooks.cs)
```csharp
[TestClass]
public class Hooks
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
```
* The class uses the attribute ``[TestClass]`` to allow MSTest to identify this class as part of the execution life cycle.
* The class uses the attributes ``[AssemblyInitialize]`` and ``[AssemblyCleanup]`` to hook the initialization and disposal of the ``serviceProvider`` to the start and end of the test execution.
* After the AssemblyInitialize hook, ``serviceProvider`` is a ``ServiceProvider`` with the dependencies registered using the designated scopes.
* Note that ``Hooks.serviceProvider`` is a static property that could be used elsewhere in the code.

### Step 2. Build test method attributes
Once the service provider is defined, TestAttributes from MSTest should be extended to use it. To do that, we can take advantage of the reflection capabilities provided by .NET (Decorators.cs)

```csharp
public class TestMethodDI : TestMethodAttribute
{
    public override TestResult[] Execute(ITestMethod testMethod)
    {
        object[] injectedArgs = new object[testMethod.ParameterTypes.Length];
        var serviceProvider = Hooks.serviceProvider;
        using (var scope = serviceProvider!.CreateScope())
        {
            for (var i = 0; i < testMethod.ParameterTypes.Length; i++)
            {
                injectedArgs[i] = scope.ServiceProvider.GetService(testMethod.ParameterTypes[i])!;
            }
        }
        return new[] { testMethod.Invoke(injectedArgs) };
    }
}
```
* ``TestMethodAttribute`` receives the decorated ``testMethod`` as a parameter in the execute command.
* By overriding the ``Execute`` method, we are able to resolve the dependencies using our service provider defined in Step 1.
* A ``scope`` is created to resolve the dependencies of each test, so the scoped dependencies are independent from one test to another.
* The type of the parameter is read and used to instantiate the dependency using reflection.

### Step 3. Tests with dependency injection
Now it is time to create tests with the capability to receive as parameters the types registered in the service provider (tests folder).
```csharp
[TestMethodDI]
public void TestAllDependencies(IDependencyA myDependencyA, IDependencyB myDependencyB, IDependencyC myDependencyC)
{
    Assert.AreEqual("DependencyA", myDependencyA.Name);
    Assert.AreEqual("DependencyB", myDependencyB.Name);
    Assert.AreEqual("DependencyC", myDependencyC.Name);
}
```
* ``[TestMethodDI]`` is used to define the test case.
* Using the parameter type, the test receives an instance within the defined scope.

Happy Testing!