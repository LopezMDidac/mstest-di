using Microsoft.Extensions.DependencyInjection;

namespace mstest_di;

internal class TestMethodDI : TestMethodAttribute
{
    public override TestResult[] Execute(ITestMethod testMethod)
    {
        var nParameters = testMethod.ParameterTypes?.Length ?? 0;
        if (nParameters != 0)
        {
            object?[] injectedArgs = new object[nParameters];
            var serviceProvider = Hooks.serviceProvider;
            using (var scope = serviceProvider!.CreateScope())
            {

                for (var i = 0; i < nParameters; i++)
                {
                    injectedArgs[i] = scope.ServiceProvider.GetService(testMethod.ParameterTypes![i].ParameterType)!;
                }
            }
            return [testMethod.Invoke(injectedArgs!)];
        }
        else
        {
            return base.Execute(testMethod);
        }
    }
}

internal class TestMethodDIMultipleProviders: TestMethodAttribute
{
    public override TestResult[] Execute(ITestMethod testMethod)
    {
        var nParameters = testMethod.ParameterTypes?.Length ?? 0;
        if (nParameters != 0)
        {
            object?[] injectedArgs = new object[nParameters];
            var localServiceCollection = (IServiceCollection?) Type.GetType(testMethod.TestClassName)?
                .GetFields()?
                .First(x => x.FieldType == typeof(IServiceCollection))?
                .GetValue(null);
            var serviceProvider = localServiceCollection?.BuildServiceProvider();
            using (var localScope = serviceProvider?.CreateScope())
            using (var globalScope = Hooks.serviceProvider?.CreateScope())
            {

                for (var i = 0; i < nParameters; i++)
                {
                    injectedArgs[i] = localScope?.ServiceProvider.GetService(testMethod.ParameterTypes![i].ParameterType);
                    if (injectedArgs[i] == null)
                        injectedArgs[i] = globalScope?.ServiceProvider.GetService(testMethod.ParameterTypes![i].ParameterType)!;
                }
            }
            serviceProvider?.Dispose();
            return [testMethod.Invoke(injectedArgs!)];
        }
        else
        {
            return base.Execute(testMethod);
        }
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
            object?[] injectedArgs = new object[nParameters];
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
                        injectedArgs[i] = scope.ServiceProvider.GetService(testMethod.ParameterTypes[i].ParameterType);
                    }
                }
            }
            return [testMethod.Invoke(injectedArgs!)];
        }
        else
        {
            return base.Execute(testMethod);
        }
    }
}
