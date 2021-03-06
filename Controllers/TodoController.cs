﻿using System;
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
    public class TodoController : Controller
    {
        private readonly IQueryPipe SqlPipe;
        private readonly ICommand SqlCommand;

        public TodoController(ICommand sqlCommand, IQueryPipe sqlPipe)
        {
            this.SqlCommand = sqlCommand;
            this.SqlPipe = sqlPipe;
        }

        // GET api/Todo
        [HttpGet]
        public async Task Get()
        {
            await SqlPipe.Stream("select id, full_name, email from users FOR JSON PATH", Response.Body, "[]");
        }

        // GET api/Todo/5
        [HttpGet("{id}")]
        public async Task Get(int id)
        {
            var cmd = new SqlCommand("select * from Todo where id = @id FOR JSON PATH, WITHOUT_ARRAY_WRAPPER");
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
