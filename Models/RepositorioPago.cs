using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using MySqlConnector;
using System.Data;
namespace inmobiliariaDEramo.Models
{
    public class RepositorioPago : RepositorioBase
    {
        private readonly IConfiguration configuration;
        public RepositorioPago(IConfiguration configuration) : base(configuration)

        {

        }

        public int Alta(Pago entidad)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"INSERT INTO pagos 
            (NumeroPago, FechaPago, Importe, ContratoId, anulado) VALUES (@numeroPago, @fechaPago, @importe, @contratoId, @anulado);
            SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@numeroPago", entidad.NumeroPago);
                    command.Parameters.AddWithValue("@fechaPago", entidad.FechaPago);
                    command.Parameters.AddWithValue("@importe", entidad.Importe);
                    command.Parameters.AddWithValue("@contratoId", entidad.ContratoId);
                    command.Parameters.AddWithValue("@anulado", entidad.Anulado);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    entidad.Id = res;
                    connection.Close();
                }
            }
            return res;
        }



        /*public IEnumerable<Pago> ObtenerPorContrato(int contratoId)
        {

             SELECT * FROM Pagos WHERE ContratoId = @contratoId

        }*/

       /* public Pago ObtenerPorId(int id)
        {
            // SELECT * FROM Pagos WHERE Id = @id
        }*/

        public void Editar(Pago pago)
        {
            // UPDATE Pagos SET Detalle = @detalle WHERE Id = @id
        }

        public void Anular(int id)
        {
            // UPDATE Pagos SET Anulado = 1 WHERE Id = @id
        }
    }
}
