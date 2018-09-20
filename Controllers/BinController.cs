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
        // Just checking comment line.
        [HttpGet("{warehouse}/{bin}")]
        public async Task Get(string warehouse, string bin)
        {
            
            var cmd = new SqlCommand(@"select [warehouse_code]
                                             ,[code]
                                             ,[description]
                                             ,[barcode] from [dbo].[n_bin] where [warehouse_code] = @whouse and [code] = @bin FOR JSON PATH, WITHOUT_ARRAY_WRAPPER");
            cmd.Parameters.AddWithValue("whouse", warehouse.ToUpper());
            cmd.Parameters.AddWithValue("bin", bin.ToUpper());
            await SqlPipe.Stream(cmd, Response.Body, "{}");
        }

        // GET api/bin/exists
        [HttpGet("exists/{warehouse}/{bin}")]
        public async Task Exists(string warehouse, string bin)
        {
            var cmd = new SqlCommand(@"select (case when exists 
                                        (SELECT * from [dbo].[n_bin] where [warehouse_code] = @whouse and [code] = @bin) 
	                                   then 'true' else 'false' end) as [status] for json path, WITHOUT_ARRAY_WRAPPER");
            cmd.Parameters.AddWithValue("whouse", warehouse.ToUpper());
            cmd.Parameters.AddWithValue("bin", bin.ToUpper());
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

        // DELETE api/bin/deletee
        [HttpDelete("{warehouse}/{bin}")]
        public async Task Delete(string warehouse, string bin)
        {
            var cmd = new SqlCommand(@"delete from [dbo].[n_bin] where [warehouse_code] = @warehouse and [code] = @bin");
            cmd.Parameters.AddWithValue("warehouse", warehouse.ToUpper());
            cmd.Parameters.AddWithValue("bin", bin.ToUpper());
            await SqlCommand.ExecuteNonQuery(cmd);
        }

       

    }
}
