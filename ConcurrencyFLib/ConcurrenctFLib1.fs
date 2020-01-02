namespace Concurrency.F

open System
open System.Collections.Generic
open System.Threading.Tasks
open FSharp.Collections.ParallelSeq
open System.Threading
open System.Linq

type Person (firstName: string, lastName: string) =
    member this.FirstName = firstName
    member this.LastName = lastName
    member this.FullName = String.concat (",") <| [firstName; lastName]

type FList<'a> =
    | Empty   
    | Cons of head:'a * tail:FList<'a>   //A change



type AsyncRetryBuilder(max, sleepMilliseconds : int) =
    let rec retry n (task:Async<'a>) (continuation:'a -> Async<'b>) = 
        async { 
            try 
                let! result = task   
                let! conResult = continuation result   
                return conResult
            with error ->
                if n = 0 then return raise error   
                else
                    do! Async.Sleep sleepMilliseconds   
                    return! retry (n - 1) task continuation }
   
   
    member x.ReturnFrom(f) = f   

    member x.Return(v) = async { return v } 


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
            
    let LazyF =
        let kibsterFunction = lazy (Person( "Thomas", "McKirbster"))
        kibsterFunction.Force().FullName

    let rec map f (list:FList<'a>) =    
        match list with
        | Empty -> Empty
        | Cons(hd,tl) -> Cons(f hd, map f tl)
    
    let rec filter p (list:FList<'a>) =
        match list with
        | Empty -> Empty
        | Cons(hd,tl) when p hd = true -> Cons(hd, filter p tl)
        | Cons(hd,tl) -> filter p tl



    // fun and games

    type Suit = 
        | Hearts 
        | Clubs 
        | Diamonds 
        | Spades

    type Rank = 
        | Value of int
        | Ace
        | King
        | Queen
        | Jack
        static member GetAllRanks() = [ 
            yield Ace
            for i in 2 .. 10 do yield Value i
            yield Jack
            yield Queen
            yield King ]

    type SuitRankCombo = {TheSuit: Suit; TheRank:Rank}

    let fullDeck = 
        [ for suit in [ Hearts; Diamonds; Clubs; Spades] do
              for rank in Rank.GetAllRanks() do 
                  yield { TheSuit = suit; TheRank=rank } ]

    let (|DivisibleBy|_|) divideBy n = 
        if n % divideBy = 0 then Some DivisibleBy else None

    let (|Fizz|Buzz|FizzBuzz|Val|) n = 
        match n % 3, n % 5 with
        | 0, 0 -> FizzBuzz
        | 0, _ -> Fizz
        | _, 0 -> Buzz
        | _ -> Val n

    type Operation = Add | Sub | Mul | Div | Pow
    type Calculator =
        | Value of double
        | Expr of Operation * Calculator * Calculator

    type Tree<'a> =    
        | Empty        
        | Node of leaf:'a * left:Tree<'a> * right:Tree<'a> 

    let spawn (op:unit->double) = Task.Run(op)

    let rec eval expr =
        match expr with       
        | Value(value) -> value       
        | Expr(op, lExpr, rExpr) ->       
            let op1 = spawn(fun () -> eval lExpr)   
            let op2 = spawn(fun () -> eval rExpr)               
            let apply = Task.WhenAll([op1;op2])   
            let lRes, rRes = apply.Result.[0], apply.Result.[1]
            match op with   
                    | Add -> lRes + rRes
                    | Sub -> lRes - rRes
                    | Mul -> lRes * rRes
                    | Div -> lRes / rRes
                    | Pow -> System.Math.Pow(lRes, rRes)



    let reduceF R (reduce:'key -> seq<'value> -> 'reducedValues) (inputs:('key * seq<'key * 'value>) seq) =
        inputs 
        |> PSeq.withExecutionMode ParallelExecutionMode.ForceParallelism   
        |> PSeq.withDegreeOfParallelism R   
        |> PSeq.map (fun (key, items) ->  items 
                                       |> Seq.map (snd)   
                                       |> reduce key)   
                                       |> PSeq.toList


module Result =
 let ofChoice value =
            match value with
            | Choice1Of2 value -> Ok value
            | Choice2Of2 e -> Error e

//module AsyncResult = 
//      let handler (operation:Async<'a>) : AsyncResult<'a> = 
//        async {
//        let! result = Async.Catch operation
//        return (Result.ofChoice result) 
//        }

    
