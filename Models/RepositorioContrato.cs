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
    public class RepositorioContrato : RepositorioBase, IRepositorioContrato
    {
        public Inmueble Inmueble { get; private set; }

        public RepositorioContrato(IConfiguration configuration) : base(configuration)
        {

        }

        public int Alta(Contrato entidad)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"INSERT INTO contratos 
            (IdInquilino, IdInmueble, MontoMensual, FechaDesde, FechaHasta) 
            VALUES (@idInquilino, @idInmueble, @montoMensual, @fechaDesde, @fechaHasta);
            SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@idInquilino", entidad.IdInquilino);
                    command.Parameters.AddWithValue("@idInmueble", entidad.IdInmueble);
                    command.Parameters.AddWithValue("@montoMensual", entidad.MontoMensual);
                    command.Parameters.AddWithValue("@fechaDesde", entidad.FechaDesde);
                    command.Parameters.AddWithValue("@fechaHasta", entidad.FechaHasta);
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
                string sql = @$"DELETE FROM Contratos WHERE Id = @id";
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



        public int Modificacion(Contrato entidad)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "UPDATE Contratos SET " +
    "IdInquilino = @idInquilino, IdInmueble = @idInmueble, MontoMensual = @montoMensual, FechaDesde = @fechaDesde, FechaHasta = @fechaHasta " +
    "WHERE Id = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idInquilino", entidad.IdInquilino);
                    command.Parameters.AddWithValue("@idInmueble", entidad.IdInmueble);
                    command.Parameters.AddWithValue("@montoMensual", entidad.MontoMensual);
                    command.Parameters.AddWithValue("@fechaDesde", entidad.FechaDesde);
                    command.Parameters.AddWithValue("@fechaHasta", entidad.FechaHasta);
                    command.Parameters.AddWithValue("@id", entidad.Id);
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }



        public IList<Contrato> ObtenerTodos()
        {
            IList<Contrato> res = new List<Contrato>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
            SELECT 
                c.Id, c.IdInquilino, c.IdInmueble, c.MontoMensual, c.FechaDesde, c.FechaHasta,
                i.IdInquilino, i.Nombre, i.Apellido, i.Dni,
                m.Id, m.Direccion, m.PropietarioId,
                p.IdPropietario, p.Nombre AS PropNombre, p.Apellido AS PropApellido, p.Dni AS PropDni
            FROM contratos c
            INNER JOIN inquilinos i ON c.IdInquilino = i.IdInquilino
            INNER JOIN inmuebles m ON c.IdInmueble = m.Id
            INNER JOIN propietarios p ON m.PropietarioId = p.IdPropietario";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Contrato entidad = new Contrato
                        {
                            Id = reader.GetInt32(0),
                            IdInquilino = reader.GetInt32(1),
                            IdInmueble = reader.GetInt32(2),
                            MontoMensual = reader.GetDouble(3),
                            FechaDesde = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4),
                            FechaHasta = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5),
                            Inquilino = new Inquilino
                            {
                                IdInquilino = reader.GetInt32(6),
                                Nombre = reader.GetString(7),
                                Apellido = reader.GetString(8),
                                Dni = reader.GetString(9)
                            },
                            Inmueble = new Inmueble
                            {
                                Id = reader.GetInt32(10),
                                Direccion = reader.GetString(11),
                                PropietarioId = reader.GetInt32(12),
                                Duenio = new Propietario
                                {
                                    IdPropietario = reader.GetInt32(13),
                                    Nombre = reader.GetString(14),
                                    Apellido = reader.GetString(15),
                                    Dni = reader.GetString(16)
                                }
                            }
                        };
                        res.Add(entidad);
                    }
                    connection.Close();
                }
            }
            return res;
        }





        public Contrato ObtenerPorId(int id)
        {
            Contrato entidad = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @$"
            SELECT 
                c.Id, c.IdInquilino, c.IdInmueble, c.MontoMensual, c.FechaDesde, c.FechaHasta,
                i.IdInquilino, i.Nombre, i.Apellido, i.Dni,
                m.Id, m.Direccion, m.PropietarioId,
                p.IdPropietario, p.Nombre AS PropNombre, p.Apellido AS PropApellido, p.Dni AS PropDni
            FROM contratos c
            INNER JOIN inquilinos i ON c.IdInquilino = i.IdInquilino
            INNER JOIN inmuebles m ON c.IdInmueble = m.Id
            INNER JOIN propietarios p ON m.PropietarioId = p.IdPropietario
            WHERE c.Id = @id";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        entidad = new Contrato
                        {
                            Id = reader.GetInt32(0),
                            IdInquilino = reader.GetInt32(1),
                            IdInmueble = reader.GetInt32(2),
                            MontoMensual = reader.GetDouble(3),
                            FechaDesde = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4),
                            FechaHasta = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5),
                            Inquilino = new Inquilino
                            {
                                IdInquilino = reader.GetInt32(6),
                                Nombre = reader.GetString(7),
                                Apellido = reader.GetString(8),
                                Dni = reader.GetString(9)
                            },
                            Inmueble = new Inmueble
                            {
                                Id = reader.GetInt32(10),
                                Direccion = reader.GetString(11),
                                PropietarioId = reader.GetInt32(12),
                                Duenio = new Propietario
                                {
                                    IdPropietario = reader.GetInt32(13),
                                    Nombre = reader.GetString(14),
                                    Apellido = reader.GetString(15),
                                    Dni = reader.GetString(16)
                                }
                            }
                        };
                    }
                    connection.Close();
                }
            }
            return entidad;
        }

        public IList<Contrato> BuscarPorInquilino(int IdInquilino)
        {
            throw new NotImplementedException();
        }




        /*public IList<Contrato> BuscarPorInquilino(int IdInquilino)
        {
            List<Contrato> res = new List<Contrato>();
            Contrato entidad = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @$"
					SELECT {nameof(Contrato.Id)}, Direccion, Uso, Tipo, Precio, Ambientes, Superficie, Latitud, Longitud, PropietarioId, p.Nombre, p.Apellido
					FROM Contratos i JOIN propietarios p ON i.PropietarioId = p.IdPropietario
					WHERE PropietarioId=@idPropietario";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.Add("@idPropietario", MySqlDbType.Int32).Value = IdInquilino;
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        entidad = new Contrato
                        {
                            Id = reader.GetInt32(nameof(Contrato.Id)),
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
        }*/


    }
}