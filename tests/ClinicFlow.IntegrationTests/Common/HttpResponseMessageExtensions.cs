namespace ClinicFlow.IntegrationTests.Common;

public static class HttpResponseMessageExtensions
{
    public static async Task<string> ReadBodyAsync(this HttpResponseMessage response)
    {
        return await response.Content.ReadAsStringAsync();
    }
}