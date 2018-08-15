using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Belgrade.SqlClient;
using System.Data.SqlClient;
using System.IO;

namespace WMS_Api.Controllers
{
    [Route("api/[controller]")]
    public class DocHdrController : Controller
    {
        private readonly IQueryPipe SqlPipe;
        private readonly ICommand SqlCommand;

        public DocHdrController(ICommand sqlCommand, IQueryPipe sqlPipe)
        {
            this.SqlCommand = sqlCommand;
            this.SqlPipe = sqlPipe;
        }

        // GET api/DocHdr
        [HttpGet]
        public async Task Get()
        {
            await SqlPipe.Stream("SELECT [document_type]" +
                ",[document_no]" +
                ",[document_date]" +
                ",[worker_code]" +
                ",[warehouse_code]" +
                ",[partner_type]" +
                ",[partner_code]" +
                ",[partner_name]" +
                ",[uid] " +
                ",[status] " +
                "FROM [dbo].[n_warehouse_document_header] FOR JSON PATH"
                , Response.Body, "[]");
        }

        // GET api/worker/TEST
        [HttpGet("{uid}")]
        public async Task Get(string uid)
        {
            var cmd = new SqlCommand(@"SELECT [document_type]
                                              ,[document_no]
                                              ,[document_date]
                                              ,[worker_code]
                                              ,[warehouse_code]
                                              ,[partner_type]
                                              ,[partner_code]
                                              ,[partner_name]
                                              ,[uid]
                                              ,[status]
                                       FROM [dbo].[n_warehouse_document_header] where uid = @uid FOR JSON PATH, WITHOUT_ARRAY_WRAPPER");
            cmd.Parameters.AddWithValue("uid", uid.ToUpper());
            await SqlPipe.Stream(cmd, Response.Body, "{}");
        }

        // GET api/dochdr/exists
        [HttpGet("exists/{uid}")]
        public async Task Exists(string uid)
        {
            var cmd = new SqlCommand(@"select (case when exists 
                                        (SELECT * from [dbo].[n_warehouse_document_header] where [uid] =  @uid) 
	then 'true' else 'false' end) as [status] for json path, WITHOUT_ARRAY_WRAPPER");
            cmd.Parameters.AddWithValue("uid", uid.ToUpper());
            await SqlPipe.Stream(cmd, Response.Body, "{}");
        }


        // POST api/worker //[document_date] [datetime],
        [HttpPost]
        public async Task Post()
        {
            string req = new StreamReader(Request.Body).ReadToEnd();
            var cmd = new SqlCommand(
                                        @"insert into [dbo].[n_warehouse_document_header]
                                        select *
                                        from OPENJSON(@docHdr)
                                        WITH(
                                                [document_type] [int],
                                                [document_no] [nvarchar](20),
                                                [document_date] [datetime],
                                                [worker_code] [nvarchar](20),
                                                [warehouse_code] [nvarchar](20),
                                                [partner_type] [nvarchar](20),
                                                [partner_code] [nvarchar](20),
                                                [partner_name] [nvarchar](100),
                                                [uid] [nvarchar](100),
                                                [status] [int]
                                            )"
                                    );
            cmd.Parameters.AddWithValue("docHdr", req);
            await SqlCommand.ExecuteNonQuery(cmd);
        }

        // DELETE api/Todo/5
        [HttpDelete("{uid}")]
        public async Task Delete(string uid)
        {
            var cmd = new SqlCommand(@"delete from [dbo].[n_warehouse_document_header] where uid = @uid");
            cmd.Parameters.AddWithValue("uid", uid.ToUpper());
            await SqlCommand.ExecuteNonQuery(cmd);
        }


    }
}

