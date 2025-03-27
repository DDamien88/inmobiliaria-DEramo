using System;
using System.Collections.Generic;
using System.Data;
using MySqlConnector;
using inmobiliariaDEramo.Models;

namespace inmobiliariaDEramo.Models
{
	public class RepositorioInquilinoMysql : RepositorioBase, IRepositorioInquilino
	{
		public RepositorioInquilinoMysql(IConfiguration configuration) : base(configuration)
		{
			//https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/
		}

		public int Alta(Inquilino p)
		{
			int res = -1;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = @"INSERT INTO inquilinos 
					(Nombre, Apellido, Dni, Telefono, Email) 
					VALUES (@nombre, @apellido, @dni, @telefono, @email);
					SELECT LAST_INSERT_ID();";//devuelve el id insertado (SCOPE_IDENTITY para sql)
				using (var command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@nombre", p.Nombre);
					command.Parameters.AddWithValue("@apellido", p.Apellido);
					command.Parameters.AddWithValue("@dni", p.Dni);
					command.Parameters.AddWithValue("@telefono", p.Telefono);
					command.Parameters.AddWithValue("@email", p.Email);
					connection.Open();
					res = Convert.ToInt32(command.ExecuteScalar());
					p.IdInquilino = res;
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
				string sql = @$"DELETE FROM inquilinos WHERE {nameof(Inquilino.IdInquilino)} = @id";
				using (var command = new MySqlCommand(sql, connection))
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
		public int Modificacion(Inquilino p)
		{
			int res = -1;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = @$"UPDATE inquilinos 
					SET Nombre=@nombre, Apellido=@apellido, Dni=@dni, Telefono=@telefono, Email=@email
					WHERE {nameof(Inquilino.IdInquilino)} = @id";
				using (var command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@nombre", p.Nombre);
					command.Parameters.AddWithValue("@apellido", p.Apellido);
					command.Parameters.AddWithValue("@dni", p.Dni);
					command.Parameters.AddWithValue("@telefono", p.Telefono);
					command.Parameters.AddWithValue("@email", p.Email);
					command.Parameters.AddWithValue("@id", p.IdInquilino);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}

		public IList<Inquilino> ObtenerTodos()
		{
			IList<Inquilino> res = new List<Inquilino>();
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = @"SELECT 
					IdInquilino, Nombre, Apellido, Dni, Telefono, Email
					FROM inquilinos";
				using (var command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Inquilino p = new Inquilino
						{
							IdInquilino = reader.GetInt32(nameof(Inquilino.IdInquilino)),
							Nombre = reader.GetString("Nombre"),
							Apellido = reader.GetString("Apellido"),
							Dni = reader.GetString("Dni"),
							Telefono = reader.GetString("Telefono"),
							Email = reader.GetString("Email"),
						};
						res.Add(p);
					}
					connection.Close();
				}
			}
			return res;
		}

		public IList<Inquilino> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
		{
			IList<Inquilino> res = new List<Inquilino>();
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = @$"
					SELECT IdInquilino, Nombre, Apellido, Dni, Telefono, Email
					FROM inquilinos
					LIMIT {tamPagina} OFFSET {(paginaNro - 1) * tamPagina}
				";
				using (var command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Inquilino p = new Inquilino
						{
							IdInquilino = reader.GetInt32(nameof(Inquilino.IdInquilino)),//más seguro
							Nombre = reader.GetString("Nombre"),
							Apellido = reader.GetString("Apellido"),
							Dni = reader.GetString("Dni"),
							Telefono = reader.GetString("Telefono"),
							Email = reader.GetString("Email"),
						};
						res.Add(p);
					}
					connection.Close();
				}
			}
			return res;
		}

		virtual public Inquilino ObtenerPorId(int id)
		{
			Inquilino? p = null;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = @"SELECT 
					IdInquilino, Nombre, Apellido, Dni, Telefono, Email 
					FROM inquilinos
					WHERE IdInquilino=@id";
				using (var command = new MySqlCommand(sql, connection))
				{
					command.Parameters.Add("@id", DbType.Int32).Value = id;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						p = new Inquilino
						{
							IdInquilino = reader.GetInt32(nameof(Inquilino.IdInquilino)),
							Nombre = reader.GetString("Nombre"),
							Apellido = reader.GetString("Apellido"),
							Dni = reader.GetString("Dni"),
							Telefono = reader.GetString("Telefono"),
							Email = reader.GetString("Email"),
						};
					}
					connection.Close();
				}
			}
			return p;
		}

		public Inquilino ObtenerPorEmail(string email)
		{
			Inquilino? p = null;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = @$"SELECT 
					{nameof(Inquilino.IdInquilino)}, Nombre, Apellido, Dni, Telefono, Email
					FROM inquilinos
					WHERE Email=@email";
				using (var command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.Add("@email", DbType.String).Value = email;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						p = new Inquilino
						{
							IdInquilino = reader.GetInt32(nameof(Inquilino.IdInquilino)),//más seguro
							Nombre = reader.GetString("Nombre"),
							Apellido = reader.GetString("Apellido"),
							Dni = reader.GetString("Dni"),
							Telefono = reader.GetString("Telefono"),
							Email = reader.GetString("Email"),
						};
					}
					connection.Close();
				}
			}
			return p;
		}

		public IList<Inquilino> BuscarPorNombre(string nombre)
		{
			List<Inquilino> res = new List<Inquilino>();
			Inquilino? p = null;
			nombre = "%" + nombre + "%";
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = @"SELECT
					IdInquilino, Nombre, Apellido, Dni, Telefono, Email
					FROM inquilinos
					WHERE Nombre LIKE @nombre OR Apellido LIKE @nombre";
				using (var command = new MySqlCommand(sql, connection))
				{
					command.Parameters.Add("@nombre", DbType.String).Value = nombre;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						p = new Inquilino
						{
							IdInquilino = reader.GetInt32(nameof(Inquilino.IdInquilino)),
							Nombre = reader.GetString("Nombre"),
							Apellido = reader.GetString("Apellido"),
							Dni = reader.GetString("Dni"),
							Telefono = reader.GetString("Telefono"),
							Email = reader.GetString("Email"),
						};
						res.Add(p);
					}
					connection.Close();
				}
			}
			return res;
		}
	}
}