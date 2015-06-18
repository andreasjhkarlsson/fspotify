namespace FSpotify

type ErrorObject = {status: string; message: string}

type Token = {access_token: string; token_type: string; expires_in: int}

type SpotifyId =
    |SpotifyId of string
    static member asString (SpotifyId id) = id

type AlbumType =
    |Album
    |Single
    |Compilation
    |Appears_On
    static member asString this =
        match this with
        | Album -> "album"
        | Single -> "single"
        | Compilation -> "compilation"
        | Appears_On -> "appears_on"

type Market = Market of string

type Country = Country of string

type Locale = Locale of string

type CopyrightType = C | P

type Copyright = {text: string; ``type``: CopyrightType}

type ExternalId = ExternalId of string

type ExternalIdType = ISRC | EAN | UPC | Other of string

// Todo: use ExternalIdType instead of string (needs json type converter)
type ExternalIdMap = Map<string,ExternalId>

type UrlType = Spotify

type Url = Url of string

// Todo: use UrlType instead of string (needs json type converter)
type UrlMap = Map<string,Url>

type Genre = Genre of string

type Image = {
    url: Url
    height: int option
    width: int option
}

type DatePrecision = Year | Month | Day

type SpotifyUri = SpotifyUri of string

type Followers = {
    href: Url option
    total: int
}

type Tracks = {
    href: Url
    total: int
}

type SimpleArtist = {
    external_urls: UrlMap
    href: Url
    id: SpotifyId
    name: string
    ``type``: string
    uri: SpotifyUri
}

type SimpleAlbum = {
    album_type: AlbumType
    available_markets: Market list
    external_urls: UrlMap
    href: Url
    id: SpotifyId
    images: Image list
    name: string
    ``type``: string
    uri: SpotifyUri
}

type PublicUser = {
    display_name: string
    external_urls: UrlMap
    followers: Followers
    href: Url
    id: SpotifyId
    images: Image list
    ``type``: string
    uri: SpotifyUri
}

type SimpleTrack = {
    artists: SimpleArtist list
    available_markets: Market list
    disc_number: int
    duration_ms: int
    explicit: bool
    external_urls: UrlMap
    href: Url
    id: SpotifyId
    is_playable: bool
    linked_from: unit
    name: string
    preview_url: Url
    track_number: int
    ``type``: string
    uri: SpotifyUri 
}

type SimplePlaylist = {
    collaborative: bool
    external_urls: UrlMap
    href: Url
    id: SpotifyId
    images: Image list
    name: string
    owner: PublicUser
    ``public``: bool option
    snapshot_id: string
    tracks: Tracks
    ``type``: string
    uri: SpotifyUri
}

type Category = {
    href: Url
    icons: Image list
    id: SpotifyId
    name: string
}

