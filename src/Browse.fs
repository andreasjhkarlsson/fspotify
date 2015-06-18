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
        |> Request.addOptionals (Optionals.TimestampLocaleCountryOffsetAndLimitOption())
        |> Request.unwrap "playlists"
        |> Request.parse<SimplePlaylist Paging,_>

    let categories =
        request
        |> Request.withUrlPath "categories"
        |> Request.addOptionals (Optionals.LocaleCountryOffsetAndLimitOption())
        |> Request.unwrap "categories"
        |> Request.parse<Category Paging,_>

    let category (SpotifyId id) =
        request
        |> Request.withUrlPath "categories"
        |> Request.withUrlPath id
        |> Request.addOptionals (Optionals.CountryAndLocaleOption())
        |> Request.parse<Category,_>

    let categoryPlaylists (SpotifyId id) =
        request
        |> Request.withUrlPath (sprintf "categories/%s/playlists" id)
        |> Request.addOptionals (Optionals.CountryOffsetAndLimitOption())
        |> Request.unwrap "playlists"
        |> Request.parse<SimplePlaylist Paging,_>