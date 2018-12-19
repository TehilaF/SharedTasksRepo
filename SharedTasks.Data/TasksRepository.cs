using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTasks.Data
{
    public class TasksRepository
    {
        private string _connectionstring;
        public TasksRepository(string connectionString)
        {
            _connectionstring = connectionString;
        }

        public void AddTask(Task task)
        {
            using (var context = new SharedTasksDataContext(_connectionstring))
            {
                context.Tasks.InsertOnSubmit(task);
                context.SubmitChanges();
            }
        }

        public IEnumerable<Task> GetActiveTasks()
        {
            using (var context = new SharedTasksDataContext(_connectionstring))
            {
                var loadOptions = new DataLoadOptions();
                loadOptions.LoadWith<Task>(t => t.User);
                context.LoadOptions = loadOptions;
                return context.Tasks.Where(t => !t.IsCompleted).ToList();
            }
        }

        public void SetDoing(int userId, int taskId)
        {
            using (var context = new SharedTasksDataContext(_connectionstring))
            {
                context.ExecuteCommand("UPDATE Tasks SET HandledBy = {0} WHERE Id = {1}", userId, taskId);
            }
        }

        public void SetCompleted(int taskId)
        {
            using (var context = new SharedTasksDataContext(_connectionstring))
            {
                context.ExecuteCommand("UPDATE Tasks SET IsCompleted = 1 WHERE Id = {0}", taskId);
            }
        }
    }
}
