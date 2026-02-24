namespace SecureAssist.Application.Interfaces;

public interface IAiService
{
    Task<string> ProcessPromptAsync(string prompt);
}
