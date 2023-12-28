module EmailFunctions

open System
open System.IO
open UserFunctions
open SmtpFunctions

// Function to load and parse the email template
let loadTemplate filePath = File.ReadAllText(filePath)

// Function to log the result
let logResult result =
    let logFileName =
        sprintf "logs/email-sent-%s.log" (DateTime.Now.ToString("yyyyMMdd"))

    Directory.CreateDirectory("logs") |> ignore
    File.WriteAllText(logFileName, sprintf "%s: %s" (DateTime.Now.ToString("yyyyMMdd-HHmmss")) result)

// Main function to process each user
let processUser smtpConfig templatePath (userAttributes: UserAttributes) =
    let template = loadTemplate templatePath
    match sendEmail smtpConfig userAttributes template with
    | Ok s -> logResult <| sprintf "Ok: %s" s
    | Error s -> logResult <| sprintf "Error: %s" s

let (>>=) result func = Result.bind func result

let processUserList smtpConfigPath templatePath userListPath =
    readSmtpConfig smtpConfigPath
    >>= (fun smtpConfig ->
        readUserList userListPath
        >>= (fun userList ->
            let processOneUser = processUser smtpConfig templatePath
            List.iter processOneUser userList
            Ok()))
