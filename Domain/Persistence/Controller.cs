﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using Domain.Business.Entities;
using Domain.Persistence.Mappers;
using Persistence;

namespace Domain.Persistence
{
    internal static class Controller
    {
        private static string _connectionString = "";

        private static string ConnectionString
        {
            get
            {
                if (_connectionString == "")
                {
                    if (ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location)
                            .ConnectionStrings.ConnectionStrings.Count ==
                        0) throw new Exception("Connection string not configured");

                    _connectionString = ConfigurationManager.ConnectionStrings["Database"].ConnectionString;
                }

                return _connectionString;
            }
        }

        #region Bug

        internal static List<Bug> GetBugs()
        {
            var bugMapper = new BugMapper(ConnectionString);
            return bugMapper.GetBugsFromDb();
        }

        internal static void AddBug(Bug bug)
        {
            var bugMapper = new BugMapper(ConnectionString);
            bugMapper.AddBugToDb(bug);
        }

        internal static void RemoveBug(Bug bug)
        {
            var bugMapper = new BugMapper(ConnectionString);
            bugMapper.DeleteBugInDb(bug);
        }

        internal static void UpdateBug(Bug bug)
        {
            var bugMapper = new BugMapper(ConnectionString);
            bugMapper.UpdateBugInDb(bug);
        }

        #endregion

        #region Task

        internal static List<Task> GetTasks()
        {
            var taskMapper = new TaskMapper(ConnectionString);
            return taskMapper.GetTaskFromDb();
        }

        internal static void AddTask(Task task)
        {
            var taskMapper = new TaskMapper(ConnectionString);
            taskMapper.AddTaskToDb(task);
        }

        internal static void RemoveTask(Task task)
        {
            var taskMapper = new TaskMapper(ConnectionString);
            taskMapper.DeleteTaskFromDb(task);
        }

        internal static void UpdateTask(Task task)
        {
            var taskMapper = new TaskMapper(ConnectionString);
            taskMapper.UpdateTaskInDb(task);
        }

        #endregion

        #region User

        internal static List<User> GetUsers()
        {
            var userMapper = new UserMapper(ConnectionString);
            return userMapper.GetUsersFromDb();
        }

        internal static void AddUser(User user)
        {
            var userMapper = new UserMapper(ConnectionString);
            userMapper.AddUserToDb(user);
        }

        internal static void UpdateUser(User user)
        {
            var userMapper = new UserMapper(ConnectionString);
            userMapper.UpdateUserInDb(user);
        }

        internal static void DeleteUser(User user)
        {
            var userMapper = new UserMapper(ConnectionString);
            userMapper.DeleteUserInDb(user);
        }

        #endregion
    }
}