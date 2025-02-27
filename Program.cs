using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text;

namespace TodoList
{
    class Program
    {
        private static readonly JsonSerializerOptions jsonOptions = new() { WriteIndented = true };

        public static async Task Main()
        {
            Dictionary<string, Lst> todoLists = new()
            {
                { "Work", new Lst("workList.json") },
                { "Personal", new Lst("personalList.json") }
            };

            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== TODO LISTS =====");
                foreach (var list in todoLists)
                {
                    Console.WriteLine($"{list.Key}: {list.Value.TaskCount()} tasks");
                }
                Console.WriteLine("\nSelect a list to view or press Q to quit:");
                string? selectedList = Console.ReadLine()?.Trim().ToLower();
                if (selectedList == "q") break;

                if (selectedList != null)
                {
                    selectedList = char.ToUpper(selectedList[0]) + selectedList.Substring(1);
                    if (todoLists.TryGetValue(selectedList, out var selectedTodoList))
                    {
                        await MainLoop(selectedTodoList);
                    }
                    else
                    {
                        Console.WriteLine("Invalid selection. Press Enter to try again...");
                        Console.ReadLine();
                    }
                }

            }

            static async Task MainLoop(Lst list)
            {
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine($"===== {list.ListName} TODO LIST =====");
                    Console.WriteLine(list);
                    Console.WriteLine("\n1. Add Task\n2. Remove Task\n3. Mark Task as Completed\n4. Edit Task\n5. Exit");
                    Console.Write("Choose an option: ");

                    switch (Console.ReadLine()?.Trim())
                    {
                        case "1":
                            await list.AddTaskFromInput();
                            break;
                        case "2":
                            await list.RemoveTaskFromInput();
                            break;
                        case "3":
                            await list.MarkTaskAsCompleted();
                            break;
                        case "4":
                            await list.EditTask();
                            break;
                        case "5":
                            return;
                        default:
                            Console.WriteLine("Invalid choice. Press Enter to try again...");
                            Console.ReadLine();
                            break;
                    }
                }
            }
        }

        class Todo
        {
            public required string Title { get; set; }
            public int Diff { get; set; }
            public bool Status { get; set; }

            public override string ToString()
            {
                return $"[{(Status ? "✔" : "✘")}] {Title} (Difficulty: {Diff})";
            }
        }

        class Lst
        {
            public string ListName { get; set; }
            public List<Todo> Tasks { get; set; } = new();
            private string filePath;
            private JsonSerializerOptions jsonOptions = new() { WriteIndented = true };

            public Lst(string path)
            {
                filePath = path;
                ListName = Path.GetFileNameWithoutExtension(path);
                LoadListFromFile();
            }

            public int TaskCount() => Tasks.Count;

            public void LoadListFromFile()
            {
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    Tasks = JsonSerializer.Deserialize<List<Todo>>(json) ?? new();
                }
            }

            public async Task SaveListToFile()
            {
                string json = JsonSerializer.Serialize(Tasks, jsonOptions);
                await File.WriteAllTextAsync(filePath, json);
            }

            public async Task AddTaskFromInput()
            {
                Console.Write("Enter task title: ");
                string title = Console.ReadLine()?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(title)) return;

                Console.Write("Enter task difficulty (1-5): ");
                if (int.TryParse(Console.ReadLine(), out int diff) && diff >= 1 && diff <= 5)
                {
                    Tasks.Add(new Todo { Title = title, Diff = diff, Status = false });
                    await SaveListToFile();
                }
            }

            public async Task RemoveTaskFromInput()
            {
                Console.WriteLine("Choose a task to remove:");
                DisplayTasks();
                if (int.TryParse(Console.ReadLine(), out int taskNum) && taskNum > 0 && taskNum <= Tasks.Count)
                {
                    Tasks.RemoveAt(taskNum - 1);
                    await SaveListToFile();
                }
            }

            public async Task MarkTaskAsCompleted()
            {
                Console.WriteLine("Choose a task to mark as completed:");
                DisplayTasks();
                if (int.TryParse(Console.ReadLine(), out int taskNum) && taskNum > 0 && taskNum <= Tasks.Count)
                {
                    Tasks[taskNum - 1].Status = true;
                    await SaveListToFile();
                }
            }

            public async Task EditTask()
            {
                Console.WriteLine("Choose a task to edit:");
                DisplayTasks();
                if (int.TryParse(Console.ReadLine(), out int taskNum) && taskNum > 0 && taskNum <= Tasks.Count)
                {
                    Console.Write("Enter new task title: ");
                    string newTitle = Console.ReadLine()?.Trim() ?? Tasks[taskNum - 1].Title;

                    Console.Write("Enter new difficulty (1-5): ");
                    if (int.TryParse(Console.ReadLine(), out int newDiff) && newDiff >= 1 && newDiff <= 5)
                    {
                        Tasks[taskNum - 1].Title = newTitle;
                        Tasks[taskNum - 1].Diff = newDiff;
                        await SaveListToFile();
                    }
                }
            }

            private void DisplayTasks()
            {
                for (int i = 0; i < Tasks.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {Tasks[i]}");
                }
            }

            public override string ToString()
            {
                if (Tasks.Count == 0) return "No tasks available.";
                var taskList = new StringBuilder();
                for (int i = 0; i < Tasks.Count; i++)
                {
                    taskList.AppendLine($"{i + 1}. {Tasks[i]}");
                }
                return taskList.ToString();
            }
        }
    }
}