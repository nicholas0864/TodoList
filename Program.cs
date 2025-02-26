using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace TodoList
{
    class Program
    {
        // Path to store JSON in project directory
        private static readonly string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "todoList.json");
        private static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions { WriteIndented = true };

        public static void Main(string[] args)
        {
            // Load the todo list from file
            Lst todoList = LoadListFromFile();

            bool running = true;
            while (running)
            {
                Console.Clear();  // Clears the screen for a cleaner look
                Console.WriteLine("===== TODO LIST =====");
                Console.WriteLine(todoList.ToString());
                Console.WriteLine("\n1. Add Task\n2. Remove Task\n3. Exit");
                Console.Write("Choose an option: ");

                string? input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        Console.Clear();
                        Console.Write("Enter task title: ");
                string title = Console.ReadLine() ?? "";

                if (!string.IsNullOrWhiteSpace(title))
                {
                    Console.Write("Enter task difficulty (1-5): ");
                    string? diffInput = Console.ReadLine();

                    // Try parsing the input
                    if (int.TryParse(diffInput, out int diff) && diff >= 1 && diff <= 5)
                    {
                        todoList.AddTask(new Todo { Title = title, Diff = diff, Status = false });
                        SaveListToFile(todoList);
                        break; // Valid input, exit loop
                    }

                    Console.WriteLine("Invalid input. Please enter a number between 1 and 5.");
                }
                else
                {
                    Console.WriteLine("Title cannot be empty. Please enter a valid title.");
                }
                break;
                case "2":
                    Console.Clear();
                    Console.WriteLine(todoList.ToString());
                    Console.Write("Enter task number to remove: ");
                    if (int.TryParse(Console.ReadLine(), out int taskNum))
                    {
                        todoList.RemoveTask(taskNum - 1);
                        SaveListToFile(todoList);
                    }
                    break;
                case "3":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Press Enter to try again...");
                    Console.ReadLine();  // Waits for input before refreshing
                    break;
                }
            }
        }

        static void ViewList(Lst todoList)
        {
            Console.Clear();
            Console.WriteLine(todoList.ToString());
            static void AddTask(Lst todoList)
            {
                Console.Clear();
                Console.Write("Enter task title: ");
                string? title = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(title))
                {
                    Console.WriteLine("Task title cannot be empty. Press Enter to continue...");
                    Console.ReadLine();
                    return;
                }

                Console.Write("Enter difficulty (1-5): ");
                if (!int.TryParse(Console.ReadLine(), out int diff) || diff < 1 || diff > 5)
                {
                    Console.WriteLine("Invalid difficulty. Press Enter to continue...");
                    Console.ReadLine();
                    return;
                }

                Todo newTask = new Todo { Title = title, Diff = diff, Status = false };
                todoList.Tasks.Add(newTask);

                Console.WriteLine("Task added! Press Enter to continue...");
                Console.ReadLine();
            }

            static void RemoveTask(Lst todoList)
            {
                Console.Clear();
                Console.WriteLine("Choose a task to remove:");
                for (int i = 0; i < todoList.Tasks.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {todoList.Tasks[i]}");
                }

                Console.Write("Enter task number: ");
                if (!int.TryParse(Console.ReadLine(), out int taskNum) || taskNum < 1 || taskNum > todoList.Tasks.Count)
                {
                    Console.WriteLine("Invalid task number. Press Enter to continue...");
                    Console.ReadLine();
                    return;
                }

                todoList.Tasks.RemoveAt(taskNum - 1);
                Console.WriteLine("Task removed. Press Enter to continue...");
                Console.ReadLine();
            }
        }

        public static Lst LoadListFromFile()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    return JsonSerializer.Deserialize<Lst>(json) ?? new Lst();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading todo list: {ex.Message}");
            }
            return new Lst(); // Return empty list if there's an issue
        }

        public static void SaveListToFile(Lst todoList)
        {
            try
            {
                string json = JsonSerializer.Serialize(todoList, jsonOptions);
                File.WriteAllText(filePath, json);
                Console.WriteLine("Todo list saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving todo list: {ex.Message}");
            }
        }
    }
}
