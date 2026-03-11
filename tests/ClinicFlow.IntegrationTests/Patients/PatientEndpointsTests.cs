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

    [Fact]
    public async Task UpdatePatient_WhenRequestIsValid_ShouldReturnOk()
    {
        var patientId = await CreatePatientAsync();

        var request = new
        {
            firstName = "Updated",
            lastName = "Patient",
            phoneNumber = "5550009988",
            email = "updated@test.com",
            notes = "updated via integration test",
            isActive = true
        };

        var response = await _client.PutAsJsonAsync($"/api/patients/{patientId}", request);
        var content = await response.Content.ReadAsStringAsync();

        _output.WriteLine($"Status Code: {(int)response.StatusCode} {response.StatusCode}");
        _output.WriteLine(content);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdatePatient_WhenPatientDoesNotExist_ShouldReturnNotFound()
    {
        var request = new
        {
            firstName = "Updated",
            lastName = "Patient",
            phoneNumber = "5550009988",
            email = "updated@test.com",
            notes = "updated via integration test",
            isActive = true
        };

        var response = await _client.PutAsJsonAsync($"/api/patients/{Guid.NewGuid()}", request);
        var content = await response.Content.ReadAsStringAsync();

        _output.WriteLine($"Status Code: {(int)response.StatusCode} {response.StatusCode}");
        _output.WriteLine(content);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeletePatient_WhenPatientExists_ShouldReturnSuccess()
    {
        var patientId = await CreatePatientAsync();

        var response = await _client.DeleteAsync($"/api/patients/{patientId}");
        var content = await response.Content.ReadAsStringAsync();

        _output.WriteLine($"Status Code: {(int)response.StatusCode} {response.StatusCode}");
        _output.WriteLine(content);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeletePatient_WhenPatientDoesNotExist_ShouldReturnNotFound()
    {
        var response = await _client.DeleteAsync($"/api/patients/{Guid.NewGuid()}");
        var content = await response.Content.ReadAsStringAsync();

        _output.WriteLine($"Status Code: {(int)response.StatusCode} {response.StatusCode}");
        _output.WriteLine(content);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<Guid> CreatePatientAsync()
    {
        var request = new
        {
            firstName = "Create",
            lastName = "BeforeUpdate",
            phoneNumber = "5551112233",
            email = "create.before.update@test.com",
            notes = "seed"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/patients", request);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var patient = await createResponse.Content.ReadFromJsonAsync<PatientResponse>();
        patient.Should().NotBeNull();

        return patient!.Id;
    }

    private sealed class PatientResponse
    {
        public Guid Id { get; set; }
    }
}
