using LiteDB;

namespace Devon4Net.Application.WebAPI.Implementation.Domain.Entities
{
    public partial class Session
    {
        public long Id { get; set; }

        public string InviteToken { get; set; }

        public DateTime ExpiresAt { get; set; }

        public IList<Task> Tasks { get; set; }

        [BsonRef("User")]
        public IList<User> Users { get; set; }

        public bool IsValid()
        {
            return ExpiresAt > DateTime.Now;
        }

        /* changes every open task to suspended and every evaluated task to closed
           suspended tasks are untouched 
        */
        private List<(String, Status)> SuspendPendingTasksOtherThan(string ignoredTask)
        {
            List<(String, Status)> modifiedTasks = new List<(string, Status)>();

            foreach (var task in Tasks.Where(item => (item.Status.HasFlag(Status.Open) || item.Status.HasFlag(Status.Evaluated)) && item.Id != ignoredTask))
            {
                switch (task.Status)
                {
                    case Status.Open:
                        {
                            task.Status = Status.Suspended;

                            modifiedTasks.Add((task.Id, task.Status));

                            break;
                        }
                    case Status.Evaluated:
                        {
                            task.Status = Status.Ended;

                            modifiedTasks.Add((task.Id, task.Status));

                            break;
                        }
                    default: break;
                }
            }

            return modifiedTasks;
        }

        public (bool, List<(String, Status)>) ChangeStatusOfTask(string taskId, Status status)
        {
            List<(String, Status)> modifiedTasks = new List<(string, Status)>();

            var task = Tasks.FirstOrDefault(item => item.Id == taskId);

            // if the status is already the same status
            if (task.Status == status)
            {
                return (false, modifiedTasks);
            }

            // react upon the change
            switch (status)
            {
                case Status.Open:
                    {
                        modifiedTasks = SuspendPendingTasksOtherThan(taskId);
                        break;
                    }
                case Status.Suspended: break;
                case Status.Ended:
                    {
                        if (task.Status != Status.Evaluated)
                        {
                            return (false, modifiedTasks);
                        }
                        break;
                    }
                case Status.Evaluated:
                    {
                        // task must have been previously open for votes
                        if (task.Status == Status.Open)
                        {
                            modifiedTasks = SuspendPendingTasksOtherThan(taskId);
                        }
                        else
                        {
                            return (false, modifiedTasks);
                        }

                        break;
                    }
                default: throw new Exception("This should have been unreachable!");
            }

            task.Status = status;

            // add the changed task itself to the modified list
            modifiedTasks.Add((task.Id, task.Status));

            return (true, modifiedTasks);
        }

        public bool isPrivilegedUser(string userId)
        {
            if (!this.Users.Where(item => item.Id.Equals(userId)).Any())
            {
                return false;
            }

            if (this.Users.First().Id.Equals(userId))
            {
                return true;
            }

            return false;
        }
    }
}