namespace FileProcessor.Entities;

public record OperationDefinition(IList<IList<double>> DataSets, IList<Generator> Generators);
