using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Belgrade.SqlClient;
using System.Data.SqlClient;
using System.IO;

namespace WMS_Api.Controllers
{
    [Route("api/[controller]")]
    public class DocLineController : Controller
    {
        private readonly IQueryPipe SqlPipe;
        private readonly ICommand SqlCommand;

        public DocLineController(ICommand sqlCommand, IQueryPipe sqlPipe)
        {
            this.SqlCommand = sqlCommand;
            this.SqlPipe = sqlPipe;
        }

        // GET api/DocLine
        [HttpGet]
        public async Task Get()
        {
            await SqlPipe.Stream("SELECT[line_no]" +
                ",[document_type]" +
                ",[transaction_no]" +
                ",[document_no]" + 
                ",[document_date]" +
                ",[entry_state]" +
                ",[item_no]" +
                ",[item_barcode]" +
                ",[planned_quantity]" +
                ",[received_quantity]" +
                ",[comment]" +
                ",[processed_quantity]" +
                ",[processed_date_time]" +
                ",[worker_code]" +
                ",[warehouse_code]" +
                ",[bin]" +
                ",[org_bin]" +
                ",[uid]" +
                ",[unit_of_measure]" +
                ",[status]" +
                ",[add_line]" +
                ",[last_update]" + 
                "FROM[dbo].[n_warehouse_document_line] FOR JSON PATH"
                , Response.Body, "[]");
        }

        // GET api/DocLine/uid
        [HttpGet("{uid}")]
        public async Task Get(string uid)
        {
            var cmd = new SqlCommand(@"SELECT [line_no]
                                          ,[document_type]
                                          ,[transaction_no]
                                          ,[document_no]
                                          ,[document_date]
                                          ,[entry_state]
                                          ,[item_no]
                                          ,[item_barcode]
                                          ,[planned_quantity]
                                          ,[received_quantity]
                                          ,[comment]
                                          ,[processed_quantity]
                                          ,[processed_date_time]
                                          ,[worker_code]
                                          ,[warehouse_code]
                                          ,[bin]
                                          ,[org_bin]
                                          ,[uid]
                                          ,[unit_of_measure]
                                          ,[status]
                                          ,[add_line]
                                      FROM [dbo].[n_warehouse_document_line] where uid = @uid FOR JSON PATH, WITHOUT_ARRAY_WRAPPER");
            cmd.Parameters.AddWithValue("uid", uid.ToUpper());
            await SqlPipe.Stream(cmd, Response.Body, "{}");
        }

        // GET api/worker/TEST
        [HttpGet("exists/{uid}/{transaction}")]
        public async Task Exists(string uid, string transaction)
        {
            var cmd = new SqlCommand(@"select (case when exists 
                                        (SELECT * from [dbo].[n_warehouse_document_line] where [transaction_no] = @transaction and [uid] =  @uid) 
	            then 'true' else 'false' end) as [status] for json path, WITHOUT_ARRAY_WRAPPER");
            cmd.Parameters.AddWithValue("uid", uid.ToUpper());
            cmd.Parameters.AddWithValue("transaction", transaction.ToUpper());
            await SqlPipe.Stream(cmd, Response.Body, "{}");
        }

        // POST api/DocLine
       
        [HttpPost]
        public async Task Post()
        {
            string req = new StreamReader(Request.Body).ReadToEnd();
            var cmd = new SqlCommand(
                                        @"insert into [dbo].[n_warehouse_document_line]
                                        select *
                                        from OPENJSON(@docLine)
                                        WITH(
                                             [line_no] [int],
                                             [document_type] [int],   
	                                         [document_no] [nvarchar](20),
	                                         [document_date] [datetime],
	                                         [entry_state] [int],
	                                         [item_no] [nvarchar](20),
	                                         [item_barcode] [nvarchar](20),
	                                         [planned_quantity] [decimal](38, 20),
	                                         [received_quantity] [decimal](38, 20),
	                                         [comment] [nvarchar](100),
	                                         [processed_quantity] [decimal](38, 20),
	                                         [processed_date_time] [datetime],
                                             [last_update] [datetime],
	                                         [worker_code] [nvarchar](20),
	                                         [warehouse_code] [nvarchar](20),
	                                         [bin] [nvarchar](20),
                                             [org_bin] [nvarchar](50),
	                                         [uid] [nvarchar](100),
	                                         [unit_of_measure] [nvarchar](20),
	                                         [status] [int],
                                             [add_line] [int],
                                             [transaction_no] [nvarchar](20),
                                             [inventory_document] [nvarchar](20)
                                            )"
                                    );
            cmd.Parameters.AddWithValue("docLine", req);
            await SqlCommand.ExecuteNonQuery(cmd);
        }

        // DELETE api/Todo/5
        [HttpDelete("{uid}")]
        public async Task Delete(string uid)
        {
            var cmd = new SqlCommand(@"delete from [dbo].[n_warehouse_document_line] where uid = @uid");
            cmd.Parameters.AddWithValue("uid", uid.ToUpper());
            await SqlCommand.ExecuteNonQuery(cmd);
        }
    }
}

