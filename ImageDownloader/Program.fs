// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open FSharp.Data
open System

module String =
    let contains string (input:string) = input.Contains string

    let split separators (input:string) = input.Split separators

    let newlineConcat string input = string + "\n" + input

type Page = JsonProvider<"""
{
    "results": [{
        "images": [{
            "url": "https://website.com/image.jpeg"
        }]
    }]
}
""">

let getPage = async {
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
    |> Seq.filter (String.contains "nopic")
    |> Seq.distinctBy (String.split [|'_'|] >> Array.last)

[<EntryPoint>]
let main argv = 
    getPage
    |> Async.RunSynchronously
    |> Result.map placeholderImages
    |> (fun result ->
        match result with
        | Ok images -> Seq.fold String.newlineConcat "" images
        | Error error -> sprintf "Error downloading images: %s" <| error.ToString ()
        )
    |> printfn "%s"

    0 // return an integer exit code