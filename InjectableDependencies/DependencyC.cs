using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace mstest_with_DI.InjectableDependencies;

public interface IDependencyC
{
    string Name { get; }
    Guid GetId();

    void DoStuff();


}

public class TransientDependencyC : IDependencyC
{
    private readonly Guid _id;

    public TransientDependencyC()
    {
        _id = Guid.NewGuid();

    }

    public string Name => "DependencyC";

    public void DoStuff()
    {
        throw new NotImplementedException();
    }

    public Guid GetId()
    {
        return _id;
    }

}
