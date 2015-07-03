namespace FSpotify

open Optionals

module Search = 
    type Query = Query of string

    [<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
    module Query =
        
        let wildcard = "*"

        let add str (Query query) = (if query = "" then str else sprintf "%s+%s" query str) |> Query

        let excluding str = add "NOT" >> add str

        let either str = add "OR" >> add str

        let artist = (+) "artist:"

        let album = (+) "album:"

        let track = (+) "track:"
    
        let genre = (+) "genre:"

        let upc = (+) "upc:"

        let isrc = (+) "isrc:"


    type SearchTypes = Track | Artist | Album | Playlist

    let search (types: SearchTypes List) (Query query) =

        let typesArgument =
            types 
            |> List.map (function |Artist -> "artist" |Album -> "album" |Playlist -> "playlist" |Track -> "track")
            |> Misc.buildCommaList

        Request.createFromEndpoint Request.Get "search"
        |> Request.withQueryParameter ("query", query)
        |> Request.withQueryParameter ("type",typesArgument)
        |> Request.withOptionals (fun _ -> MarketLimitAndOffsetOption.Default)
        |> Request.mapResponse (fun response ->
            let unwrapAndParse element =
                if Serializing.hasElement element response then
                    response |> Serializing.unwrap element |> Serializing.deserialize<'a Paging>
                else
                    Paging.Empty<'a> ()
            (unwrapAndParse "tracks", unwrapAndParse "artists", unwrapAndParse "albums", unwrapAndParse "playlists"):
                Paging<Track>*Paging<Artist>*Paging<SimpleAlbum>*Paging<SimplePlaylist>
        )

    let all = search [Track; Artist; Album; Playlist]

    let track = search [Track] >> Request.mapResponse (fun (tracks,_,_,_) -> tracks)

    let artist = search [Artist] >> Request.mapResponse (fun (_,artists,_,_) -> artists)

    let album = search [Album] >> Request.mapResponse (fun (_,_,albums,_) -> albums)

    let playlist = search [Playlist] >> Request.mapResponse (fun (_,_,_,playlists) -> playlists)