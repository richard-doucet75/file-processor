using FileProcessor.Entities;
using FileProcessor.UseCase.PresentationModels;

namespace FileProcessor.UseCase;

public class ProcessDataSet
{
    public interface IPresenter
    {
        void Present(PresentableResult result);
    }
    
    public async Task Execute(IPresenter dataPresenter, IList<GeneratorPresentation> generators, IList<IList<double>> dataSets)
    {
        await Task.Run(() =>
        {
            var gens = Map(generators);
            Task.WaitAll(
                gens.Select(
                    generator => generator.Start(dataPresenter, dataSets)
                ).ToArray()
            );
        });
    }

    private static IEnumerable<Generator> Map(IList<GeneratorPresentation> generators)
    {
        return generators.Select(c => new Generator(c.Name, c.Interval, c.Operation));
    }
}

