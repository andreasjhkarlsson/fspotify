namespace FSpotify

open System

type PlaylistTrack = {
    added_at: string // Todo: Change to DateTime & handle null values (does not warrant an option type since only very old playlist may return null here)
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
    uri: Uri
}


[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Playlist =

    let playlistsRequest (SpotifyId userId) =
        User.request
        |> Request.withUrlPath userId
        |> Request.withUrlPath "playlists"

    let playlistRequest userId (SpotifyId playlistId) =
        playlistsRequest userId |> Request.withUrlPath playlistId
        

    let playlists userId =
        playlistsRequest userId
        |> Request.addOptionals (Optionals.LimitAndOffsetOption())
        |> Request.parse<SimplePlaylist Paging,_>

    let playlist userId playlistId =
        playlistRequest userId playlistId
        |> Request.addOptionals (Optionals.MarketOption()) // Todo: support fields optional(?)
        |> Request.parse<Playlist,_>

    let tracks userId playlistId =
        playlistRequest userId playlistId
        |> Request.withUrlPath "tracks"
        |> Request.addOptionals (Optionals.MarketOffsetAndLimitOption()) // Todo: support fields optional(?)
        |> Request.parse<PlaylistTrack Paging,_>

    type createPostArgs = {name: string; ``public``: bool}

    let create userId name ``public`` =
        playlistsRequest userId
        |> Request.withVerb Request.Post // Change to POST request
        |> Request.withJsonBody {name = name; ``public`` = ``public``}
        |> Request.parse<Playlist,_>