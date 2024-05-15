using Microsoft.Extensions.DependencyInjection;
using mstest_di.InjectableDependencies;
using System;

namespace mstest_di.Tests;




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

    [TestMethodDIMultipleProviders]
    public void TestDISingleDependencyB(IDependencyB myDependency)
    {
        Assert.AreEqual("DependencyB", myDependency.Name);
    }

    [TestMethodDIMultipleProviders]
    public void TestDISingleDependencyC(IDependencyC myDependency)
    {
        Assert.AreEqual("DependencyC", myDependency.Name);
    }

    [TestMethodDIMultipleProviders]
    public void TestUnsortedDependencies(IDependencyC myDependencyC, IDependencyA myDependencyA)
    {
        Assert.AreEqual("DependencyC", myDependencyC.Name);
        Assert.AreEqual("DependencyAA", myDependencyA.Name);
    }

    [TestMethodDIMultipleProviders]
    public void TestAllDependencies(IDependencyA myDependencyA, IDependencyB myDependencyB, IDependencyC myDependencyC)
    {
        Assert.AreEqual("DependencyC", myDependencyC.Name);
        Assert.AreEqual("DependencyAA", myDependencyA.Name);
        Assert.AreEqual("DependencyB", myDependencyB.Name);
    }

    [TestMethodDIMultipleProviders]
    public void TestNonRegisteredDependency(IDependencyD myDependency)
    {
        Assert.IsNull(myDependency);
    }

    [TestMethodDIMultipleProviders]
    public void TestNoDI()
    {
        Assert.IsTrue(true);
    }
    [TestMethodDIMultipleProviders]
    public void TestByType(TransientDependencyC mydependency)
    {
        Assert.IsNotNull(mydependency);
    }
}