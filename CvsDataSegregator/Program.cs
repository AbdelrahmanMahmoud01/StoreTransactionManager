using GoodsDataSegregator;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Application application = new Application();
        await application.Run();
    }
}