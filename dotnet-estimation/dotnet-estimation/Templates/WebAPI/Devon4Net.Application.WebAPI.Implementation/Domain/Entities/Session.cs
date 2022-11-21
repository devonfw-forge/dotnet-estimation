namespace Devon4Net.Application.WebAPI.Implementation.Domain.Entities
{
    public partial class Session
    {
        public long Id { get; set; }

        public string InviteToken { get; set; }

        public DateTime ExpiresAt { get; set; }

        public IList<Task> Tasks { get; set; }

        public IList<User> Users { get; set; }

        public bool IsValid()
        {
            return ExpiresAt > DateTime.Now;
        }

        /* changes every open task to suspended and every evaluated task to closed
           suspended tasks are untouched 
        */
        private List<(String, Status)> SuspendPendingTask()
        {
            List<(String, Status)> modifiedTasks = new List<(string, Status)>();

            foreach (var task in Tasks.Where(item => item.Status.HasFlag(Status.Open) || item.Status.HasFlag(Status.Evaluated)))
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

            foreach (var item in Tasks)
            {
                if (item.Id == taskId)
                {
                    if (item.Status == status)
                    {
                        // if the status is already the same status
                        return (false, modifiedTasks);
                    }

                    switch (status)
                    {
                        case Status.Open:
                            {
                                modifiedTasks = SuspendPendingTask();

                                break;
                            }
                        case Status.Evaluated:
                            {
                                // task must have been previously voted for
                                if (item.Status == Status.Open)
                                {
                                    modifiedTasks = SuspendPendingTask();
                                } else
                                {
                                    return (false, modifiedTasks);
                                }

                                break;
                            }
                        case Status.Suspended:
                        case Status.Ended:
                            {
                                break;
                            }
                        default: throw new Exception("This should have been unreachable!");
                    }

                    item.Status = status;

                    modifiedTasks.Add((item.Id, item.Status));

                    return (true, modifiedTasks);
                }
            }

            return (false, modifiedTasks);
        }
    }
}