using FileProcessorConsole.Presenters;

namespace FileProcessorConsole;

public class Program
{
    static async Task Main(string[] args)
    {
        var presenter = new DataPresenter();
        await presenter.LoadOperations(args[0]);
        await presenter.ExecuteOperations();
    }
}