using Unity;
using Unity.Interception;
using Unity.Interception.ContainerIntegration;
using Unity.Interception.InterceptionBehaviors;
using Unity.Interception.Interceptors.TypeInterceptors.VirtualMethodInterception;
using Unity.Interception.PolicyInjection.Pipeline;

// コンテナを生成
var container = new UnityContainer();
// Interceptorを有効化
container.AddNewExtension<Interception>();
// コンテナに登録する際に、オプションを指定
container.RegisterType<IGreetingService, GreetingService>(
    new Interceptor<VirtualMethodInterceptor>(),
    new InterceptionBehavior<LoggingBehavior>());

// ここからテスト
// コンテナから取り出す
var service = container.Resolve<IGreetingService>();
// メソッドの呼び出し
service.DoSomething("戻り値ない");

public interface IGreetingService
{
    void DoSomething(string param1);
}

public class GreetingService : IGreetingService
{
    public virtual void DoSomething(string param1)
    {
        Console.WriteLine(param1);
    }
}

public class LoggingBehavior : IInterceptionBehavior
{
    public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
    {
        // 元のメソッドを呼び出す前の処理
        WriteLog(String.Format("Invoking Method {0} at {1}", input.MethodBase, DateTime.Now.ToString()));

        // 元のメソッドを呼び出す
        var result = getNext()(input, getNext);

        // 元のメソッドを呼び出した後の処理
        if (result.Exception != null)
        {
            WriteLog(String.Format("Method {0} threw exception {1} at {2}", input.MethodBase, result.Exception.Message, DateTime.Now.ToString()));
        }
        else
        {
            WriteLog(String.Format("Method {0} returned {1} at {2}", input.MethodBase, result.ReturnValue, DateTime.Now.ToString()));
        }

        return result;
    }

    public IEnumerable<Type> GetRequiredInterfaces() => Type.EmptyTypes;

    public bool WillExecute => true;

    private void WriteLog(string message)
    {
        // 実際はNLogなど利用する
        Console.WriteLine("LOG:" + message);
    }
}
