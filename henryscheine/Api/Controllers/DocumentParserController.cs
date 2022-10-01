using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DocumentParserController : ControllerBase
    {
        private readonly IParserService<string> _parserService;

        private readonly ILogger<DocumentParserController> _logger;

        public DocumentParserController(
            IParserService<string> parserService,
            ILogger<DocumentParserController> logger
        )
        {
            _parserService = parserService;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<IEnumerable<string>> Get(string input)
        {
            try
            {
                return _parserService.ParseText(input);
            }
            catch
            {
                _logger.LogError("Error parsing a request", input);
                return null;
            }
        }
    }
}
