﻿using System;
using System.Collections.Generic;

namespace Domain.Business.Entities
{
    public class Employee : User
    {
        private readonly List<Task> _todo;

        public Employee(int id, string name, DateTime birthday, string username, string password) : base(id, name,
            birthday, username, password)
        {
            _todo = new List<Task>();
        }

        internal void AddTask(Task task)
        {
            _todo.Add(task);
        }

        internal void Load(List<Task> tasks)
        {
            _todo.AddRange(tasks);
        }
    }
}