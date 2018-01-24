using LogAnalyzer.Engine.Thread.Results;
using LogAnalyzer.Engine.Thread.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Engine
{
    class ThreadRunResultEventArgs 
    {
        public ThreadRunResultEventArgs(List<BaseThreadResult> result, bool cancelled)
        {
            Result = result;
            Cancelled = cancelled;
        }

        public List<BaseThreadResult> Result { get; }
        public bool Cancelled { get; }
    }

    delegate void ThreadFinishedDelegate(object sender, ThreadRunResultEventArgs args);

    class EngineWorker : IDisposable
    {
        private BackgroundWorker backgroundWorker;
        private bool workerRunning = false;

        private List<BaseThreadResult> DoParseLines(ParseLinesThreadTask parseLines)
        {
            // TODO
            return new List<BaseThreadResult>();
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument is ParseLinesThreadTask parseLines)
            {
                e.Result = DoParseLines(parseLines);
            }
            else
            {
                throw new ArgumentException("Invalid task!");
            }
        }

        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Ran on UI thread
            try
            {
                if (e.Cancelled)
                {
                    ThreadFinished?.Invoke(this, new ThreadRunResultEventArgs(null, true));
                }
                else
                {
                    if (e.Result is List<BaseThreadResult> results)
                        ThreadFinished?.Invoke(this, new ThreadRunResultEventArgs(results, false));
                    else
                        throw new Exception("Thread finished without expected result!");
                }
            }
            finally
            {
                workerRunning = false;
            }
        }

        public EngineWorker()
        {
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += DoWork;
            backgroundWorker.RunWorkerCompleted += RunWorkerCompleted;
        }

        public void RunTask(BaseThreadTask task)
        {
            if (workerRunning)
                throw new InvalidAsynchronousStateException("Worker is already running!");

            backgroundWorker.RunWorkerAsync(task);
            workerRunning = true;
        }

        public event ThreadFinishedDelegate ThreadFinished;
        public bool WorkerRunning => workerRunning;
    }
}
