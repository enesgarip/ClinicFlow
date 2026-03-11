using Xunit;
using Xunit.Abstractions;

namespace ClinicFlow.IntegrationTests.Patients;

public class PatientEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;

    public PatientEndpointsTests(CustomWebApplicationFactory factory, ITestOutputHelper output)
    {
        _client = factory.CreateTenantClient();
        _output = output;
    }

    [Fact]
    public async Task GetAllPatients_ShouldReturnSuccessStatusCode()
    {
        var response = await _client.GetAsync("/api/patients");
        var content = await response.Content.ReadAsStringAsync();

        _output.WriteLine($"Status Code: {(int)response.StatusCode} {response.StatusCode}");
        _output.WriteLine(content);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetPatientById_WhenPatientDoesNotExist_ShouldReturnNotFound()
    {
        var nonExistingId = Guid.NewGuid();

        var response = await _client.GetAsync($"/api/patients/{nonExistingId}");
        var content = await response.Content.ReadAsStringAsync();

        _output.WriteLine($"Status Code: {(int)response.StatusCode} {response.StatusCode}");
        _output.WriteLine(content);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    [Fact]
    public async Task CreatePatient_WhenRequestIsValid_ShouldReturnCreated()
    {
        var request = new
        {
            firstName = "Enes",
            lastName = "Garip",
            phoneNumber = "5551112233",
            email = "enes@test.com",
            notes = "integration test"
        };

        var response = await _client.PostAsJsonAsync("/api/patients", request);
        var content = await response.Content.ReadAsStringAsync();

        _output.WriteLine($"Status Code: {(int)response.StatusCode} {response.StatusCode}");
        _output.WriteLine(content);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
    [Fact]
    public async Task CreatePatient_WhenRequestIsInvalid_ShouldReturnBadRequest()
    {
        var request = new
        {
            firstName = "",
            lastName = "",
            phoneNumber = ""
        };

        var response = await _client.PostAsJsonAsync("/api/patients", request);
        var content = await response.Content.ReadAsStringAsync();

        _output.WriteLine($"Status Code: {(int)response.StatusCode} {response.StatusCode}");
        _output.WriteLine(content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}