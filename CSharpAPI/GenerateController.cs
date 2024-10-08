using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text; // Needed for Encoding.UTF8
using Microsoft.Extensions.Logging; // Needed for logging

[ApiController]
[Route("generate")] // Use the specific route "generate" for the controller
public class GenerateController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<GenerateController> _logger;

    public GenerateController(IHttpClientFactory httpClientFactory, ILogger<GenerateController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    [HttpPost]
public async Task<IActionResult> GenerateText([FromBody] PromptRequest request)
{
    try
    {
        if (string.IsNullOrWhiteSpace(request.Prompt))
        {
            return BadRequest(new { error = "Prompt cannot be empty" });
        }

        _logger.LogInformation("Received request to GenerateText with prompt: {Prompt}", request.Prompt);

        var client = _httpClientFactory.CreateClient();
        var pythonApiUrl = "http://localhost:8000/generate";

        var content = new StringContent(JsonSerializer.Serialize(new { prompt = request.Prompt }), Encoding.UTF8, "application/json");

        var response = await client.PostAsync(pythonApiUrl, content);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully received response from FastAPI: {Response}", result);
            return Ok(new { generated_text = result });
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("Error calling FastAPI. Status Code: {StatusCode}, Response: {ErrorResponse}", response.StatusCode, errorContent);
            
            return StatusCode((int)response.StatusCode, new { error = "Error calling FastAPI", details = errorContent });
        }   
    }
    catch (HttpRequestException ex)
    {
        _logger.LogError("HttpRequestException occurred while calling FastAPI: {Message}", ex.Message);
        return StatusCode(500, new { error = "Error connecting to FastAPI", details = ex.Message });
    }
    catch (Exception ex)
    {
        _logger.LogError("An unexpected error occurred: {Message}", ex.Message);
        return StatusCode(500, new { error = "An unexpected error occurred", details = ex.Message });
    }
}

}

public class PromptRequest
{
    public string Prompt { get; set; } = string.Empty;
}
