namespace FSpotify

open System

type PlaylistTrack = {
    added_at: DateTime // Todo: Handle null values (does not warrant an option type since only very old playlist may return null here)
    added_by: PublicUser
    is_local: bool
    track: Track
}

type Playlist = {
    collaborative: bool
    description: string
    external_urls: UrlMap
    followers: Followers
    href: Uri
    id: SpotifyId
    images: Image list
    name: string
    owner: PublicUser
    ``public``: bool option
    snapshot_id: string
    tracks: PlaylistTrack Paging
    ``type``: string
    uri: SpotifyUri
}


[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Playlist =

    let playlistsRequest (SpotifyId userId) =
        User.request
        |> Request.withUrlPath userId
        |> Request.withUrlPath "playlists"

    let playlistRequest userId (SpotifyId playlistId) =
        playlistsRequest userId |> Request.withUrlPath playlistId
    
    let playlistTracksRequest userId = playlistRequest userId >> Request.withUrlPath "tracks"

    let playlists userId =
        playlistsRequest userId
        |> Request.addOptionals (Optionals.LimitAndOffsetOption())
        |> Request.parse<SimplePlaylist Paging,_>

    let playlist userId playlistId =
        playlistRequest userId playlistId
        |> Request.addOptionals (Optionals.MarketOption()) // Todo: support fields optional(?)
        |> Request.parse<Playlist,_>

    let tracks userId playlistId =
        playlistTracksRequest userId playlistId
        |> Request.addOptionals (Optionals.MarketOffsetAndLimitOption()) // Todo: support fields optional(?)
        |> Request.parse<PlaylistTrack Paging,_>

    type playlistInfoArgs = {name: string option; ``public``: bool option}

    let create userId name ``public`` =
        playlistsRequest userId
        |> Request.withVerb Request.Post // Change to POST request
        |> Request.withJsonBody {name = Some name; ``public`` = Some ``public``}
        |> Request.parse<Playlist,_>

    let add userId playlistId (tracks: SpotifyId list)  =
        let uris =
            tracks
            |> List.map (SpotifyUri.track >> SpotifyUri.asString)
            |> Misc.buildCommaList

        playlistTracksRequest userId playlistId
        |> Request.withVerb Request.Post
        |> Request.withQueryParameter ("uris",uris)
        |> Request.addOptionals (Optionals.PositionOption())
        |> Request.unwrap "snapshot_id"
        |> Request.mapResponse SpotifyId

    let change userId playlistId (name: string option) (``public``: bool option) =
        let args =
            ["name", name |> Option.map box
             "public", ``public`` |> Option.map box]
            |> List.filter (snd >> Option.isSome)
            |> Map.ofList
        playlistRequest userId playlistId 
        |> Request.withVerb Request.Put
        |> Request.withJsonBody args
        |> Request.mapResponse (fun _ -> ())

    type RemovedTrack = {uri: SpotifyUri}
    type RemovedTrackAtPositions = {uri: SpotifyUri; positions: int list}

    let removeTracksWithSomePositions userId playlistId (tracksAndPositions: list<SpotifyId*(int list) option>) =
        let args =
            tracksAndPositions
            |> List.fold (fun args (track,positions) ->
                let trackUri = track |> SpotifyUri.track
                match positions with
                | Some positions ->
                    (box {uri = trackUri; positions = positions}) :: args
                | None ->
                    (box {RemovedTrack.uri = trackUri}) :: args
                ) List.empty
            |> (fun tracks ->
                [("tracks",tracks)] |> Map.ofList
            )

        playlistTracksRequest userId playlistId
        |> Request.withVerb Request.Delete
        |> Request.withJsonBody args
        |> Request.unwrap "snapshot_id"
        |> Request.mapResponse SpotifyId

    let removeTracks userId playlistId =
        List.map (fun track -> track, None) >> removeTracksWithSomePositions userId playlistId

    let removeTracksWithPositions userId playlistId =
        List.map (fun (id,positions) -> id, Some positions) >> removeTracksWithSomePositions userId playlistId

    type reorderArguments = {range_start: int; insert_before: int; range_length: int}

    let reorderTracks userId playlistId start insertBefore length =
        playlistTracksRequest userId playlistId
        |> Request.withVerb Request.Put
        |> Request.withJsonBody {range_start = start
                                 insert_before = insertBefore
                                 range_length = length}
        |> Request.unwrap "snapshot_id"
        |> Request.mapResponse SpotifyId
       
    let reorderTrack userId playlistId trackPosition insertPosition = reorderTracks userId playlistId trackPosition insertPosition 1

    let replaceTracks userId playlistId tracks =
        playlistTracksRequest userId playlistId
        |> Request.withVerb Request.Put
        |> Request.withJsonBody (["uris",tracks |> List.map SpotifyUri.track] |> Map.ofList)
        |> Request.mapResponse (fun _  -> ())