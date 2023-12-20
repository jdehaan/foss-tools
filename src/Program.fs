open EmailFunctions

let smtpConfigPath = "config/smtp.conf"
let templatePath = "data/template.txt"
let userListPath = "data/users.json"

let result = processUserList smtpConfigPath templatePath userListPath

match result with
| Ok() -> ignore()
| Error e -> printfn "Error: %s" e
