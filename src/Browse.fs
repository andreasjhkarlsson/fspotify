namespace FSpotify


module Browse =
    
    let request = Request.createFromEndpoint Request.Get "browse"

    let newReleases =
        request
        |> Request.withUrlPath "new-releases"
        |> Request.addOptionals (Optionals.CountryOffsetAndLimitOption())
        |> Request.unwrap "albums"
        |> Request.parse<SimpleAlbum Paging,_>

