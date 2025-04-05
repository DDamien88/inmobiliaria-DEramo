using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using MySqlConnector;

namespace inmobiliariaDEramo.Models
{
    public class RepositorioInmueble : RepositorioBase, IRepositorioInmueble
    {
        public RepositorioInmueble(IConfiguration configuration) : base(configuration)
        {

        }

        public int Alta(Inmueble entidad)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"INSERT INTO inmuebles 
					(Direccion, Uso, Tipo, Precio, Ambientes, Superficie, Latitud, Longitud, PropietarioId)
					VALUES (@direccion, @uso, @tipo, @precio, @ambientes, @superficie, @latitud, @longitud, @propietarioId);
					SELECT LAST_INSERT_ID();";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@direccion", entidad.Direccion == null ? DBNull.Value : entidad.Direccion);
                    command.Parameters.AddWithValue("@uso", entidad.Uso);
                    command.Parameters.AddWithValue("@tipo", entidad.Tipo);
                    command.Parameters.AddWithValue("@precio", entidad.Precio);
                    command.Parameters.AddWithValue("@ambientes", entidad.Ambientes);
                    command.Parameters.AddWithValue("@superficie", entidad.Superficie);
                    command.Parameters.AddWithValue("@latitud", entidad.Latitud);
                    command.Parameters.AddWithValue("@longitud", entidad.Longitud);
                    command.Parameters.AddWithValue("@propietarioId", entidad.PropietarioId);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    entidad.Id = res;
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
                string sql = @$"DELETE FROM inmuebles WHERE Id = @id";
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
        public int Modificacion(Inmueble entidad)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "UPDATE inmuebles SET " +
    "Direccion=@direccion, Uso = @uso, Tipo = @tipo, Precio = @precio, Ambientes=@ambientes, Superficie=@superficie, Latitud=@latitud, Longitud=@longitud, PropietarioId=@propietarioId " +
    "WHERE Id = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@direccion", entidad.Direccion);
                    command.Parameters.AddWithValue("@uso", entidad.Uso);
                    command.Parameters.AddWithValue("@tipo", entidad.Tipo);
                    command.Parameters.AddWithValue("@precio", entidad.Precio);
                    command.Parameters.AddWithValue("@ambientes", entidad.Ambientes);
                    command.Parameters.AddWithValue("@superficie", entidad.Superficie);
                    command.Parameters.AddWithValue("@latitud", entidad.Latitud);
                    command.Parameters.AddWithValue("@longitud", entidad.Longitud);
                    command.Parameters.AddWithValue("@propietarioId", entidad.PropietarioId);
                    command.Parameters.AddWithValue("@id", entidad.Id);
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public IList<Inmueble> ObtenerTodos()
        {
            IList<Inmueble> res = new List<Inmueble>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT Id, Direccion, Uso, Tipo, Precio, Ambientes, Superficie, Latitud, Longitud, PropietarioId,
					p.Nombre, p.Apellido, p.Dni
					FROM inmuebles i INNER JOIN propietarios p ON i.PropietarioId = p.IdPropietario";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Inmueble entidad = new Inmueble
                        {
                            Id = reader.GetInt32(0),
                            Direccion = reader["Direccion"] == DBNull.Value ? "" : reader.GetString("Direccion"),
                            Uso = reader.GetString(2),
                            Tipo = reader.GetString(3),
                            Precio = reader.GetDouble(4),
                            Ambientes = reader.GetInt32(5),
                            Superficie = reader.GetInt32(6),
                            Latitud = reader.GetDecimal(7),
                            Longitud = reader.GetDecimal(8),
                            PropietarioId = reader.GetInt32(9),
                            Duenio = new Propietario
                            {
                                IdPropietario = reader.GetInt32(9),
                                Nombre = reader.GetString(10),
                                Apellido = reader.GetString(11),
                            }
                        };

                        res.Add(entidad);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public Inmueble ObtenerPorId(int id)
        {
            Inmueble entidad = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @$"
					SELECT {nameof(Inmueble.Id)}, Direccion, Uso, Tipo, Precio, Ambientes, Superficie, Latitud, Longitud, PropietarioId, p.Nombre, p.Apellido
					FROM inmuebles i JOIN propietarios p ON i.PropietarioId = p.IdPropietario
					WHERE {nameof(Inmueble.Id)}=@id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        entidad = new Inmueble
                        {
                            Id = reader.GetInt32(nameof(Inmueble.Id)),
                            Direccion = reader["Direccion"] == DBNull.Value ? "" : reader.GetString("Direccion"),
                            Ambientes = reader.GetInt32("Ambientes"),
                            Uso = reader.GetString("Uso"),
                            Tipo = reader.GetString("Tipo"),
                            Precio = reader.GetDouble("Precio"),
                            Superficie = reader.GetInt32("Superficie"),
                            Latitud = reader.GetDecimal("Latitud"),
                            Longitud = reader.GetDecimal("Longitud"),
                            PropietarioId = reader.GetInt32("PropietarioId"),
                            Duenio = new Propietario
                            {
                                IdPropietario = reader.GetInt32("PropietarioId"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                            }
                        };
                    }
                    connection.Close();
                }
            }
            return entidad;
        }

        public IList<Inmueble> BuscarPorPropietario(int idPropietario)
        {
            List<Inmueble> res = new List<Inmueble>();
            Inmueble entidad = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @$"
					SELECT {nameof(Inmueble.Id)}, Direccion, Uso, Tipo, Precio, Ambientes, Superficie, Latitud, Longitud, PropietarioId, p.Nombre, p.Apellido
					FROM inmuebles i JOIN propietarios p ON i.PropietarioId = p.IdPropietario
					WHERE PropietarioId=@idPropietario";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.Add("@idPropietario", MySqlDbType.Int32).Value = idPropietario;
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        entidad = new Inmueble
                        {
                            Id = reader.GetInt32(nameof(Inmueble.Id)),
                            Direccion = reader["Direccion"] == DBNull.Value ? "" : reader.GetString("Direccion"),
                            Uso = reader.GetString("Uso"),
                            Tipo = reader.GetString("Tipo"),
                            Precio = reader.GetDouble("Precio"),
                            Ambientes = reader.GetInt32("Ambientes"),
                            Superficie = reader.GetInt32("Superficie"),
                            Latitud = reader.GetDecimal("Latitud"),
                            Longitud = reader.GetDecimal("Longitud"),
                            PropietarioId = reader.GetInt32("PropietarioId"),
                            Duenio = new Propietario
                            {
                                IdPropietario = reader.GetInt32("PropietarioId"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                            }
                        };
                        res.Add(entidad);
                    }
                    connection.Close();
                }
            }
            return res;
        }




        public IList<Inmueble> BuscarPorDireccion(string direccion)
        {
            var lista = new List<Inmueble>();
            direccion = "%" + direccion + "%";

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
            SELECT i.Id, i.Direccion, i.Uso, i.Tipo,
                   p.Nombre AS PropietarioNombre, p.Apellido AS PropietarioApellido, p.Dni AS PropietarioDni
            FROM inmuebles i
            JOIN propietarios p ON i.PropietarioId = p.IdPropietario
            WHERE i.Direccion LIKE @direccion";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.Add("@direccion", DbType.String).Value = direccion;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var inmueble = new Inmueble
                        {
                            Id = reader.GetInt32("Id"),
                            Direccion = reader.GetString("Direccion"),
                            Uso = reader.GetString("Uso"),
                            Tipo = reader.GetString("Tipo"),
                            Duenio = new Propietario
                            {
                                Nombre = reader.GetString("PropietarioNombre"),
                                Apellido = reader.GetString("PropietarioApellido"),
                                Dni = reader.GetString("PropietarioDni")
                            }
                        };
                        lista.Add(inmueble);
                    }
                    connection.Close();
                }
            }

            return lista;
        }
    }
}
