using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace todoappapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoAPP : ControllerBase
    {

        private IConfiguration _configuration;

        public TodoAPP(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("GetNotes")]
        public async Task<IActionResult> GetNotes()
        {
            try
            {
                string query = "SELECT * FROM dbo.notes";
                DataTable table = new DataTable();
                string sqlDatasource = _configuration.GetConnectionString("todoAppDBCon");
                using (SqlConnection myCon = new SqlConnection(sqlDatasource))
                {
                    await myCon.OpenAsync();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        using (SqlDataReader myReader = await myCommand.ExecuteReaderAsync())
                        {
                            table.Load(myReader);
                        }
                    }
                }

                return Ok(table);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, new { error = "Internal Server Error", message = ex.Message });
            }
        }


        [HttpPost]
        [Route("AddNote")]
        public JsonResult AddNotes([FromForm] string newNotes)
        {
            string query = "insert into dbo.notes values(@newNotes)";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("todoAppDBCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@newNotes", newNotes);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult("Added Successfully");
        }

        [HttpDelete]
        [Route("DeleteNote")]
        public JsonResult DeleteNote(int id)
        {
            try
            {
                string query = "DELETE FROM dbo.notes WHERE id=@id";
                DataTable table = new DataTable();
                string sqlDatasource = _configuration.GetConnectionString("todoAppDBCon");
                SqlDataReader myReader;
                using (SqlConnection myCon = new SqlConnection(sqlDatasource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@id", id);
                        myReader = myCommand.ExecuteReader();
                        table.Load(myReader);
                        myReader.Close();
                        myCon.Close();
                    }
                }

                return new JsonResult("Note Deleted Successfully");
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return new JsonResult($"Error: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("DeleteAllNotes")]
        public JsonResult DeleteAllNotes()
        {
            try
            {
                string query = "DELETE FROM dbo.notes";
                string sqlDatasource = _configuration.GetConnectionString("todoAppDBCon");
                using (SqlConnection myCon = new SqlConnection(sqlDatasource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.ExecuteNonQuery();
                    }
                }

                return new JsonResult("All Notes Deleted Successfully");
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return new JsonResult($"Error: {ex.Message}");
            }
        }


        [HttpGet("GetNoteById/{id}")]
        public async Task<IActionResult> GetNoteById(int id)
        {
            try
            {
                string query = "SELECT * FROM dbo.notes WHERE id = @id";
                DataTable table = new DataTable();
                string sqlDatasource = _configuration.GetConnectionString("todoAppDBCon");
                using (SqlConnection myCon = new SqlConnection(sqlDatasource))
                {
                    await myCon.OpenAsync();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader myReader = await myCommand.ExecuteReaderAsync())
                        {
                            table.Load(myReader);
                        }
                    }
                }

                if (table.Rows.Count > 0)
                {
                    return Ok(table);
                }
                else
                {
                    return NotFound("Note not found");
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, new { error = "Internal Server Error", message = ex.Message });
            }
        }

        [HttpPut]
        [Route("UpdateNote/{id}")]
        public JsonResult UpdateNote(int id, [FromBody] string newNotes)
        {
            try
            {
                string query = "UPDATE dbo.notes SET description = @newNotes WHERE id = @id";
                DataTable table = new DataTable();
                string sqlDatasource = _configuration.GetConnectionString("todoAppDBCon");

                using (SqlConnection myCon = new SqlConnection(sqlDatasource))
                {
                    myCon.Open();

                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@id", id);
                        myCommand.Parameters.AddWithValue("@newNotes", newNotes);

                        int rowsAffected = myCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return new JsonResult("Note Updated Successfully");
                        }
                        else
                        {
                            return new JsonResult("Note not found or no changes made");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return new JsonResult($"Error: {ex.Message}");
            }
        }



    }
}
