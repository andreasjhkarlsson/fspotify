namespace FSpotify

type Playlist = {
    collaborative: bool
    description: string
    external_urls: UrlMap
    followers: Followers
    href: Url
    id: SpotifyId
    images: Image list
    name: string
    owner: PublicUser
    ``public``: bool option
    snapshot_id: string
    tracks: SimpleTrack Paging
    ``type``: string
    uri: SpotifyUri
}


[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Playlist =

    ()