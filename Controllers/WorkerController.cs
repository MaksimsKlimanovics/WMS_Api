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
    public class WorkerController : Controller
    {
        private readonly IQueryPipe SqlPipe;
        private readonly ICommand SqlCommand;

        public WorkerController(ICommand sqlCommand, IQueryPipe sqlPipe)
        {
            this.SqlCommand = sqlCommand;
            this.SqlPipe = sqlPipe;
        }

        // GET api/worker
        [HttpGet]
        public async Task Get()
        {
            await SqlPipe.Stream("SELECT [id]" +
                                        ", [type]" +
                                        ", [name]" +
                                        ", [address]" +
                                        ", [city]" +
                                        ", [phone]" +
                                        ", [email]" +
                                        ", [skype]" +
                                        ", [location] " +
                                 "FROM [dbo].[n_worker] FOR JSON PATH"
                , Response.Body, "[]");
        }

        // GET api/worker/TEST
        [HttpGet("{id}")]
        public async Task Get(string id)
        {
            var cmd = new SqlCommand(@"select [id]
                                              , [type]
                                              , [name]
                                              , [address]
                                              , [city]
                                              , [phone]
                                              , [email]
                                              , [skype]
                                              , [location]
                                       from [dbo].[n_worker] where id = @id FOR JSON PATH, WITHOUT_ARRAY_WRAPPER");
            cmd.Parameters.AddWithValue("id", id.ToUpper());
            await SqlPipe.Stream(cmd, Response.Body, "{}");
        }

        // POST api/worker
        [HttpPost]
        public async Task Post()
        {
            string req = new StreamReader(Request.Body).ReadToEnd();
            var cmd = new SqlCommand(
                                        @"insert into [dbo].[n_worker]
                                        select *
                                        from OPENJSON(@worker)
                                        WITH([id] [varchar](10)
                                             ,[type] [varchar](15)
                                             ,[name] [varchar](50)
                                             ,[address] [varchar](50)
                                             ,[city] [varchar](30)
                                             ,[phone] [varchar](30)
                                             ,[email] [varchar](80)
                                             ,[skype] [varchar](50)
                                             ,[location] [varchar](20)
                                            )"
                                    );
            cmd.Parameters.AddWithValue("worker", req);
            await SqlCommand.ExecuteNonQuery(cmd);
        }

        // DELETE api/Todo/5
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var cmd = new SqlCommand(@"delete from n_worker where id = @id");
            cmd.Parameters.AddWithValue("id", id.ToUpper());
            await SqlCommand.ExecuteNonQuery(cmd);
        }
    }
}
