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
    public class ItemsController : Controller
    {
        private readonly IQueryPipe SqlPipe;
        private readonly ICommand SqlCommand;

        public ItemsController(ICommand sqlCommand, IQueryPipe sqlPipe)
        {
            this.SqlCommand = sqlCommand;
            this.SqlPipe = sqlPipe;
        }

        // GET api/Items
        [HttpGet]
        public async Task Get()
        {
            await SqlPipe.Stream("SELECT [no.] as [no]" +
                                    ", [additional_no.] as [add_no]" +
                                    ", [description]" +
                                    ", [base_uom]" +
                                    ", [vendor_num.] as [vend_num]" +
                                    ", [vendor_item_num.] as [vend_itm_num]" +
                                    ", [gross_weight]" +
                                    ", [net_weight]" +
                                    ", [barcode_no.2] as [barcode_no_2]" +
                                    ", [barcode_no.3] as [barcode_no_3]" +
                                 "FROM [dbo].[n_item] FOR JSON PATH"
                , Response.Body, "[]");
        }

        // GET api/items/TEST
        [HttpGet("{no}")]
        public async Task Get(string no)
        {
            var cmd = new SqlCommand(@"SELECT [no.] as [no]
                                                ,[additional_no.] as [additional_no]
                                                ,[description]
                                                ,[base_uom]
                                                ,[vendor_num.] as [vendor_num]
                                                ,[vendor_item_num.] as [vendor_item_num]
                                                ,[gross_weight]
                                                ,[net_weight]
                                                ,[barcode_no.2] as [barcode_no_2]
                                                ,[barcode_no.3] as [barcode_no_3]
                                       FROM [dbo].[n_item] where [no.] = @no FOR JSON PATH, WITHOUT_ARRAY_WRAPPER");
            cmd.Parameters.AddWithValue("no", no.ToUpper());
            await SqlPipe.Stream(cmd, Response.Body, "{}");
        }

        // POST api/items
        [HttpPost]
        public async Task Post()
        {
            string req = new StreamReader(Request.Body).ReadToEnd();
            var cmd = new SqlCommand(
                                        @"insert into [dbo].[n_item]
                                        select *
                                        from OPENJSON(@item)
                                        WITH([no_] [nvarchar](20),
	                                            [additional_no_] [nvarchar](20),
	                                            [description] [nvarchar](50),
	                                            [base_uom] [nvarchar](10),
	                                            [vendor_num_] [nvarchar](20),
	                                            [vendor_item_num_] [nvarchar](20),
	                                            [gross_weight] [decimal](38, 20),
	                                            [net_weight] [decimal](38, 20),
	                                            [barcode_no_2] [nvarchar](20),
	                                            [barcode_no_3] [nvarchar](20)
                                            )"
                                    );
            cmd.Parameters.AddWithValue("item", req);
            await SqlCommand.ExecuteNonQuery(cmd);
        }

        // DELETE api/items/5
        [HttpDelete("{no}")]
        public async Task Delete(string no)
        {
            var cmd = new SqlCommand(@"delete from n_item where [no.] = @no");
            cmd.Parameters.AddWithValue("no", no.ToUpper());
            await SqlCommand.ExecuteNonQuery(cmd);
        }
    }
}
