namespace FSpotify



module Misc =
    let buildCommaList = String.concat ","

    let buildIdList = List.map SpotifyId.asString >> buildCommaList