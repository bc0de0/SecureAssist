using System.Threading.Tasks;

namespace SecureAssist.Application.Interfaces;

public interface IAiResponseStorage
{
    Task StoreResponseAsync(string interactionId, string prompt, string response, string metadata);
}
