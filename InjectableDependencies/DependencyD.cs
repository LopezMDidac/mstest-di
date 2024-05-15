using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace mstest_di.InjectableDependencies;

public interface IDependencyD
{
    string Name { get; }
    Guid GetId();

    void DoStuff();


}