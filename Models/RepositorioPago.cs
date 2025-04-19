using InmobiliariaDEramo.Models;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using MySqlConnector;
using System.Data;
namespace inmobiliariaDEramo.Models
{
    public class RepositorioPago : RepositorioBase, IRepositorioPago
    {
        private readonly IRepositorioUsuario repositorioUsuario;
        public RepositorioPago(IConfiguration configuration, IRepositorioUsuario repositorioUsuario) : base(configuration) { }


        public int Alta(Pago pago)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"INSERT INTO pagos (ContratoId, NumeroPago, FechaPago, Importe, Detalle, Anulado, UsuarioAltaId)
                    VALUES (@ContratoId, @NumeroPago, @FechaPago, @Importe, @Detalle, 0, @UsuarioAltaId);
                    SELECT LAST_INSERT_ID();";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@numeroPago", pago.NumeroPago);
                    command.Parameters.AddWithValue("@fechaPago", pago.FechaPago);
                    command.Parameters.AddWithValue("@importe", pago.Importe);
                    command.Parameters.AddWithValue("@detalle", pago.Detalle);
                    command.Parameters.AddWithValue("@contratoId", pago.ContratoId);
                    command.Parameters.AddWithValue("@usuarioAltaId", pago.UsuarioAltaId);

                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    pago.Id = res;
                    connection.Close();
                }
            }
            return res;
        }





        public IEnumerable<Pago> ObtenerPorContrato(int contratoId)
        {
            var res = new List<Pago>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
            SELECT p.Id, p.ContratoId, p.NumeroPago, p.FechaPago, p.Importe, p.Detalle, p.Anulado,
                c.FechaHasta
            FROM pagos p
            INNER JOIN contratos c ON p.ContratoId = c.Id
            WHERE p.ContratoId = @contratoId";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@contratoId", contratoId);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var pago = new Pago
                        {
                            Id = reader.GetInt32("Id"),
                            ContratoId = reader.GetInt32("ContratoId"),
                            NumeroPago = reader.GetInt32("NumeroPago"),
                            FechaPago = reader.GetDateTime("FechaPago"),
                            Importe = reader.GetDecimal("Importe"),
                            Detalle = reader.GetString("Detalle"),
                            Anulado = reader.GetBoolean("Anulado"),
                            Contrato = new Contrato
                            {
                                FechaHasta = reader.GetDateTime("FechaHasta")
                            }
                        };
                        res.Add(pago);
                    }
                    connection.Close();
                }
            }
            return res;
        }




        public Pago ObtenerPorId(int id)
        {
            Pago pago = null;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT 
                p.Id, p.ContratoId, p.NumeroPago, p.FechaPago, p.Importe, p.Detalle, p.Anulado,
                p.UsuarioAltaId, p.UsuarioBajaId,
                ua.Nombre AS NombreAlta, ua.Apellido AS ApellidoAlta, ua.Email AS EmailAlta,
                ub.Nombre AS NombreBaja, ub.Apellido AS ApellidoBaja, ub.Email AS EmailBaja
            FROM pagos p
            LEFT JOIN usuarios ua ON p.UsuarioAltaId = ua.Id
            LEFT JOIN usuarios ub ON p.UsuarioBajaId = ub.Id
            WHERE p.Id = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            pago = new Pago
                            {
                                Id = reader.GetInt32(0),
                                ContratoId = reader.GetInt32(1),
                                NumeroPago = reader.GetInt32(2),
                                FechaPago = reader.GetDateTime(3),
                                Importe = reader.GetDecimal(4),
                                Detalle = reader.GetString(5),
                                Anulado = reader.GetBoolean(6),
                                UsuarioAltaId = reader.IsDBNull(7) ? null : reader.GetInt32(7),
                                UsuarioBajaId = reader.IsDBNull(8) ? null : reader.GetInt32(8),
                                UsuarioAlta = reader.IsDBNull(7) ? null : new Usuario
                                {
                                    Nombre = reader["NombreAlta"].ToString(),
                                    Apellido = reader["ApellidoAlta"].ToString(),
                                    Email = reader["EmailAlta"].ToString()
                                },
                                UsuarioBaja = reader.IsDBNull(8) ? null : new Usuario
                                {
                                    Nombre = reader["NombreBaja"].ToString(),
                                    Apellido = reader["ApellidoBaja"].ToString(),
                                    Email = reader["EmailBaja"].ToString()
                                }
                            };
                        }
                    }

                    connection.Close();
                }
            }

            return pago;
        }




        /*public void Editar(Pago pago)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = "UPDATE Pagos SET Detalle = @detalle WHERE Id = @id;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@detalle", pago.Detalle);
                    command.Parameters.AddWithValue("@id", pago.Id);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }*/

        public int Anular(Pago pago)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE pagos 
                        SET Anulado = 1, UsuarioBajaId = @usuarioBajaId 
                        WHERE Id = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@usuarioBajaId", pago.UsuarioBajaId);
                    command.Parameters.AddWithValue("@id", pago.Id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }




        public void EditarDetalle(Pago pago)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "UPDATE pagos SET Detalle = @detalle WHERE Id = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@detalle", pago.Detalle);
                    command.Parameters.AddWithValue("@id", pago.Id);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }



        public int ObtenerUltimoNumeroPago(int contratoId)
        {
            int res = 0;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT MAX(NumeroPago) FROM pagos WHERE ContratoId = @contratoId AND Anulado = 0";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@contratoId", contratoId);
                    connection.Open();
                    var result = command.ExecuteScalar();
                    if (result != DBNull.Value && result != null)
                    {
                        res = Convert.ToInt32(result);
                    }
                    connection.Close();
                }
            }
            return res;
        }




        public int CalcularMesesAdeudados(int contratoId)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = "SELECT COUNT(*) FROM pagos WHERE ContratoId = @contratoId AND Anulado = 0;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@contratoId", contratoId);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    connection.Close();
                }
            }
            return res;
        }



    }
}


