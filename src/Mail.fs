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
    try
        let template = loadTemplate templatePath
        sendEmail smtpConfig userAttributes template
        logResult (sprintf "OK %s" userAttributes.EMail)
    with ex ->
        logResult (sprintf "Error: %s (%s)" ex.Message userAttributes.EMail)

let (>>=) result func = Result.bind func result

let processUserList smtpConfigPath templatePath userListPath =
    readSmtpConfig smtpConfigPath
    >>= (fun smtpConfig ->
        readUserList userListPath
        >>= (fun userList ->
            let processOneUser = processUser smtpConfig templatePath
            List.iter processOneUser userList
            Ok()))
