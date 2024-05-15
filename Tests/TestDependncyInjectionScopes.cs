using Microsoft.Extensions.DependencyInjection;
using mstest_di.InjectableDependencies;

namespace mstest_di.Tests;




[TestClass]
public class TestDependncyInjectionScopes
{
    static IDependencyB? ScopedDependency;

    [ClassInitialize]
    public static void ClassInitialize(TestContext _)
    {
        ScopedDependency = Hooks.serviceProvider!.GetRequiredService<IDependencyB>();
    }

    [TestMethodDI]
    public void TestDITransientDependencies(IDependencyC myDependencyOne, IDependencyC myDependencyTwo)
    {
        Assert.AreNotSame(myDependencyOne, myDependencyTwo);
    }

    [TestMethodDI]
    public void TestDISingletonDependencies(IDependencyA myDependencyOne, IDependencyA myDependencyTwo)
    {
        Assert.AreSame(myDependencyOne, myDependencyTwo);
    }

    [TestMethodDI]
    public void TestDIScopedDependencies(IDependencyB myDependencyOne, IDependencyB myDependencyTwo)
    {
        Assert.AreSame(myDependencyOne, myDependencyTwo);
    }
    [TestMethodDI]
    public void TestDIScopedDependenciesDifferentScope(IDependencyB myDependency)
    {
        Assert.AreNotSame(myDependency, ScopedDependency);
    }
}