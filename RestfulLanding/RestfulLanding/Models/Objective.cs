using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace RestfulLanding.Models {
    public enum Priority { low, normal, high };
    public enum Urgency { low, normal, high  };
    public enum Status { inwork, none, done };
    public class Objective {
        [Key]
        public int Id { get; set; }
        public string userId { get; set; }
        public User user { get; set; }
        public Priority priority { get; set; }
        public Urgency urgency { get; set; }
        public Status status { get; set; }
        public string description { get; set; }
        public DateTime? due { get; set; } = default;

        public override string ToString() {
            return $"{description} {(due != DateTime.MaxValue ? $"(срок до {due?.ToString("g")})" : "")}";
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
