namespace FSpotify

open Misc

type Album = {
    album_type: AlbumType
    artists: SimpleArtist list
    available_markets: Market list
    copyrights: Copyright list
    external_ids: ExternalIdMap
    external_urls: UrlMap
    genres: Genre list
    href: Url
    id: SpotifyId
    images: Image list
    name: string
    popularity: int
    release_date: string
    release_date_precision: DatePrecision
    tracks: SimpleTrack Paging
    ``type``: string
    uri: SpotifyUri
}


[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Album =
    
    let toSimple (album: Album) =
        {
            SimpleAlbum.album_type = album.album_type
            available_markets = album.available_markets
            external_urls = album.external_urls
            href = album.href
            id = album.id
            images = album.images
            name = album.name
            ``type`` = album.``type``
            uri = album.uri
        }

    let request = Request.createFromEndpoint Request.Get "albums"
        
    let album (SpotifyId id) =
        request
        |> Request.withUrlPath id
        |> Request.parse<Album,_>
                       
    let albums ids =
        request
        |> Request.withQueryParameter ("ids",buildIdList ids)
        |> Request.unwrap "albums"
        |> Request.addOptionals (Optionals.MarketOption())
        |> Request.parse<Album list,_>
            
    let tracks (SpotifyId id) =
        request
        |> Request.withUrlPath id
        |> Request.withUrlPath "tracks"
        |> Request.addOptionals (Optionals.MarketOffsetAndLimitOption())
        |> Request.parse<SimpleTrack Paging,_>

    let ofSimple (simple: SimpleAlbum) = album simple.id
    

