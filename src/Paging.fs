namespace FSpotify

open Request

type Paging<'a> = {
    items: 'a list
    href: Url
    limit: int
    next: Url option
    offset: int
    previous: Url option
    total: int
} with static member Empty<'a> () = {
        items = List.empty<'a>
        href = Url ""
        limit = 0
        next = None
        offset = 0
        previous = None
        total = 0 }


[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Paging =
    
    type State<'a,'b> = {data: 'a Paging; next: Request<'a Paging,'b> option}

    let stateOf request paging =
        {
            data = paging
            next = paging.next |> Option.map (fun (Url url) ->
                    request |> Request.withUrl url
                )
        }


    let page<'a,'b> (request: Request<'a Paging,'b>) =
        request
        |> Request.mapResponse (stateOf request)
        |> Request.send

    let page4<'a,'b,'c,'d,'e> (request: Request<Paging<'a>*Paging<'b>*Paging<'c>*Paging<'d>,'e>) =
        request
        |> Request.mapResponse (fun (a,b,c,d) ->
            stateOf (request |> Request.mapResponse (fun (paging,_,_,_) -> paging)) a,
            stateOf (request |> Request.mapResponse (fun (_,paging,_,_) -> paging)) b,
            stateOf (request |> Request.mapResponse (fun (_,_,paging,_) -> paging)) c,
            stateOf (request |> Request.mapResponse (fun (_,_,_,paging) -> paging)) d
        )
        |> Request.send

    let rec asSeq {data = data; next = next} = seq {
    
        yield! data.items

        match next with
        | Some request ->
            yield! request |> page |> asSeq
        | None ->
            ()
    }


     