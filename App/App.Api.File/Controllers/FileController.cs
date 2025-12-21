using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.File.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        [Authorize(Roles = "seller,admin")]
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Dosya gönderilmedi");
            }

            var uploadFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot", "uploads"
            );

            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            var savedFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadFolder, savedFileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return Created("", new { fileName = savedFileName });
        }


        [HttpGet("download")]
        public IActionResult Download([FromQuery] string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return BadRequest();
            }

            var uploadPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot", "uploads"
            );

            var filePath = Path.Combine(uploadPath, fileName);

            if (!System.IO.File.Exists(filePath))   
            {
                return NotFound();
            }

            var contentType = "application/octet-stream";

            return PhysicalFile(filePath, contentType, fileName);
        }
    }
}
