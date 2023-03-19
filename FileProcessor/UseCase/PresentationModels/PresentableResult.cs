namespace FileProcessor.UseCase.PresentationModels;

public record PresentableResult(DateTime Timestamp, string OperationName, double Value);