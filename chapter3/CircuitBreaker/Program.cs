using Polly;
using Polly.CircuitBreaker;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CircuitBreaker;

class Program
{
    static readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> circuitBreakerPolicy =
        Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
              .CircuitBreakerAsync(3, TimeSpan.FromSeconds(30),
                onBreak: (ex, ts) => Console.WriteLine($"Circuit broken for {ts.TotalSeconds} seconds."),
                onReset: () => Console.WriteLine("Circuit reset. Back to normal operation."),
                onHalfOpen: () => Console.WriteLine("Circuit is half-open. Testing the next call.")
              );
    static async Task Main(string[] args)
    {
        HttpClient httpClient = new HttpClient();

        for (int i = 0; i < 10; i++)
        {
            try
            {
                HttpResponseMessage response = await circuitBreakerPolicy.ExecuteAsync(() => 
                    httpClient.GetAsync("https://example.com/api/resource"));

                Console.WriteLine(response.IsSuccessStatusCode ? "Success" : "Failure");
            }
            catch (BrokenCircuitException)
            {
                Console.WriteLine("Circuit is open. Request not attempted.");
            }
            await Task.Delay(1000);
        }
    }
}
