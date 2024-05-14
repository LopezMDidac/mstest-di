using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mstest_with_DI.InjectableDependencies;

public interface IDependencyA
{
    string Name { get; }
    Guid GetId();

    void DoStuff();


}

public class SingletonDependencyA : IDependencyA
{
    private readonly Guid _id;

    public SingletonDependencyA() 
    { 
        _id = Guid.NewGuid();
        
    }

    public string Name => "DependencyA";

    public void DoStuff()
    {
        throw new NotImplementedException();
    }

    public Guid GetId()
    {
        return _id;
    }
}
