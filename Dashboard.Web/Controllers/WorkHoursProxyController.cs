using Dashboard.Core.DTOs;
using Dashboard.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;

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

        if (!response.IsSuccessStatusCode)
        {
            var errorObj = await response.Content.ReadFromJsonAsync<object>();
            return StatusCode((int)response.StatusCode, errorObj);
        }

        var workHours = await response.Content.ReadFromJsonAsync<List<WorkHoursDto>>();
        return Json(workHours);
    }

    [HttpPost]
    public async Task<IActionResult> InsertWorkHours([FromBody] WorkHoursDto dto)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7170/api/WorkHours")
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

        var workHours = await response.Content.ReadFromJsonAsync<WorkHoursDto>();
        return Ok(workHours);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateWorkHours([FromBody] WorkHoursDto dto)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"https://localhost:7170/api/WorkHours/{dto.Id}")
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

        var workHours = await response.Content.ReadFromJsonAsync<WorkHoursDto>();
        return Ok(workHours);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteWorkHours([FromForm] int key)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"https://localhost:7170/api/WorkHours/{key}");
        AttachJwtHeader(request);

        var response = await _http.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorObj = await response.Content.ReadFromJsonAsync<object>();
            return StatusCode((int)response.StatusCode, errorObj);
        }

        return Ok(new { success = true });
    }

    [HttpGet]
    public async Task<IActionResult> GetByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var requestUrl = $"https://localhost:7170/api/WorkHours?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
        var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
        AttachJwtHeader(request);

        var response = await _http.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorObj = await response.Content.ReadFromJsonAsync<object>();
            return StatusCode((int)response.StatusCode, errorObj);
        }

        var workHours = await response.Content.ReadFromJsonAsync<List<WorkHoursDto>>();

        var dates = Enumerable.Range(0, (endDate - startDate).Days + 1)
                              .Select(i => startDate.AddDays(i))
                              .ToList();

        var result = dates.Select(date =>
        {
            var entry = workHours.FirstOrDefault(w => w.WorkDate.Date == date.Date);

            return new
            {
                WorkDate = date.ToString("yyyy-MM-dd"),
                RegularWork = entry?.RegularWork ?? 0,
                Overtime = entry?.Overtime ?? 0,
                TimeOff = entry?.TimeOff ?? 0
            };
        }).ToList();

        return Json(result);
    }
}
