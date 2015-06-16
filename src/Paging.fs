namespace FSpotify

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
    
    let next (paging: 'a Paging) =
            match paging.next with
            | Some (Url url) ->
                Request.create url
                |> Request.parse<'a Paging,_> 
                |> Request.send
                |> Some               
            | None ->
                None         

    let rec asSeq (paging: 'a Paging) =
        seq {
            yield! paging.items

            match next paging with
            | Some paging ->
                yield! asSeq paging
            | None ->
                ()
        }