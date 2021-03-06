﻿using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Business.Entities;
using Domain.Business.Repositories;
using MySql.Data.MySqlClient;

namespace Domain.Persistence.Mappers
{
    internal class UserMapper
    {
        private readonly string _connectionString;

        internal UserMapper(string connectionString)
        {
            _connectionString = connectionString;
        }


        private List<Employee> GetEmployeesFromProjectManager(int pmId)
        {
            var employees = new List<Employee>();

            var connection = new MySqlConnection(_connectionString);
            var command = new MySqlCommand("SELECT * from tbluser WHERE projectmanager_id = @id", connection);

            command.Parameters.AddWithValue("@id", pmId);

            connection.Open();
            var dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                var id = Convert.ToInt32(dataReader["id"]);
                var name = Convert.ToString(dataReader["name"]);
                var birthday = Convert.ToDateTime(dataReader["birthday"]);

                var username = Convert.ToString(dataReader["username"]);
                var password = Convert.ToString(dataReader["password"]);

                var employee = new Employee(id, name, birthday, password, username);

                employees.Add(employee);
            }

            return employees;
        }

        private Projectmanager getProjectmanagerFromEmployee(User employee)
        {
            var users = UserRepository.Items.OfType<Projectmanager>().ToList();
            Projectmanager returnProjectmanager = null;
            foreach (var pm in users.Where(user => user.GetType() == typeof(Projectmanager))
                .Where(pm => pm.GetEmployees().Contains((Employee) employee)))
                returnProjectmanager = pm;
            return returnProjectmanager;
        }


        internal List<User> GetUsersFromDb()
        {
            var users = new List<User>();

            var connection = new MySqlConnection(_connectionString);
            var command = new MySqlCommand("SELECT * from tbluser", connection);

            connection.Open();
            var dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                var id = Convert.ToInt32(dataReader["id"]);
                var name = Convert.ToString(dataReader["name"]);
                var birthday = Convert.ToDateTime(dataReader["birthday"]);

                var username = Convert.ToString(dataReader["username"]);
                var password = Convert.ToString(dataReader["password"]);


                switch (Convert.ToString(dataReader["rol"]).ToLower())
                {
                    case "projectmanager":
                        var projectmanager = new Projectmanager(id, name, birthday, password, username);
                        projectmanager.Load(GetEmployeesFromProjectManager(projectmanager.Id));
                        users.Add(projectmanager);
                        break;
                    case "employee":
                        var employee = new Employee(id, name, birthday, password, username);
                        users.Add(employee);
                        break;
                    default:
                        var user = new User(id, name, birthday, password, username);
                        users.Add(user);
                        break;
                }
            }

            connection.Close();
            return users;
        }


        internal void AddUserToDb(User user)
        {
            var connection = new MySqlConnection(_connectionString);
            var command = new MySqlCommand(
                "INSERT INTO tbluser (id, name, birthday, rol, username, password)" +
                " VALUES (@id, @name, @birthday, @rol, @username, @password)"
                , connection);

            if (user.GetType() == typeof(Employee)) command.Parameters.AddWithValue("rol", "employee");
            else if (user.GetType() == typeof(Projectmanager)) command.Parameters.AddWithValue("rol", "projectmanager");
            else command.Parameters.AddWithValue("rol", DBNull.Value);

            command.Parameters.AddWithValue("id", user.Id);
            command.Parameters.AddWithValue("name", user.Name);
            command.Parameters.AddWithValue("birthday", user.Birthday);
            command.Parameters.AddWithValue("username", user.Username);
            command.Parameters.AddWithValue("password", user.Password);

            connection.Open();
            command.ExecuteNonQuery();

            connection.Close();
        }

        internal void UpdateUserInDb(User user)
        {
            var connection = new MySqlConnection(_connectionString);
            var command = new MySqlCommand(
                "UPDATE tbluser SET name = @name, birthday = @birthday, rol = @rol, username = @username, password = @password WHERE id=@id"
                , connection);
            command.Parameters.AddWithValue("name", user.Name);
            command.Parameters.AddWithValue("id", user.Id);
            command.Parameters.AddWithValue("birthday", user.Birthday);
            command.Parameters.AddWithValue("username", user.Username);
            command.Parameters.AddWithValue("password", user.Password);


            if (user.GetType() == typeof(Employee))
            {
                var index = command.CommandText.IndexOf("WHERE", StringComparison.Ordinal);
                command.CommandText = command.CommandText.Insert(index - 1, ", projectmanager_id = @projectmanager_id");
                //We do this because the variable is not always changed 


                command.Parameters.AddWithValue("rol", "employee");
                var pm = getProjectmanagerFromEmployee(user);
                if (pm != null) command.Parameters.AddWithValue("projectmanager_id", pm.Id);
                else command.Parameters.AddWithValue("projectmanager_id", DBNull.Value);
            }

            else if (user.GetType() == typeof(Projectmanager))
            {
                command.Parameters.AddWithValue("rol", "projectmanager");
            }
            else
            {
                command.Parameters.AddWithValue("rol", DBNull.Value);
            }

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }

        internal void DeleteUserInDb(User user)
        {
            var connection = new MySqlConnection(_connectionString);
            MySqlCommand command;

            //clean out
            if (user.GetType() == typeof(Projectmanager))
            {
                var pm = (Projectmanager) user;
                foreach (var werknemer in pm.GetEmployees())
                {
                    command = new MySqlCommand(
                        "UPDATE tbluser SET projectmanager_id = null " +
                        " WHERE id=@id"
                        , connection);
                    command.Parameters.AddWithValue("id", werknemer.Id);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                    command.Parameters.Clear();
                }
            }


            command = new MySqlCommand(
                "DELETE FROM tbluser" +
                " WHERE id=@id"
                , connection);
            command.Parameters.AddWithValue("id", user.Id);

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }
    }
}