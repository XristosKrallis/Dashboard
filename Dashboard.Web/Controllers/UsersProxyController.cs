using Dashboard.Core.DTOs;
using Dashboard.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;

[Route("[controller]/[action]")]
[ApiController]
public class UsersProxyController : Controller
{
    private readonly HttpClient _http;
    private readonly IJwtService _jwtService;

    public UsersProxyController(IHttpClientFactory httpFactory, IJwtService jwtService)
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
    public async Task<IActionResult> LoadUsers()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7170/api/Users");
        AttachJwtHeader(request);

        var response = await _http.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorObj = await response.Content.ReadFromJsonAsync<object>();
            return StatusCode((int)response.StatusCode, errorObj);
        }

        var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
        return Json(users);
    }

    [HttpPost]
    public async Task<IActionResult> InsertUser([FromForm] IFormCollection form)
    {
        var valuesJson = form["values"];
        var dto = System.Text.Json.JsonSerializer.Deserialize<UserDto>(
            valuesJson,
            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        )!;

        var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7170/api/Users")
        {
            Content = JsonContent.Create(dto)
        };
        AttachJwtHeader(request);

        var response = await _http.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorObj = await response.Content.ReadFromJsonAsync<object>();
            return StatusCode((int)response.StatusCode, errorObj);
        }

        var user = await response.Content.ReadFromJsonAsync<UserDto>();
        return Ok(user);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUser([FromForm] int key, [FromForm] IFormCollection form)
    {
        var valuesJson = form["values"];
        var dto = System.Text.Json.JsonSerializer.Deserialize<UserDto>(
            valuesJson,
            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        )!;

        var request = new HttpRequestMessage(HttpMethod.Put, $"https://localhost:7170/api/Users/{key}")
        {
            Content = JsonContent.Create(dto)
        };
        AttachJwtHeader(request);

        var response = await _http.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorObj = await response.Content.ReadFromJsonAsync<object>();
            return StatusCode((int)response.StatusCode, errorObj);
        }

        var user = await response.Content.ReadFromJsonAsync<UserDto>();
        return Ok(user);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteUser([FromForm] int key)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"https://localhost:7170/api/Users/{key}");
        AttachJwtHeader(request);

        var response = await _http.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorObj = await response.Content.ReadFromJsonAsync<object>();
            return StatusCode((int)response.StatusCode, errorObj);
        }

        return Ok(new { success = true });
    }
}
