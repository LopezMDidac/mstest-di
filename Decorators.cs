using Microsoft.Extensions.DependencyInjection;

namespace mstest_with_DI;

internal class TestMethodDI : TestMethodAttribute
{
    public override TestResult[] Execute(ITestMethod testMethod)
    {
        object[] injectedArgs = new object[testMethod.ParameterTypes.Length];
        var serviceProvider = Hooks.serviceProvider;
        using (var scope = serviceProvider!.CreateScope())
        {
            
            for (var i = 0; i < testMethod.ParameterTypes.Length; i++)
            {
                injectedArgs[i] = scope.ServiceProvider.GetService(testMethod.ParameterTypes[i].ParameterType)!;
            }
        }
        return [testMethod.Invoke(injectedArgs)];
    }
}

internal class DataTestMethodDI : DataTestMethodAttribute
{
    public override TestResult[] Execute(ITestMethod testMethod)
    {
        var nArguments = testMethod.Arguments?.Length ?? 0;
        var nParameters = testMethod.ParameterTypes?.Length ?? 0;
        if (nArguments < nParameters)
        {
            object[] injectedArgs = new object[nParameters];
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
                        injectedArgs[i] = scope.ServiceProvider.GetService(testMethod.ParameterTypes[i].ParameterType)!;
                    }
                }
            }
            return [testMethod.Invoke(injectedArgs)];
        }
        else
            return base.Execute(testMethod);
    }
}
