namespace FSpotify

open Misc


type Artist = {
    external_urls: UrlMap
    followers: Followers
    genres: Genre list
    href: Url
    id: SpotifyId
    images: Image list
    name: string
    popularity: int
    ``type``: string
    uri: SpotifyUri
}

[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Artist =

    let artistsRequest = Request.createFromEndpoint Request.Get "artists"

    let artist (SpotifyId id) =
        artistsRequest
        |> Request.withUrlPath id
        |> Request.parse<Artist,_>

    let artists ids =
        artistsRequest
        |> Request.withQueryParameter ("ids",buildIdList ids)
        |> Request.unwrap "artists"
        |> Request.parse<Artist list,_>

    let albums (SpotifyId id) =
        artistsRequest
        |> Request.withUrlPath id
        |> Request.withUrlPath "albums"
        |> Request.addOptionals (Optionals.MarketOffsetLimitAndAlbumTypesOption())
        |> Request.parse<SimpleAlbum Paging,_>

    let topTracks (SpotifyId id) (Country country) =
        artistsRequest
        |> Request.withUrlPath id
        |> Request.withUrlPath "top-tracks"
        |> Request.withQueryParameter("country",country)
        |> Request.unwrap "tracks"
        |> Request.parse<Track list,_>

    let related (SpotifyId id) =
        artistsRequest
        |> Request.withUrlPath id
        |> Request.withUrlPath "related-artists"
        |> Request.unwrap "artists"
        |> Request.parse<Artist list,_>