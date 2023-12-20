module UserFunctions

open System.IO
open Thoth.Json.Net

// Define a type for user attributes
type UserAttributes = {
    UserName: string
    EMail: string
    // Add other attributes as needed
}

let readUserList filePath =
    let jsonString = File.ReadAllText(filePath)
    Decode.Auto.fromString<UserAttributes list> jsonString

