using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace mstest_with_DI.InjectableDependencies;

public interface IDependencyB
{
    string Name { get; }
    Guid GetId();

    void DoStuff();


}

public class ScopedDependencyB : IDependencyB
{
    private readonly Guid _id;

    public ScopedDependencyB()
    {
        _id = Guid.NewGuid();

    }

    public string Name => "DependencyB";

    public void DoStuff()
    {
        throw new NotImplementedException();
    }

    public Guid GetId()
    {
        return _id;
    }

}
