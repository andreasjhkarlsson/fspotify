namespace FSpotify


module Browse =
    
    let request = Request.createFromEndpoint Request.Get "browse"

    let newReleases =
        request
        |> Request.withUrlPath "new-releases"
        |> Request.addOptionals (Optionals.CountryOffsetAndLimitOption())
        |> Request.unwrap "albums"
        |> Request.parse<SimpleAlbum Paging,_>

    let featuredPlaylists =
        request
        |> Request.withUrlPath "featured-playlists"
        |> Request.addOptionals (Optionals.LocaleTimestampCountryOffsetAndLimitOption())
        |> Request.unwrap "playlists"
        |> Request.parse<SimplePlaylist Paging,_>