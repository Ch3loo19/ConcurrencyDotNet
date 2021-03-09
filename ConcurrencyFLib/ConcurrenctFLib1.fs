namespace Concurrency.F

open System
open System.Threading.Tasks
open System.Threading


module F_Library =

     // 'T -> M<'T>
    let Return value : Task<'T> = 
        Task.FromResult<'T> (value)
    
     // M<'T> * ('T -> M<'U>) -> M<'U>
     // The idea of the TaskCompletionSource is to create a task that can be governed and updated manually to indicate when and how a given operation completes. 
     // The power of the TaskCompletionSource type is in the capability of creating tasks that don’t tie up threads. http://bit.ly/2vDOmSN
    let Bind (input : Task<'T>) (binder : Func<'T , Task<'U>>) = 
        let tcs = new TaskCompletionSource<'U>() 
        input.ContinueWith(fun (task:Task<'T>) ->
               if (task.IsFaulted) then 
                    tcs.SetException(task.Exception.InnerExceptions)
               elif (task.IsCanceled) then 
                    tcs.SetCanceled()
               else
                    try
                       (binder.Invoke task.Result).ContinueWith(fun (nextTask:Task<'U>) -> tcs.SetResult(nextTask.Result)) |> ignore 
                    with
                    | ex -> tcs.SetException(ex)) |> ignore
        tcs.Task

    let Compose (f : Func<'A, Task<'B>>) (g : Func<'B,Task<'C>>)  = 
        new Func<'A,Task<'C>>(fun (n: 'A) -> Bind (f.Invoke n) g)


        
    let DoSharedVariableClosureCorrectly =
        let tasks = Array.zeroCreate<Task> 10    
        for index = 0 to 9 do
            tasks.[index] <- Task.Run(fun () -> printf "%i - %i%s" Thread.CurrentThread.ManagedThreadId index Environment.NewLine)
        Task.WhenAll tasks
            
  