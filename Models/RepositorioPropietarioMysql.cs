﻿using System;
using System.Collections.Generic;
using System.Data;
using MySqlConnector;
using inmobiliariaDEramo.Models;

namespace inmobiliariaDEramo.Models
{
	public class RepositorioPropietarioMySql : RepositorioBase, IRepositorioPropietario
	{
		public RepositorioPropietarioMySql(IConfiguration configuration) : base(configuration)
		{
			//https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/
		}

		public int Alta(Propietario p)
		{
			int res = -1;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = @"INSERT INTO propietarios 
					(Nombre, Apellido, Dni, Telefono, Email, Clave, Activo) 
					VALUES (@nombre, @apellido, @dni, @telefono, @email, @clave, 1);
					SELECT LAST_INSERT_ID();";
				using (var command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@nombre", p.Nombre);
					command.Parameters.AddWithValue("@apellido", p.Apellido);
					command.Parameters.AddWithValue("@dni", p.Dni);
					command.Parameters.AddWithValue("@telefono", p.Telefono);
					command.Parameters.AddWithValue("@email", p.Email);
					command.Parameters.AddWithValue("@clave", p.Clave);
					command.Parameters.AddWithValue("@activo", p.Activo);
					connection.Open();
					res = Convert.ToInt32(command.ExecuteScalar());
					p.IdPropietario = res;
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
				string sql = @$"Update propietarios SET Activo=0 WHERE {nameof(Propietario.IdPropietario)} = @id";
				string sql2 = @$"Update inmuebles SET Activo=0 WHERE PropietarioId = @id";
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



		public int Modificacion(Propietario p)
		{
			int res = -1;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = @$"UPDATE Propietarios 
					SET Nombre=@nombre, Apellido=@apellido, Dni=@dni, Telefono=@telefono, Email=@email, Clave=@clave 
					WHERE {nameof(Propietario.IdPropietario)} = @id";
				using (var command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@nombre", p.Nombre);
					command.Parameters.AddWithValue("@apellido", p.Apellido);
					command.Parameters.AddWithValue("@dni", p.Dni);
					command.Parameters.AddWithValue("@telefono", p.Telefono);
					command.Parameters.AddWithValue("@email", p.Email);
					command.Parameters.AddWithValue("@clave", p.Clave);
					command.Parameters.AddWithValue("@id", p.IdPropietario);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}

		public IList<Propietario> ObtenerTodos()
		{
			IList<Propietario> res = new List<Propietario>();
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = @"SELECT 
					IdPropietario, Nombre, Apellido, Dni, Telefono, Email, Clave, Activo
					FROM propietarios";
				using (var command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Propietario p = new Propietario
						{
							IdPropietario = reader.GetInt32(nameof(Propietario.IdPropietario)),
							Nombre = reader.GetString("Nombre"),
							Apellido = reader.GetString("Apellido"),
							Dni = reader.GetString("Dni"),
							Telefono = reader.GetString("Telefono"),
							Email = reader.GetString("Email"),
							Clave = reader.GetString("Clave"),
							Activo = reader.GetBoolean("Activo"),
						};
						res.Add(p);
					}
					connection.Close();
				}
			}
			return res;
		}



		public IList<Propietario> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
		{
			IList<Propietario> res = new List<Propietario>();
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = @$"
					SELECT IdPropietario, Nombre, Apellido, Dni, Telefono, Email, Clave, Activo
					FROM propietarios
					LIMIT {tamPagina} OFFSET {(paginaNro - 1) * tamPagina}
				";
				using (var command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Propietario p = new Propietario
						{
							IdPropietario = reader.GetInt32(nameof(Propietario.IdPropietario)),
							Nombre = reader.GetString("Nombre"),
							Apellido = reader.GetString("Apellido"),
							Dni = reader.GetString("Dni"),
							Telefono = reader.GetString("Telefono"),
							Email = reader.GetString("Email"),
							Clave = reader.GetString("Clave"),
							Activo = reader.GetBoolean("Activo"),
						};
						res.Add(p);
					}
					connection.Close();
				}
			}
			return res;
		}

		public Propietario ObtenerPorId(int id)
		{
			Propietario? p = null;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = @"SELECT 
					IdPropietario, Nombre, Apellido, Dni, Telefono, Email, Clave, Activo
					FROM propietarios
					WHERE IdPropietario=@id";
				using (var command = new MySqlCommand(sql, connection))
				{
					command.Parameters.Add("@id", DbType.Int32).Value = id;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						p = new Propietario
						{
							IdPropietario = reader.GetInt32(nameof(Propietario.IdPropietario)),
							Nombre = reader.GetString("Nombre"),
							Apellido = reader.GetString("Apellido"),
							Dni = reader.GetString("Dni"),
							Telefono = reader.GetString("Telefono"),
							Email = reader.GetString("Email"),
							Clave = reader.GetString("Clave"),
							Activo = reader.GetBoolean("Activo"),
						};
					}
					connection.Close();
				}
			}
			return p;
		}

		public Propietario ObtenerPorEmail(string email)
		{
			Propietario? p = null;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = @$"SELECT 
					{nameof(Propietario.IdPropietario)}, Nombre, Apellido, Dni, Telefono, Email, Clave 
					FROM propietarios
					WHERE Email=@email";
				using (var command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.Add("@email", DbType.String).Value = email;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						p = new Propietario
						{
							IdPropietario = reader.GetInt32(nameof(Propietario.IdPropietario)),//más seguro
							Nombre = reader.GetString("Nombre"),
							Apellido = reader.GetString("Apellido"),
							Dni = reader.GetString("Dni"),
							Telefono = reader.GetString("Telefono"),
							Email = reader.GetString("Email"),
							Clave = reader.GetString("Clave"),
						};
					}
					connection.Close();
				}
			}
			return p;
		}

		public IList<Propietario> BuscarPorNombre(string nombre)
		{
			List<Propietario> res = new List<Propietario>();
			Propietario? p = null;
			nombre = "%" + nombre + "%";
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = @"SELECT
					IdPropietario, Nombre, Apellido, Dni, Telefono, Email, Clave 
					FROM propietarios
					WHERE Nombre LIKE @nombre OR Apellido LIKE @nombre";
				using (var command = new MySqlCommand(sql, connection))
				{
					command.Parameters.Add("@nombre", DbType.String).Value = nombre;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						p = new Propietario
						{
							IdPropietario = reader.GetInt32(nameof(Propietario.IdPropietario)),
							Nombre = reader.GetString("Nombre"),
							Apellido = reader.GetString("Apellido"),
							Dni = reader.GetString("Dni"),
							Telefono = reader.GetString("Telefono"),
							Email = reader.GetString("Email"),
							Clave = reader.GetString("Clave"),
						};
						res.Add(p);
					}
					connection.Close();
				}
			}
			return res;
		}

		public IList<Propietario> Activar(int idPropietario)
		{
			IList<Propietario> res = new List<Propietario>();
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = @"UPDATE propietarios SET Activo=1 WHERE IdPropietario=@idPropietario";
				using (var command = new MySqlCommand(sql, connection))
				{
					command.Parameters.Add("@idPropietario", DbType.Int32).Value = idPropietario;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					connection.Close();
				}
			}
			return res;
		}


	}

}