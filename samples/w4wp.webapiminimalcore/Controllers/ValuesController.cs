using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebApiNetCore.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            try
            {
                if (id != 1)
                {
                    StringBuilder strb = new StringBuilder();
                    foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
                    {
                        strb.AppendLine(module.FileName);
                    }
                    return strb.ToString();
                }
                else
                {
                    var indexHtml = Path.Combine(Environment.ExpandEnvironmentVariables(@"%ProgramFiles(x86)%\w4wp.webapinetcore"), "index.html");
                    return System.IO.File.ReadAllText(indexHtml);
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
