using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Belgrade.SqlClient;
using System.Data.SqlClient;
using System.IO;

namespace TodoApp.Controllers
{
    [Route("api/[controller]")]
    public class InventoryController : Controller
    {
        private readonly IQueryPipe SqlPipe;
        private readonly ICommand SqlCommand;

        public InventoryController(ICommand sqlCommand, IQueryPipe sqlPipe)
        {
            this.SqlCommand = sqlCommand;
            this.SqlPipe = sqlPipe;
        }

        // GET api/Todo
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
                                 "FROM [dbo].[n_item] FOR JSON PATH", Response.Body, "[]");
        }

        // GET api/Todo/5
        [HttpGet("{id}")]
        public async Task Get(int id)
        {
            var cmd = new SqlCommand("select * from [dbo].[n_item] where [no.] = @id FOR JSON PATH, WITHOUT_ARRAY_WRAPPER");
            cmd.Parameters.AddWithValue("id", id);
            await SqlPipe.Stream(cmd, Response.Body, "{}");
        }
    
        // POST api/Todo
        [HttpPost]
        public async Task Post()
        {
            string todo = new StreamReader(Request.Body).ReadToEnd();
            var cmd = new SqlCommand(
        @"insert into Todo
        select *
        from OPENJSON(@todo)
        WITH( Title nvarchar(30), Description nvarchar(4000), Completed bit, TargetDate datetime2)");
            cmd.Parameters.AddWithValue("todo", todo);
            await SqlCommand.ExecuteNonQuery(cmd);
        }

        // PATCH api/Todo
        [HttpPatch]
        public async Task Patch(int id)
        {
            string todo = new StreamReader(Request.Body).ReadToEnd();
            var cmd = new SqlCommand(
        @"update Todo
        set Title = ISNULL(json.Title, Title), Description = ISNULL(json.Description, Description),
        Completed = ISNULL(json.Completed, Completed), TargetDate = ISNULL(json.TargetDate, TargetDate)
        from OPENJSON(@todo)
        WITH(   Title nvarchar(30), Description nvarchar(4000),
                Completed bit, TargetDate datetime2) AS json
        where Id = @id");
            cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("todo", todo);
            await SqlCommand.ExecuteNonQuery(cmd);
        }

        // PUT api/Todo/5
        [HttpPut("{id}")]
        public async Task Put(int id)
        {
            string todo = new StreamReader(Request.Body).ReadToEnd();
            var cmd = new SqlCommand(
        @"update Todo
        set Title = json.Title, Description = json.Description,
        Completed = json.completed, TargetDate = json.TargetDate
        from OPENJSON( @todo )
        WITH(   Title nvarchar(30), Description nvarchar(4000),
                Completed bit, TargetDate datetime2) AS json
        where Id = @id");
            cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("todo", todo);
            await SqlCommand.ExecuteNonQuery(cmd);
        }

        // DELETE api/Todo/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            var cmd = new SqlCommand(@"delete Todo where Id = @id");
            cmd.Parameters.AddWithValue("id", id);
            await SqlCommand.ExecuteNonQuery(cmd);
        }
    }
}
