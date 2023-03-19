using FileProcessor.Entities;
using FileProcessor.Gateways;
using FileProcessor.UseCase.PresentationModels;

namespace FileProcessor.UseCase;

public class GetOperationDefinition
{
    public interface IPresenter
    {
        void Present(IList<double> dataSet);
        void Present(GeneratorPresentation generator);
    }

    
    private readonly IFileDataGateway _fileDataGateway;

    public GetOperationDefinition(IFileDataGateway fileDataGateway)
    {
        _fileDataGateway = fileDataGateway;
    }
    
    public async Task Execute(IPresenter presenter, string filename)
    {
        var operationDefinition = await _fileDataGateway.Load(filename);
        Present(presenter, operationDefinition.DataSets);
        Present(presenter, operationDefinition);
    }
    
    private static void Present(IPresenter presenter, IEnumerable<IList<double>> dataSets)
    {
        foreach (var dataset in dataSets)
        {
            presenter.Present(dataset);
        }
    }
    private static void Present(IPresenter presenter, OperationDefinition operationDefinition)
    {
        foreach (var generator in operationDefinition.Generators)
        {
            presenter.Present(
                new GeneratorPresentation(
                    generator.Name, 
                    generator.Interval,
                    generator.Operation.OperationName)
                );
        }
    }
}