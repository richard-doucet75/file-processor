using FileProcessor.Shared.Gateways;
using FileProcessor.UseCase;
using FileProcessor.UseCase.PresentationModels;

namespace FileProcessorConsole.Presenters;

public class DataPresenter : 
    GetOperationDefinition.IPresenter,
    ProcessDataSet.IPresenter
{
    private GetOperationDefinition GetOperationDefinitionCommand { get; }
    private ProcessDataSet ProcessCommand { get; }
    
    private readonly IList<IList<double>> _datasets = new List<IList<double>>();
    private readonly IList<GeneratorPresentation> _generators = new List<GeneratorPresentation>();
    
    public DataPresenter()
    {
        GetOperationDefinitionCommand = new GetOperationDefinition(new FileDataGateway());
        ProcessCommand = new ProcessDataSet();
    }
    
    public void Present(PresentableResult result)
    {
        Console.WriteLine($"{result.Timestamp:HH:mm:ss} {result.OperationName} {result.Value::0.##}");
    }

    public void Present(IList<double> dataSet)
    {
        _datasets.Add(dataSet);
    }

    public void Present(GeneratorPresentation generator)
    {
        _generators.Add(generator);
    }

    public async Task LoadOperations(string filename)
    {
        await GetOperationDefinitionCommand.Execute(this, filename);
    }

    public async Task ExecuteOperations()
    {
        await ProcessCommand.Execute(this, _generators, _datasets);
    }
}