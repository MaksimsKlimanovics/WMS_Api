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
    public class WarehouseController : Controller
    {
        private readonly IQueryPipe SqlPipe;
        private readonly ICommand SqlCommand;

        public WarehouseController(ICommand sqlCommand, IQueryPipe sqlPipe)
        {
            this.SqlCommand = sqlCommand;
            this.SqlPipe = sqlPipe;
        }

        // GET api/warehouse
        [HttpGet]
        public async Task Get()
        {
            await SqlPipe.Stream("SELECT [code]" +
                                    ",[name]" +
                                    ",[default_bin] " +
                                 "FROM [dbo].[n_warehouse] FOR JSON PATH"
                , Response.Body, "[]");
        }

        // GET api/warehouse/test
        [HttpGet("{code}")]
        public async Task Get(string code)
        {
            var cmd = new SqlCommand(@"SELECT [code]
                                              ,[name]
                                              ,[default_bin]
                                       FROM [dbo].[n_warehouse] where [Code] = @code FOR JSON PATH, WITHOUT_ARRAY_WRAPPER");
            cmd.Parameters.AddWithValue("code", code.ToUpper());
            await SqlPipe.Stream(cmd, Response.Body, "{}");
        }

        // POST api/warehouse
        [HttpPost]
        public async Task Post()
        {
            string req = new StreamReader(Request.Body).ReadToEnd();
            var cmd = new SqlCommand(
                                        @"insert into [dbo].[n_warehouse]
                                        select *
                                        from OPENJSON(@warehouse)
                                        WITH([code] [nvarchar](20)
                                             ,[name] [nvarchar](50)
                                             ,[default_bin] [nvarchar](20)
                                            )"
                                    );
            cmd.Parameters.AddWithValue("code", req);
            await SqlCommand.ExecuteNonQuery(cmd);
        }

        // DELETE api/warehouse/test
        [HttpDelete("{code}")]
        public async Task Delete(string code)
        {
            var cmd = new SqlCommand(@"delete from n_warehouse where code = @code");
            cmd.Parameters.AddWithValue("code", code.ToUpper());
            await SqlCommand.ExecuteNonQuery(cmd);
        }
    }
}
