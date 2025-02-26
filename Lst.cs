using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.IO;

namespace TodoList
{
    public class Lst
    {
        public string Title { get; set; } = "My Todo List";  // Default title
        public List<Todo> Tasks { get; set; } = new();  // Auto-property initializer

        public void AddTask(Todo task) => Tasks.Add(task);

        public bool RemoveTask(int index)
        {
            if (index >= 0 && index < Tasks.Count)
            {
                Tasks.RemoveAt(index);
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Title: {Title}");
            sb.AppendLine("Tasks:");
            for (int i = 0; i < Tasks.Count; i++)
            {
                sb.AppendLine($"{i + 1}. {Tasks[i]}");
            }
            return sb.ToString();
        }
    }
}
