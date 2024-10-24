namespace RestfulLanding.Models {
    public enum Priority { low, normal, high };
    public enum Urgency { low, normal, high  };
    public class Objective {
        public Priority priority;
        public Urgency urgency;
        public string title;
        public DateTime due = default;

        public Objective(Priority priority, Urgency urgency, string title, DateTime due = default) {
            this.priority = priority;
            this.urgency = urgency;
            this.title = title;
            this.due = due;
        }

        public override string ToString() {
            return $"{title} {(due != default ? $"(срок до {due.ToString("g")})" : "")}";
        }
    }

    public class ObjectivePriorityComparer : IComparer<Objective> {
        public int Compare(Objective a, Objective b) {
            return a.priority.CompareTo(b.priority);
        }
    }

    public class ObjectiveUrgencyComparer : IComparer<Objective> {
        public int Compare(Objective a, Objective b) {
            return a.urgency.CompareTo(b.urgency);
        }
    }
}
