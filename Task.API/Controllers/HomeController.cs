using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Task.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetContentAsync(CancellationToken cancellationToken) // Bu parametreyi geçtiğimiz andan itibaren artık sayfayı işlem tamamlanmadan kapatırsak hemen TaskCanceledException fırlatacaktır. 
        {   // Bu TaskCanceledException ı ınıda aşağıda try catch blogu üzerinden handle edelim.
            try
            {
                _logger.LogInformation("istek başladı.");
                Enumerable.Range(1, 10).ToList().ForEach(x => // Bu arkadaş 10 kere dönecek ve her 10 kere döndüğünde 1 sn ye uyuyacak.
                {
                    Thread.Sleep(1000);
                    cancellationToken.ThrowIfCancellationRequested();
                }); 
                _logger.LogInformation("istek bitti.");
                return Ok();
            }
            catch (System.Exception e)
            {
                _logger.LogInformation("İstek İptal edildi." + e.Message);
                return BadRequest(e.Message);
            }
        }
    }
}

