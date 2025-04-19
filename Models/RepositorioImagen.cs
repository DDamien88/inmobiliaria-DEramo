using inmobiliariaDEramo.Models;
using MySqlConnector;
using System.Data;


namespace InmobiliariaDEramo.Models
{
    public class RepositorioImagen : RepositorioBase, IRepositorioImagen
    {
        public RepositorioImagen(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(Imagen p)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"INSERT INTO imagenes 
					(InmuebleId, Url) 
					VALUES (@inmuebleId, @url)";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@inmuebleId", p.InmuebleId);
                    command.Parameters.AddWithValue("@url", p.Url);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public int Baja(int id)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @$"DELETE FROM imagenes WHERE Id = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public int Modificacion(Imagen p)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
				UPDATE imagenes SET 
					Url=@url
				WHERE Id=@id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id", p.Id);
                    command.Parameters.AddWithValue("@url", p.Url);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public Imagen ObtenerPorId(int id)
        {
            Imagen res = null;
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string sql = @$"
					SELECT 
						{nameof(Imagen.Id)}, 
						{nameof(Imagen.InmuebleId)}, 
						{nameof(Imagen.Url)} 
					FROM imagenes
					WHERE {nameof(Imagen.Id)}=@id";
                using (MySqlCommand comm = new MySqlCommand(sql, conn))
                {
                    comm.Parameters.AddWithValue("@id", id);
                    conn.Open();
                    var reader = comm.ExecuteReader();
                    if (reader.Read())
                    {
                        res = new Imagen();
                        res.Id = reader.GetInt32(nameof(Imagen.Id));
                        res.InmuebleId = reader.GetInt32(nameof(Imagen.InmuebleId));
                        res.Url = reader.GetString(nameof(Imagen.Url));
                    }
                    conn.Close();
                }
            }
            return res;
        }

        public IList<Imagen> ObtenerTodos()
        {
            List<Imagen> res = new List<Imagen>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string sql = @$"
					SELECT 
						{nameof(Imagen.Id)}, 
						{nameof(Imagen.InmuebleId)}, 
						{nameof(Imagen.Url)} 
					FROM imagenes";
                using (MySqlCommand comm = new MySqlCommand(sql, conn))
                {
                    conn.Open();
                    var reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        res.Add(new Imagen
                        {
                            Id = reader.GetInt32(nameof(Imagen.Id)),
                            InmuebleId = reader.GetInt32(nameof(Imagen.InmuebleId)),
                            Url = reader.GetString(nameof(Imagen.Url)),
                        });
                    }
                    conn.Close();
                }
            }
            return res;
        }

        public IList<Imagen> BuscarPorInmueble(int inmuebleId)
        {
            List<Imagen> res = new List<Imagen>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string sql = @$"
					SELECT 
						{nameof(Imagen.Id)}, 
						{nameof(Imagen.InmuebleId)}, 
						{nameof(Imagen.Url)} 
					FROM imagenes
					WHERE {nameof(Imagen.InmuebleId)}=@inmuebleId";
                using (MySqlCommand comm = new MySqlCommand(sql, conn))
                {
                    comm.Parameters.AddWithValue("@inmuebleId", inmuebleId);
                    conn.Open();
                    var reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        res.Add(new Imagen
                        {
                            Id = reader.GetInt32(nameof(Imagen.Id)),
                            InmuebleId = reader.GetInt32(nameof(Imagen.InmuebleId)),
                            Url = reader.GetString(nameof(Imagen.Url)),
                        });
                    }
                    conn.Close();
                }
            }
            return res;
        }


    }
}