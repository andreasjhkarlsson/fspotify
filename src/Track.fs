namespace FSpotify

open System
open Optionals

type Track = {
    album: SimpleAlbum
    artists: SimpleArtist list
    available_markets: Market list
    disc_number: int
    duration_ms: int
    explicit: bool
    external_ids: ExternalIdMap
    external_urls: UrlMap
    href: Uri
    id: SpotifyId
    is_playable: bool
    linked_from: unit
    name: string
    popularity: int
    preview_url: Uri
    track_number: int
    ``type``: string
    uri: Uri
}

[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Track =
    

    let trackRequest = Request.createFromEndpoint Request.Get "tracks"

    let track (SpotifyId id) =
        trackRequest
        |> Request.withUrlPath id
        |> Request.addOptionals (MarketOption())
        |> Request.parse<Track,_>

    let tracks ids =
        trackRequest
        |> Request.withQueryParameter("ids", Misc.buildIdList ids)
        |> Request.unwrap "tracks"
        |> Request.parse<Track list,_>