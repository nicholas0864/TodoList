namespace TodoList
{
    public class Todo
    {
        public required string Title { get; set; }
        public int Diff { get; set; }
        public bool Status { get; set; }

        public override string ToString()
        {
            return $"[{(Status ? "✔" : "✘")}] {Title} (Difficulty: {Diff})";
        }
    }
}
