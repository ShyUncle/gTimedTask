using System;
using Grpc.Net.Client;
using gTimedTask.Core;
namespace gTimedTask.ClientSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:10101");
            var client = new Greeter.GreeterClient(channel);
            while (true)
            {
                var apply = client.SayHello(new HelloRequest() { Name = "43" });
                Console.WriteLine(apply.Message);
                Console.ReadLine();
            }
        }
    }
}
