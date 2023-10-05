using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Jegyek.Controllers
{
    public class JegyController : Controller
    {
        public DbConnection Database = new();

        [HttpGet, Route("/getAll")]
        public IActionResult Get()
        {
            try
            {
                Database.Conn.Open();

                List<Jegy> entries = new();
                var command = Database.Conn.CreateCommand();
                command.CommandText = "SELECT * FROM adatok";
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    entries.Add(new Jegy
                    {
                        Id = reader.GetGuid("id"),
                        Ertek = reader.GetInt32("ertek"),
                        Leiras = reader.GetString("leiras"),
                        Letrehozas = reader.GetDateTime("letrehozas")
                    });
                }

                return Ok(entries);
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
            finally
            {
                Database.Conn.Close();
            }
        }

        [HttpGet, Route("/getById")]
        public IActionResult GetById([Required] Guid id)
        {
            try
            {
                Database.Conn.Open();

                var command = Database.Conn.CreateCommand();
                command.Parameters.AddWithValue("Id", id);
                command.CommandText = "SELECT * FROM adatok WHERE id = @Id";
                command.Prepare();
                var reader = command.ExecuteReader();

                if (!reader.HasRows) return NotFound();

                reader.Read();
                return Ok(new Jegy
                {
                    Id = reader.GetGuid("id"),
                    Ertek = reader.GetInt32("ertek"),
                    Leiras = reader.GetString("leiras"),
                    Letrehozas = reader.GetDateTime("letrehozas")
                });

            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
            finally
            {
                Database.Conn.Close();
            }
        }

        [HttpPost, Route("/add")]
        public IActionResult Post([FromBody, Required] Dtos.PostDto dto)
        {
            try
            {
                Database.Conn.Open();

                var jegy = new Jegy
                {
                    Id = Guid.NewGuid(),
                    Ertek = dto.Jegy,
                    Leiras = dto.Leiras,
                    Letrehozas = DateTime.Now
                };

                var command = Database.Conn.CreateCommand();
                command.Parameters.AddWithValue("Id", jegy.Id);
                command.Parameters.AddWithValue("Ertek", jegy.Ertek);
                command.Parameters.AddWithValue("Leiras", jegy.Leiras);
                command.Parameters.AddWithValue("Letrehozas", jegy.Letrehozas);
                command.CommandText = "INSERT INTO adatok VALUES(@Id, @Ertek, @Leiras, @Letrehozas)";
                return command.ExecuteNonQuery() > 0 ? Ok(jegy) : Problem("A jegy nem lett hozzáadva.");
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
            finally
            {
                Database.Conn.Close();
            }
        }

        [HttpPut, Route("/update")]
        public IActionResult Put([FromBody, Required] Dtos.PutDto dto)
        {
            try
            {
                Database.Conn.Open();

                var command = Database.Conn.CreateCommand();
                command.Parameters.AddWithValue("Id", dto.Id);
                command.CommandText = "SELECT null FROM adatok WHERE id = @Id";
                var reader = command.ExecuteReader();
                if (!reader.HasRows) return NotFound();
                Database.Conn.Close();

                Database.Conn.Open();
                var command2 = Database.Conn.CreateCommand();
                command2.Parameters.AddWithValue("Jegy", dto.Jegy);
                command2.Parameters.AddWithValue("Leiras", dto.Leiras);
                command2.Parameters.AddWithValue("Id", dto.Id);
                command2.CommandText = "UPDATE adatok SET ertek = @Jegy, leiras = @Leiras WHERE id = @Id";
                return command2.ExecuteNonQuery() > 0 ? Ok("Frissítve.") : Problem("A jegy nem lett frissítve.");
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
            finally
            {
                Database.Conn.Close();
            }
        }

        [HttpDelete, Route("/remove")]
        public IActionResult Delete([Required] Guid id)
        {
            try
            {
                Database.Conn.Open();

                var command = Database.Conn.CreateCommand();
                command.Parameters.AddWithValue("Id", id);
                command.CommandText = "SELECT null FROM adatok WHERE id = @Id";
                var reader = command.ExecuteReader();
                if (!reader.HasRows) return NotFound();
                Database.Conn.Close();

                Database.Conn.Open();
                var command2 = Database.Conn.CreateCommand();
                command2.Parameters.AddWithValue("Id", id);
                command2.CommandText = "DELETE FROM adatok WHERE id = @Id";
                return command2.ExecuteNonQuery() > 0 ? Ok("Törölve") : Problem("A jegy nem lett törölve.");
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
            finally
            {
                Database.Conn.Close();
            }
        }
    }
}