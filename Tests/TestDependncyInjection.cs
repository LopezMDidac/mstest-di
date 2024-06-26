using Microsoft.Extensions.DependencyInjection;
using mstest_di.InjectableDependencies;
using System;

namespace mstest_di.Tests;




[TestClass]
public class TestDependencyInjection
{
    [TestMethodDI]
    public void TestDISingleDependencyA(IDependencyA myDependency)
    {
        Assert.AreEqual("DependencyA", myDependency.Name);
    }

    [TestMethodDI]
    public void TestDISingleDependencyB(IDependencyB myDependency)
    {
        Assert.AreEqual("DependencyB", myDependency.Name);
    }

    [TestMethodDI]
    public void TestDISingleDependencyC(IDependencyC myDependency)
    {
        Assert.AreEqual("DependencyC", myDependency.Name);
    }

    [TestMethodDI]
    public void TestUnsortedDependencies(IDependencyC myDependencyC, IDependencyA myDependencyA)
    {
        Assert.AreEqual("DependencyC", myDependencyC.Name);
        Assert.AreEqual("DependencyA", myDependencyA.Name);
    }

    [TestMethodDI]
    public void TestAllDependencies(IDependencyA myDependencyA, IDependencyB myDependencyB, IDependencyC myDependencyC)
    {
        Assert.AreEqual("DependencyC", myDependencyC.Name);
        Assert.AreEqual("DependencyA", myDependencyA.Name);
        Assert.AreEqual("DependencyB", myDependencyB.Name);
    }

    [TestMethodDI]
    public void TestNonRegisteredDependency(IDependencyD myDependency)
    {
        Assert.IsNull(myDependency);
    }

    [TestMethodDI]
    public void TestNoDI()
    {
        Assert.IsTrue(true);
    }
    [TestMethodDI]
    public void TestByType(TransientDependencyC mydependency)
    {
        Assert.IsNull(mydependency);
    }
}