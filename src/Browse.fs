namespace FSpotify

open Optionals

module Browse =
    
    let request = Request.createFromEndpoint Request.Get "browse"

    let newReleases =
        request
        |> Request.withUrlPath "new-releases"
        |> Request.withOptionals (fun _ -> CountryOffsetAndLimitOption.Default)
        |> Request.unwrap "albums"
        |> Request.parse<SimpleAlbum Paging,_>

    let featuredPlaylists =
        request
        |> Request.withUrlPath "featured-playlists"
        |> Request.withOptionals (fun _ -> TimestampLocaleCountryOffsetAndLimitOption.Default)
        |> Request.unwrap "playlists"
        |> Request.parse<SimplePlaylist Paging,_>

    let categories =
        request
        |> Request.withUrlPath "categories"
        |> Request.withOptionals (fun _ -> LocaleCountryOffsetAndLimitOption.Default)
        |> Request.unwrap "categories"
        |> Request.parse<Category Paging,_>

    let category (SpotifyId id) =
        request
        |> Request.withUrlPath "categories"
        |> Request.withUrlPath id
        |> Request.withOptionals (fun _ -> CountryAndLocaleOption.Default)
        |> Request.parse<Category,_>

    let categoryPlaylists (SpotifyId id) =
        request
        |> Request.withUrlPath (sprintf "categories/%s/playlists" id)
        |> Request.withOptionals (fun _ -> CountryOffsetAndLimitOption.Default)
        |> Request.unwrap "playlists"
        |> Request.parse<SimplePlaylist Paging,_>