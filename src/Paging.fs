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
}


[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Paging =
    
    type State<'a,'b> = {data: 'a Paging; next: Request<'a Paging,'b> option}

    let page<'a,'b> (request: Request<'a Paging,'b>) =
        request
        |> Request.mapResponse (fun paging ->
            {
                data = paging
                next = paging.next |> Option.map (fun (Url url) ->
                        request |> Request.withUrl url
                    )
            }
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


     