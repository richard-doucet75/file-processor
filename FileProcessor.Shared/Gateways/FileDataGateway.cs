using FileProcessor.Entities;
using FileProcessor.Gateways;
using Newtonsoft.Json;

namespace FileProcessor.Shared.Gateways;

public class FileDataGateway : IFileDataGateway
{
    public async Task<OperationDefinition> Load(string filename)
    {
        var content = await File.ReadAllTextAsync(filename);
        var result = JsonConvert.DeserializeObject<OperationDefinition>(content);
        
        if (result is null)
            throw new Exception();

        return result;
    }
}