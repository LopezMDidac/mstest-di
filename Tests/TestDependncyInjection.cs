using mstest_with_DI.InjectableDependencies;

namespace mstest_with_DI.Tests;




[TestClass]
public class TestDependncyInjection
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
}