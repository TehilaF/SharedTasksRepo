using Microsoft.AspNet.SignalR;
using SharedTasks.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SharedTasks
{
    public class TasksHub:Hub
    {
        public void NewTask(string title)
        {
            var repo = new TasksRepository(Properties.Settings.Default.ConStr);
            var task = new Task { Title = title, IsCompleted = false };
            repo.AddTask(task);
            SendTasks();
        }

        public void SendTasks()
        {
            var repo = new TasksRepository(Properties.Settings.Default.ConStr);
            var tasks = repo.GetActiveTasks();
            Clients.All.renderTasks(tasks.Select(t => new
            {
                Id = t.Id,
                Title = t.Title,
                HandledBy = t.HandledBy,
                UserDoingIt = t.User != null ? $"{t.User.FirstName} {t.User.LastName}":null,
            }));
        }

        public void GetAll()
        {
            SendTasks();
        }

        public void SetDoing(int id)
        {
            var userRepo = new UserRepository(Properties.Settings.Default.ConStr);
            var user = userRepo.GetByEmail(Context.User.Identity.Name);
            var taskRepo = new TasksRepository(Properties.Settings.Default.ConStr);
            taskRepo.SetDoing(user.Id, id);
            SendTasks();
        }

        public void SetDone(int id)
        {
            var repo = new TasksRepository(Properties.Settings.Default.ConStr);
            repo.SetCompleted(id);
            SendTasks();
        }
    }
}