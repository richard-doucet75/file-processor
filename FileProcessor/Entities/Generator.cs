using FileProcessor.UseCase;
using FileProcessor.UseCase.PresentationModels;

namespace FileProcessor.Entities;

public record Generator(string Name, int Interval, OperationBase Operation)
{
    public async Task Start(ProcessDataSet.IPresenter dataPresenter, IList<IList<double>> datasets)
    {
        foreach (var dataset in datasets)
        {
            dataPresenter.Present(new PresentableResult(
                DateTime.Now,
                Name,
                Operation.Execute(dataset)));
            
            await Task.Delay(Interval * 1000).ConfigureAwait(false);
        }
    }
}