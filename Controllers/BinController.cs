using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Belgrade.SqlClient;
using System.Data.SqlClient;
using System.IO;

namespace WMS_Api.Controllers
{
    [Route("api/[controller]")]
    public class WhsBinController : Controller
    {
        private readonly IQueryPipe SqlPipe;
        private readonly ICommand SqlCommand;

        public WhsBinController(ICommand sqlCommand, IQueryPipe sqlPipe)
        {
            this.SqlCommand = sqlCommand;
            this.SqlPipe = sqlPipe;
        }

        // GET api/bin
        [HttpGet]
        public async Task Get()
        {
            await SqlPipe.Stream("select [warehouse_code]" +
                                    ",[code]" +
                                    ",[description]" +
                                    ",[barcode] from [dbo].[n_bin] FOR JSON PATH"
           , Response.Body, "[]");
        }

        // GET api/bin/test
        [HttpGet("{barcode}")]
        public async Task Get(string barcode)
        {
            var cmd = new SqlCommand(@"select [warehouse_code]
                                             ,[code]
                                             ,[description]
                                             ,[barcode] from [dbo].[n_bin] where [barcode] = @barcode FOR JSON PATH, WITHOUT_ARRAY_WRAPPER");
            cmd.Parameters.AddWithValue("barcode", barcode.ToUpper());
            await SqlPipe.Stream(cmd, Response.Body, "{}");
        }

        // POST api/bin
        [HttpPost]
        public async Task Post()
        {
            string req = new StreamReader(Request.Body).ReadToEnd();
            var cmd = new SqlCommand(
                                        @"insert into [dbo].[n_bin]
                                        select *
                                        from OPENJSON(@bin)
                                        WITH([warehouse_code] [nvarchar](20)
                                             ,[code] [nvarchar](20)
                                             ,[description] [nvarchar](50)
                                             ,[barcode] [nvarchar](50)
                                            )"
                                    );
            cmd.Parameters.AddWithValue("bin", req);
            await SqlCommand.ExecuteNonQuery(cmd);
        }

        // DELETE api/bin/test
        [HttpDelete("{barcode}")]
        public async Task Delete(string barcode)
        {
            var cmd = new SqlCommand(@"delete from n_bin where barcode = @barcode");
            cmd.Parameters.AddWithValue("barcode", barcode.ToUpper());
            await SqlCommand.ExecuteNonQuery(cmd);
        }
    }
}
