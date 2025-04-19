using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using MySqlConnector;
using InmobiliariaDEramo.Models;

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
            (IdInquilino, IdInmueble, MontoMensual, FechaDesde, FechaHasta, Activo, UsuarioAltaId) 
            VALUES (@idInquilino, @idInmueble, @montoMensual, @fechaDesde, @fechaHasta, 1, @usuarioAltaId);
            SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@idInquilino", entidad.IdInquilino);
                    command.Parameters.AddWithValue("@idInmueble", entidad.IdInmueble);
                    command.Parameters.AddWithValue("@montoMensual", entidad.MontoMensual);
                    command.Parameters.AddWithValue("@fechaDesde", entidad.FechaDesde);
                    command.Parameters.AddWithValue("@fechaHasta", entidad.FechaHasta);
                    //command.Parameters.AddWithValue("@activo", entidad.Activo);
                    command.Parameters.AddWithValue("@usuarioAltaId", entidad.UsuarioAltaId);
                    connection.Open();
                    Console.WriteLine("UsuarioAltaId a guardar: " + entidad.UsuarioAltaId);

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
                string sql = @$"Update contratos SET Activo=0 WHERE {nameof(Contrato.Id)} = @id";
                string sql2 = @$"Update pagos SET Anulado=1 WHERE ContratoId = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
                using (MySqlCommand command = new MySqlCommand(sql2, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.CommandType = CommandType.Text;
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
                c.Id, c.IdInquilino, c.IdInmueble, c.MontoMensual, c.FechaDesde, c.FechaHasta, c.Activo,
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
                            Activo = reader.GetBoolean(6),
                            Inquilino = new Inquilino
                            {
                                IdInquilino = reader.GetInt32(7),
                                Nombre = reader.GetString(8),
                                Apellido = reader.GetString(9),
                                Dni = reader.GetString(10)
                            },
                            Inmueble = new Inmueble
                            {
                                Id = reader.GetInt32(11),
                                Direccion = reader.GetString(12),
                                PropietarioId = reader.GetInt32(13),
                                Duenio = new Propietario
                                {
                                    IdPropietario = reader.GetInt32(14),
                                    Nombre = reader.GetString(15),
                                    Apellido = reader.GetString(16),
                                    Dni = reader.GetString(17)
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
            c.Id, c.IdInquilino, c.IdInmueble, c.MontoMensual, c.FechaDesde, c.FechaHasta, c.Activo,
            c.FechaTerminacionAnticipada, c.MontoMulta, c.MultaPagada, c.UsuarioAltaId, c.UsuarioBajaId,

            i.IdInquilino, i.Nombre, i.Apellido, i.Dni,
            m.Id, m.Direccion, m.PropietarioId,
            p.IdPropietario, p.Nombre AS PropNombre, p.Apellido AS PropApellido, p.Dni AS PropDni,

            u.Id AS UsuarioAltaId, u.Nombre AS AltaNombre, u.Apellido AS AltaApellido, u.Email AS AltaEmail,
            u2.Id AS UsuarioBajaId, u2.Nombre AS BajaNombre, u2.Apellido AS BajaApellido, u2.Email AS BajaEmail

        FROM contratos c
        INNER JOIN inquilinos i ON c.IdInquilino = i.IdInquilino
        INNER JOIN inmuebles m ON c.IdInmueble = m.Id
        INNER JOIN propietarios p ON m.PropietarioId = p.IdPropietario
        LEFT JOIN usuarios u ON c.UsuarioAltaId = u.Id
        LEFT JOIN usuarios u2 ON c.UsuarioBajaId = u2.Id
        WHERE c.Id = @id";

                using (var command = new MySqlCommand(sql, connection))
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
                            Activo = reader.GetBoolean(6),
                            FechaTerminacionAnticipada = reader.IsDBNull(7) ? null : reader.GetDateTime(7),
                            MontoMulta = reader.IsDBNull(8) ? null : reader.GetDecimal(8),
                            MultaPagada = Convert.ToBoolean(reader["MultaPagada"]),

                            UsuarioAltaId = reader.IsDBNull(10) ? null : reader.GetInt32(10),
                            UsuarioBajaId = reader.IsDBNull(11) ? null : reader.GetInt32(11),

                            Inquilino = new Inquilino
                            {
                                IdInquilino = reader.GetInt32(12),
                                Nombre = reader.GetString(13),
                                Apellido = reader.GetString(14),
                                Dni = reader.GetString(15)
                            },
                            Inmueble = new Inmueble
                            {
                                Id = reader.GetInt32(16),
                                Direccion = reader.GetString(17),
                                PropietarioId = reader.GetInt32(18),
                                Duenio = new Propietario
                                {
                                    IdPropietario = reader.GetInt32(19),
                                    Nombre = reader.GetString(20),
                                    Apellido = reader.GetString(21),
                                    Dni = reader.GetString(22)
                                }
                            },
                            UsuarioAlta = new Usuario
                            {
                                Id = reader.IsDBNull(23) ? 0 : reader.GetInt32(23),
                                Nombre = reader.IsDBNull(24) ? "" : reader.GetString(24),
                                Apellido = reader.IsDBNull(25) ? "" : reader.GetString(25),
                                Email = reader.IsDBNull(26) ? "" : reader.GetString(26)
                            },
                            UsuarioBaja = reader.IsDBNull(27) ? null : new Usuario
                            {
                                Id = reader.GetInt32(27),
                                Nombre = reader.IsDBNull(28) ? "" : reader.GetString(28),
                                Apellido = reader.IsDBNull(29) ? "" : reader.GetString(29),
                                Email = reader.IsDBNull(30) ? "" : reader.GetString(30)
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
        {List < Contrato > res = new List<Contrato>();
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

        public IList<Contrato> Activar(int id)
        {
            IList<Contrato> res = new List<Contrato>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = $"UPDATE contratos SET Activo = 1 WHERE Id = {id}";
                string sql2 = $"UPDATE pagos SET Anulado = 0 WHERE ContratoId = {id}";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                using (MySqlCommand command = new MySqlCommand(sql2, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public bool EstaOcupado(int inmuebleId, DateTime desde, DateTime hasta)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
            SELECT COUNT(*) FROM contratos 
            WHERE InmuebleId = @inmuebleId AND Estado = 1
            AND (
                (@desde BETWEEN FechaDesde AND FechaHasta) OR
                (@hasta BETWEEN FechaDesde AND FechaHasta) OR
                (FechaDesde BETWEEN @desde AND @hasta)
            )";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@inmuebleId", inmuebleId);
                    command.Parameters.AddWithValue("@desde", desde);
                    command.Parameters.AddWithValue("@hasta", hasta);

                    connection.Open();
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
            }
        }



        public int ModificarFinalizacionAnticipada(Contrato c)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE contratos SET 
            FechaTerminacionAnticipada=@fechaTerminacionAnticipada,
            MontoMulta=@montoMulta,
            MultaPagada=@multaPagada,
            UsuarioBajaId=@usuarioBajaId,
            Activo=0
            WHERE Id=@id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@fechaTerminacionAnticipada", c.FechaTerminacionAnticipada);
                    command.Parameters.AddWithValue("@montoMulta", c.MontoMulta ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@multaPagada", c.MultaPagada ? 1 : 0);
                    command.Parameters.AddWithValue("@usuarioBajaId", c.UsuarioBajaId ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@id", c.Id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }




        public IList<Contrato> ObtenerContratosVigentesPorInmueble(int inmuebleId)
        {
            var res = new List<Contrato>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT Id, FechaDesde, FechaHasta
                    FROM contratos
                    WHERE IdInmueble = @inmuebleId AND Activo = 1";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@inmuebleId", inmuebleId);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        res.Add(new Contrato
                        {
                            Id = reader.GetInt32("Id"),
                            FechaDesde = reader.GetDateTime("FechaDesde"),
                            FechaHasta = reader.GetDateTime("FechaHasta")
                        });
                    }
                }
            }
            return res;
        }


        public Contrato BuscarRenovacion(Contrato contrato)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
			SELECT * FROM contratos 
			WHERE Id <> @id 
			AND IdInquilino = @idInquilino 
			AND IdInmueble = @idInmueble 
			AND FechaDesde > @fechaHasta 
			ORDER BY FechaDesde ASC LIMIT 1";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", contrato.Id);
                    command.Parameters.AddWithValue("@idInquilino", contrato.IdInquilino);
                    command.Parameters.AddWithValue("@idInmueble", contrato.IdInmueble);
                    command.Parameters.AddWithValue("@fechaHasta", contrato.FechaHasta);

                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        return new Contrato
                        {
                            Id = reader.GetInt32("Id"),
                            Activo = reader.GetBoolean("Activo"),
                            // Podés mapear más si querés
                        };
                    }
                }
            }
            return null;
        }


    }
}