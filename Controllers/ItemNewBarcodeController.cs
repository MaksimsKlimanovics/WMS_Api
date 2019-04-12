using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Belgrade.SqlClient;
using System.Data.SqlClient;
using System.IO;

namespace WMS_Api.Controllers
{
    [Route("api/[controller]")]
    public class ItemNewBarcodeController : Controller
    {
        private readonly IQueryPipe SqlPipe;
        private readonly ICommand SqlCommand;
        
        public ItemNewBarcodeController(ICommand sqlCommand, IQueryPipe sqlPipe)
        {
            this.SqlCommand = sqlCommand;
            this.SqlPipe = sqlPipe;
        }

        // GET api/itemNewBarcodes
        [HttpGet]
        public async Task Get()
        {
            await SqlPipe.Stream("select [Id]" +
                                    ",[ItemCode]" +
                                    ",[Barcode]" +
                                    ",[TransactionNo]" +
                                    ",[createdBy]" +
                                    ",[createdDate] from [dbo].[new_item_barcodes] FOR JSON PATH"
           , Response.Body, "[]");
        }

        // GET api/itemBarcode/test 
        // Just checking comment line.
        [HttpGet("{itemNo}/{barcode}")]
        public async Task Get(string itemNo, string barcode)
        {
            
            var cmd = new SqlCommand(@"select [Item No.]
                                             ,[Barcode]
                                             ,[Inactive] from [dbo].[new_item_barcodes] 
                                        where [ItemCode] = @itemNo and [Barcode] = @barcode FOR JSON PATH, WITHOUT_ARRAY_WRAPPER");
            cmd.Parameters.AddWithValue("itemNo", itemNo.ToUpper());
            cmd.Parameters.AddWithValue("barcode", barcode.ToUpper());
            await SqlPipe.Stream(cmd, Response.Body, "{}");
        }

        // GET api/itemBarcode/exists
        [HttpGet("exists/{itemNo}/{barcode}")]
        public async Task Exists(string itemNo, string barcode)
        {
            var cmd = new SqlCommand(@"select (case when exists 
                                        (SELECT * from [dbo].[new_item_barcodes] 
                                        where [ItemCode] = @itemNo and [Barcode] = @barcode) 
	                                   then 'true' else 'false' end) as [status] for json path, WITHOUT_ARRAY_WRAPPER");
            cmd.Parameters.AddWithValue("itemNo", itemNo.ToUpper());
            cmd.Parameters.AddWithValue("barcode", barcode.ToUpper());
            await SqlPipe.Stream(cmd, Response.Body, "{}");
        }


       

    }
}
