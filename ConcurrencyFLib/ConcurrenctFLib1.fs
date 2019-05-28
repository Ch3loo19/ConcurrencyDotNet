namespace Concurrency.F

type Person (firstName: string, lastName: string) =
    member this.FirstName = firstName
    member this.LastName = lastName
    member this.FullName = String.concat (",") <| [firstName; lastName]


module F_Library =
    let LazyF =
        let kibsterFunction = lazy (Person( "Thomas", "McKirbster"))
        kibsterFunction.Force().FullName
            
