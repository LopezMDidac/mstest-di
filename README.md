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
Once the service provider is defined, TestAttributes from MSTest should be extended to use it. To do that, we can take advantage of the reflection capabilities provided by .NET (Attributes.cs)

```csharp
internal class TestMethodDI : TestMethodAttribute
{
    public override TestResult[] Execute(ITestMethod testMethod)
    {
        var nParameters = testMethod.ParameterTypes?.Length ?? 0;
        if (nParameters != 0)
        {
            object?[] injectedArgs = new object[nParameters];
            var serviceProvider = Hooks.serviceProvider;
            using (var scope = serviceProvider!.CreateScope())
            {

                for (var i = 0; i < nParameters; i++)
                {
                    injectedArgs[i] = scope.ServiceProvider.GetService(testMethod.ParameterTypes![i].ParameterType)!;
                }
            }
            return [testMethod.Invoke(injectedArgs!)];
        }
        else
        {
            return base.Execute(testMethod);
        }
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


## Extended examples

### DataTestMethod with DI
Is it possible to do the same for DataTestMethod adding some extra code.
```csharp
internal class DataTestMethodDI : DataTestMethodAttribute
{
    public override TestResult[] Execute(ITestMethod testMethod)
    {
        var nArguments = testMethod.Arguments?.Length ?? 0;
        var nParameters = testMethod.ParameterTypes?.Length ?? 0;
        if (nArguments < nParameters)
        {
            object?[] injectedArgs = new object[nParameters];
            using (var scope = Hooks.serviceProvider!.CreateScope())
            {
                var argsCount = 0;
                for (var i = 0; i < nParameters ; i++)
                {
                    var paramType = testMethod.ParameterTypes![i].ParameterType;
                    if (paramType == testMethod.Arguments?[argsCount]!.GetType())
                    {
                        injectedArgs[i] = testMethod.Arguments[argsCount]!;
                        argsCount++;
                    }
                    else
                    {
                        injectedArgs[i] = scope.ServiceProvider.GetService(testMethod.ParameterTypes[i].ParameterType);
                    }
                }
            }
            return [testMethod.Invoke(injectedArgs!)];
        }
        else
        {
            return base.Execute(testMethod);
        }
    }
}

```
* ìnheriting from ``DataTestMethodAttribute`` We can define the behaviour for ``DataTestMethod``
* Analyze and compare the arguments injected by the test framework and fulfill the gaps with the ServiceProvider

And do the magic for the test cases
```csharp
    [DataTestMethodDI]
    [DataRow(1, "2", 3.4)]
    [DataRow(2, "3", 4.6)]
    public void DataTestDIWithRowsUnsorted(int a, string b, IDependencyC myDependency, double c)
    {
        Assert.AreEqual("DependencyC", myDependency.Name);
        Assert.IsInstanceOfType(a, typeof(int));
        Assert.IsInstanceOfType(b, typeof(string));
        Assert.IsInstanceOfType(c, typeof(double));
    }
```
* Regardless the order the dependency injection is fullfilling with custom dependencies inside the data provided

### Multiple Providers 
With more code is it possible to combine multiple providers. For instance a Global one (in the hooks) and a local one (living only in the test)

```csharp
internal class TestMethodDIMultipleProviders: TestMethodAttribute
{
    public override TestResult[] Execute(ITestMethod testMethod)
    {
        var nParameters = testMethod.ParameterTypes?.Length ?? 0;
        if (nParameters != 0)
        {
            object?[] injectedArgs = new object[nParameters];
            var localServiceCollection = (IServiceCollection?) Type.GetType(testMethod.TestClassName)?
                .GetFields()?
                .First(x => x.FieldType == typeof(IServiceCollection))?
                .GetValue(null);
            var serviceProvider = localServiceCollection?.BuildServiceProvider();
            using (var localScope = serviceProvider?.CreateScope())
            using (var globalScope = Hooks.serviceProvider?.CreateScope())
            {

                for (var i = 0; i < nParameters; i++)
                {
                    injectedArgs[i] = localScope?.ServiceProvider.GetService(testMethod.ParameterTypes![i].ParameterType);
                    if (injectedArgs[i] == null)
                        injectedArgs[i] = globalScope?.ServiceProvider.GetService(testMethod.ParameterTypes![i].ParameterType)!;
                }
            }
            serviceProvider?.Dispose();
            return [testMethod.Invoke(injectedArgs!)];
        }
        else
        {
            return base.Execute(testMethod);
        }
    }
}
```
* By reflection, the attribute looks for a static ``IServiceCollection`` inside the test class.
* If found, the attribute generate a local Service provider for this collection within the scope of the test.
* The injection is performed using the local service provider or the global service if the dependency is not found in the local
* After generate the injection parameters the local serviceProvider is disposed

Finally, the test class must declare the services for the test and initialize at class initialization time.
```csharp

[TestClass]
public class TestDependencyInjectionGlobalLocal
{
    public static IServiceCollection? serviceCollection;

    [ClassInitialize]
    public static void BuildServiceProvider(TestContext _)
    {
        serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient(typeof(TransientDependencyC));
        serviceCollection.AddTransient<IDependencyA,SingletonDependencyAA>();
    }


    [TestMethodDIMultipleProviders]
    public void TestDISingleDependencyA(IDependencyA myDependency)
    {
        Assert.AreEqual("DependencyAA", myDependency.Name);
    }
    ...
}
```
* Class ``TestDependencyInjectionGlobalLocal`` has a static field ``serviceCollection``
* It is initialized within the ``[ClassInitialize]`` method. Those are the services used by the tests within this class.
* Tests will receive the local dependency or the global or null if the type is not registered.

## Conclusion
This sample demonstrates how to inject dependencies into tests by extending MSTest attributes. Multiple implementations are possible, and these examples could be extended to include more complex injections/scopes. Implementing your tests following this approach will provide:

* Tests focus on functionality, avoiding annoying initializations.
* Tests with better readability.
* Resource sharing for elements that persist throughout the entire execution (such as database managers, loggers, etc.).
* Flexibility in types and instance injection.

Happy Testing!