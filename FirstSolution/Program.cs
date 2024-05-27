using Integration.Service;

var service = new ItemIntegrationService();
       
ThreadPool.QueueUserWorkItem(_ => service.SaveItem("a"));
ThreadPool.QueueUserWorkItem(_ => service.SaveItem("b"));
ThreadPool.QueueUserWorkItem(_ => service.SaveItem("c"));

Thread.Sleep(500);

ThreadPool.QueueUserWorkItem(_ => service.SaveItem("a"));
ThreadPool.QueueUserWorkItem(_ => service.SaveItem("b"));
ThreadPool.QueueUserWorkItem(_ => service.SaveItem("c"));

Thread.Sleep(5000);

Console.WriteLine("Everything recorded:");

service.GetAllItems().ForEach(Console.WriteLine);


var tasks = new[]
{
    Task.Run(() => Console.WriteLine(service.SaveItem("a").Message)),
    Task.Run(() => Console.WriteLine(service.SaveItem("b").Message)),
    Task.Run(() => Console.WriteLine(service.SaveItem("c").Message)),
    Task.Run(() => Console.WriteLine(service.SaveItem("a").Message)),
    Task.Run(() => Console.WriteLine(service.SaveItem("b").Message)),
    Task.Run(() => Console.WriteLine(service.SaveItem("c").Message)),
};

await Task.WhenAll(tasks);

service.GetAllItems().ForEach(Console.WriteLine);