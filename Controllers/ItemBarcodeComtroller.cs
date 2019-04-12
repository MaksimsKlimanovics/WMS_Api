using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Belgrade.SqlClient;
using System.Data.SqlClient;
using System.IO;

namespace WMS_Api.Controllers
{
    [Route("api/[controller]")]
    public class ItemBarcodeController : Controller
    {
        private readonly IQueryPipe SqlPipe;
        private readonly ICommand SqlCommand;
        
        public ItemBarcodeController(ICommand sqlCommand, IQueryPipe sqlPipe)
        {
            this.SqlCommand = sqlCommand;
            this.SqlPipe = sqlPipe;
        }

        // GET api/itemBarcode
        [HttpGet]
        public async Task Get()
        {
            await SqlPipe.Stream("select [Item No.]" +
                                    ",[Barcode]" +
                                    ",[Inactive] from [dbo].[n_item_barcode] FOR JSON PATH"
           , Response.Body, "[]");
        }

        // GET api/itemBarcode/test 
        // Just checking comment line.
        [HttpGet("{itemNo}/{barcode}")]
        public async Task Get(string itemNo, string barcode)
        {
            
            var cmd = new SqlCommand(@"select [Item No.]
                                             ,[Barcode]
                                             ,[Inactive] from [dbo].[n_item_barcode] where [Item No.] = @itemNo and [Barcode] = @barcode FOR JSON PATH, WITHOUT_ARRAY_WRAPPER");
            cmd.Parameters.AddWithValue("itemNo", itemNo.ToUpper());
            cmd.Parameters.AddWithValue("barcode", barcode.ToUpper());
            await SqlPipe.Stream(cmd, Response.Body, "{}");
        }

        // GET api/itemBarcode/exists
        [HttpGet("exists/{itemNo}/{barcode}")]
        public async Task Exists(string itemNo, string barcode)
        {
            var cmd = new SqlCommand(@"select (case when exists 
                                        (SELECT * from [dbo].[n_item_barcode] where [Item No.] = @itemNo and [Barcode] = @barcode) 
	                                   then 'true' else 'false' end) as [status] for json path, WITHOUT_ARRAY_WRAPPER");
            cmd.Parameters.AddWithValue("itemNo", itemNo.ToUpper());
            cmd.Parameters.AddWithValue("barcode", barcode.ToUpper());
            await SqlPipe.Stream(cmd, Response.Body, "{}");
        }

        // POST api/itemBarcode
        [HttpPost]
        public async Task Post()
        {
            string req = new StreamReader(Request.Body).ReadToEnd();
            var cmd = new SqlCommand(
                                        @"insert into [dbo].[n_item_barcode]
                                        select *
                                        from OPENJSON(@barcode)
                                        WITH([ItemNo] [nvarchar](20)
                                             ,[Barcode] [nvarchar](20)
                                             ,[Inactive] [int]
                                            )"
                                    );
            cmd.Parameters.AddWithValue("barcode", req);
            await SqlCommand.ExecuteNonQuery(cmd);
        }

        // DELETE api/itemBarcode/delete
        [HttpDelete("{itemNo}/{barcode}")]
        public async Task Delete(string itemNo, string barcode)
        {
            var cmd = new SqlCommand(@"delete from [dbo].[n_item_barcode] where [Item No.] = @itemNo and [Barcode] = @barcode");
            cmd.Parameters.AddWithValue("itemNo", itemNo.ToUpper());
            cmd.Parameters.AddWithValue("barcode", barcode.ToUpper());
            await SqlCommand.ExecuteNonQuery(cmd);
        }

       

    }
}
