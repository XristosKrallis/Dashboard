using Dashboard.Core.DTOs;
using Dashboard.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;

[Route("[controller]/[action]")]
public class AppointmentProxyController : Controller
{
    private readonly HttpClient _http;
    private readonly IJwtService _jwtService;

    public AppointmentProxyController(IHttpClientFactory httpFactory, IJwtService jwtService)
    {
        _http = httpFactory.CreateClient();
        _jwtService = jwtService;
    }

    private UserIdentity GetUserIdentityFromClaims()
    {
        return new UserIdentity
        {
            Id = User.FindFirstValue(ClaimTypes.NameIdentifier)!,
            Email = User.FindFirstValue(ClaimTypes.Email)!,
            Roles = User.Claims
                        .Where(c => c.Type == ClaimTypes.Role)
                        .Select(c => c.Value)
                        .ToList()
        };
    }

    private void AttachJwtHeader(HttpRequestMessage request)
    {
        var identity = GetUserIdentityFromClaims();
        var jwt = _jwtService.CreateToken(identity);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
    }

    [HttpGet]
    public async Task<IActionResult> LoadAppointments()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7170/api/Appointments");
        AttachJwtHeader(request);

        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var appointments = await response.Content.ReadFromJsonAsync<List<SchedulerAppointmentDto>>();
        return Json(appointments);
    }

    [HttpPost]
    public async Task<IActionResult> InsertAppointment([FromForm] IFormCollection form)
    {
        var valuesJson = form["values"];

        var dto = System.Text.Json.JsonSerializer.Deserialize<SchedulerAppointmentDto>(
            valuesJson,
            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        )!;

        var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7170/api/Appointments")
        {
            Content = JsonContent.Create(dto)
        };

        AttachJwtHeader(request);

        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var appointment = await response.Content.ReadFromJsonAsync<SchedulerAppointmentDto>();
        return Json(appointment);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateAppointment([FromForm] int key, [FromForm] IFormCollection form)
    {
        var valuesJson = form["values"];

        var dto = System.Text.Json.JsonSerializer.Deserialize<SchedulerAppointmentDto>(
            valuesJson,
            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        )!;

        var request = new HttpRequestMessage(HttpMethod.Put, $"https://localhost:7170/api/Appointments/{key}")
        {
            Content = JsonContent.Create(dto)
        };

        AttachJwtHeader(request);

        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var appointment = await response.Content.ReadFromJsonAsync<SchedulerAppointmentDto>();
        return Json(appointment);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAppointment([FromForm] int key)
    {
        var request = new HttpRequestMessage(
            HttpMethod.Delete,
            $"https://localhost:7170/api/Appointments/{key}"
        );

        AttachJwtHeader(request);

        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return Json(new { success = true });
    }
}
