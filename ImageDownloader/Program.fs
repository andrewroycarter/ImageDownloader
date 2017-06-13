// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open FSharp.Data

type Page = JsonProvider<"""
{
    "results": [{
        "images": [{
            "url": "https://website.com/image.jpeg"
        }]
    }]
}
""">

let getPage :Async<Result<Page.Root, exn>> = async {
    try 
        let! page = Page.AsyncLoad ("INSERT_URL_HERE")
        return Ok page
    with error -> return Error error
}

let placeholderImages (page:Page.Root) =
    page.Results
    |> Seq.collect (fun result ->
        match Seq.tryHead result.Images with
        | Some image -> [image.Url]
        | None -> []
        )
    |> Seq.filter (fun url -> url.Contains "nopic")
    |> Seq.distinctBy (fun url -> Array.last <| url.Split [|'_'|])

[<EntryPoint>]
let main argv = 
    getPage
    |> Async.RunSynchronously
    |> Result.map (fun page -> placeholderImages page)
    |> (fun result ->
        match result with
        | Ok images -> Seq.fold (fun state image -> state + "\n" + image) "" images
        | Error error -> sprintf "Error downloading images: %s" <| error.ToString ()
        )
    |> printfn "%s"

    0 // return an integer exit code