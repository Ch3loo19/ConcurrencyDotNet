namespace Concurrency.F

open System
open System.Collections.Generic
open System.Threading.Tasks

type Person (firstName: string, lastName: string) =
    member this.FirstName = firstName
    member this.LastName = lastName
    member this.FullName = String.concat (",") <| [firstName; lastName]

type FList<'a> =
    | Empty   
    | Cons of head:'a * tail:FList<'a>   



module F_Library =
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
    
