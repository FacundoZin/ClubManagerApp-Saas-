using Quartz;

namespace APIClub.Infrastructure.JobsProgramados
{
    public class HelloWorldJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("Hello world");

            return Task.CompletedTask;
        }
    }
}
