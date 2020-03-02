﻿using System;
using System.Collections.Generic;
using Domain.Business.Entities;
using Domain.Business.Repositories;
using MySql.Data.MySqlClient;

namespace Persistence
{
    internal class TaskMapper
    {
        //Task cannot exist without Bug
        //For the rest all independent


        private readonly string _connectionString = "";

        internal TaskMapper(string connectionString)
        {
            _connectionString = connectionString;
        }

        internal List<Task> GetTaskFromDb()
        {
            var tasks = new List<Task>();

            var connection = new MySqlConnection(_connectionString);
            var command = new MySqlCommand("SELECT * from tbltask", connection);

            connection.Open();
            var dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                TimeSpan timeSpan;
                TimeSpan.TryParse(Convert.ToString(dataReader["timespent"]), out timeSpan);
                var task = new Task(
                    Convert.ToInt32(dataReader["id"]),
                    BugRepository.Items.Find(x => x.Id.Equals(Convert.ToInt32(dataReader["bug_id"]))),
                    Convert.ToString(dataReader["description"]),
                    Convert.ToInt32(dataReader["size"]),
                    timeSpan
                );
                tasks.Add(task);
            }

            connection.Close();
            return tasks;
        }

        internal void AddTaskToDb(Task task)
        {
            var connection = new MySqlConnection(_connectionString);
            var command = new MySqlCommand(
                "INSERT INTO tbltask (id, description, size, timespent, bug_id)" +
                " VALUES (@id, @description, @size, @timespent, @bugid)"
                , connection);


            command.Parameters.AddWithValue("id", task.Id);
            command.Parameters.AddWithValue("description", task.Description);
            command.Parameters.AddWithValue("size", task.Size);
            command.Parameters.AddWithValue("timespent", task.TimeSpent);
            command.Parameters.AddWithValue("bugid", task.Bug.Id);
            connection.Open();
            command.ExecuteNonQuery();

            connection.Close();
        }

        internal void UpdateTaskInDb(Task task)
        {
            var connection = new MySqlConnection(_connectionString);
            var command = new MySqlCommand(
                "UPDATE tbltask SET description = @description, size = @size, timespent = @timespent" +
                " WHERE id=@id"
                , connection);
            command.Parameters.AddWithValue("id", task.Id);
            command.Parameters.AddWithValue("description", task.Description);
            command.Parameters.AddWithValue("timespent", task.TimeSpent);
            command.Parameters.AddWithValue("size", task.Size);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }

        internal void DeleteTaskFromDb(Task task)
        {
            var connection = new MySqlConnection(_connectionString);
            var command = new MySqlCommand(
                "DELETE FROM tbltask" +
                " WHERE id=@id"
                , connection);
            command.Parameters.AddWithValue("id", task.Id);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }
    }
}