using mstest_with_DI.InjectableDependencies;

namespace mstest_with_DI.Tests;




[TestClass]
public class DataTestDependncyInjection
{

    [DataTestMethodDI]
    public void DataTestDIOnlyInjection(IDependencyA myDependency)
    {
        Assert.AreEqual("DependencyA", myDependency.Name);
    }

    [DataTestMethodDI]
    [DataRow(1, "2", 3.4)]
    [DataRow(2, "3", 4.6)]
    public void DataTestDIOnlyDataRows(int a, string b, double c)
    {
        Assert.IsInstanceOfType(a, typeof(int));
        Assert.IsInstanceOfType(b, typeof(string));
        Assert.IsInstanceOfType(c, typeof(double));
    }

    [DataTestMethodDI]
    [DataRow(1, "2", 3.4)]
    [DataRow(2, "3", 4.6)]
    public void DataTestDIWithRows(IDependencyC myDependency, int a, string b, double c)
    {
        Assert.AreEqual("DependencyC", myDependency.Name);
        Assert.IsInstanceOfType(a, typeof(int));
        Assert.IsInstanceOfType(b, typeof(string));
        Assert.IsInstanceOfType(c, typeof(double));
    }

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

    [DataTestMethodDI]
    public void TestNoDI()
    {
        Assert.IsTrue(true);
    }
}