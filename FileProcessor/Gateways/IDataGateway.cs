using FileProcessor.Entities;

namespace FileProcessor.Gateways;

public interface IFileDataGateway
{
    Task<OperationDefinition> Load(string filename);
}