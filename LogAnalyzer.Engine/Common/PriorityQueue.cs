using LogAnalyzer.Models.Engine.Thread;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Engine.Common
{
    class PriorityQueue
    {
        // Private fields -----------------------------------------------------

        // 0 - beginning of the queue
        private readonly List<BaseThreadTask> queue;

        // Public methods -----------------------------------------------------

        public PriorityQueue()
        {
            queue = new List<BaseThreadTask>();
        }

        public void Enqueue(BaseThreadTask task)
        {
            int i = 0;
            while (i < queue.Count && queue[i].Priority >= task.Priority)
                i++;

            queue.Insert(i, task);
        }

        public BaseThreadTask Dequeue()
        {
            if (queue.Count == 0)
                return null;

            BaseThreadTask task = queue[0];
            queue.RemoveAt(0);

            return task;
        }

        public BaseThreadTask Peek()
        {
            if (queue.Count == 0)
                return null;

            return queue[0];
        }

        public void RemoveTaskType(Type type)
        {
            if (!typeof(BaseThreadTask).IsAssignableFrom(type))
                throw new ArgumentException("Invalid type!");

            int i = 0;
            while (i < queue.Count)
            {
                if (queue[i].GetType() == type)
                    queue.RemoveAt(i);
                else
                    i++;
            }
        }
    }
}
