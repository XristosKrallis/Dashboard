using Dashboard.Core.DTOs;
using Dashboard.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;

[Route("[controller]/[action]")]
[ApiController]
public class WorkHoursProxyController : Controller
{
    private readonly HttpClient _http;
    private readonly IJwtService _jwtService;

    public WorkHoursProxyController(IHttpClientFactory httpFactory, IJwtService jwtService)
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
    public async Task<IActionResult> LoadWorkHours()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7170/api/WorkHours");
        AttachJwtHeader(request);

        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var workHours = await response.Content.ReadFromJsonAsync<List<WorkHoursDto>>();
        return Json(workHours);
    }

    [HttpPost]
    public async Task<IActionResult> InsertWorkHours([FromForm] IFormCollection form)
    {
        var valuesJson = form["values"];

        var dto = System.Text.Json.JsonSerializer.Deserialize<WorkHoursDto>(
            valuesJson,
            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        )!;

        using var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7170/api/WorkHours")
        {
            Content = JsonContent.Create(dto)
        };
        AttachJwtHeader(request);

        using var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var workHours = await response.Content.ReadFromJsonAsync<WorkHoursDto>();
        return Json(workHours);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateWorkHours([FromForm] int key, [FromForm] IFormCollection form)
    {
        var valuesJson = form["values"];

        var dto = System.Text.Json.JsonSerializer.Deserialize<WorkHoursDto>(
            valuesJson,
            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        )!;

        var request = new HttpRequestMessage(HttpMethod.Put, $"https://localhost:7170/api/WorkHours/{key}")
        {
            Content = JsonContent.Create(dto)
        };

        AttachJwtHeader(request);

        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var workHours = await response.Content.ReadFromJsonAsync<WorkHoursDto>();
        return Json(workHours);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteWorkHours([FromForm] int key)
    {
        Console.WriteLine($"ID:{key}");
        var request = new HttpRequestMessage(HttpMethod.Delete, $"https://localhost:7170/api/WorkHours/{key}");
        AttachJwtHeader(request);

        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return Json(new { success = true });
    }
}

