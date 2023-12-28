module SmtpFunctions

open System.IO
open Thoth.Json.Net
open UserFunctions
open MimeKit
open MailKit.Net.Smtp
open MailKit.Security

// Define a type for SMTP configuration
type SmtpConfig = {
    Host: string
    Port: int
    UserName: string
    Password: string
}

let readSmtpConfig filePath =
    let jsonString = File.ReadAllText(filePath)
    Decode.Auto.fromString<SmtpConfig> jsonString

// Function to send email using MailKit
let sendEmail smtpConfig (userAttributes: UserAttributes) template =
    let message = new MimeMessage()
    message.From.Add(MailboxAddress("FOSS", smtpConfig.UserName))
    message.To.Add(MailboxAddress(userAttributes.UserName, userAttributes.EMail))
    message.Subject <- "Test Subject"

    // Create a multipart/mixed MIME entity
    let multipart = new Multipart("mixed")

    // Create the text part (can be either TextPart or HtmlPart)
    let textPart = new TextPart("plain") 
    textPart.Text <- template
    multipart.Add(textPart)

    // Create the HTML part with an embedded image
    let htmlPart = new TextPart("html")
    htmlPart.Text <- "<html><body><p>Body of the email with an embedded image:</p><img src='cid:embedded-image' /></body></html>"
    multipart.Add(htmlPart)
    
    // // Attach an image as a MimePart
    // let imagePart = new MimePart("image", "png") // Adjust the content type and subtype based on your image
    // imagePart.ContentTransferEncoding <- ContentEncoding.Base64
    // imagePart.ContentObject <- new ContentObject(new System.IO.FileStream("path/to/your/image.png", System.IO.FileMode.Open))
    // imagePart.ContentDisposition <- new ContentDisposition(ContentDisposition.Attachment)
    // imagePart.ContentDisposition.FileName <- "image.png"
    // multipart.Add(imagePart)

    // Attach the image as a MimePart with Content-ID (CID)
    // let imagePart = new MimePart("image", "png") // Adjust the content type and subtype based on your image
    // imagePart.ContentTransferEncoding <- ContentEncoding.Base64
    // imagePart.ContentObject <- new ContentObject(new System.IO.FileStream("path/to/your/image.png", System.IO.FileMode.Open))
    // imagePart.ContentDisposition <- new ContentDisposition(ContentDisposition.Inline)
    // imagePart.ContentDisposition.FileName <- "embedded-image.png"
    // imagePart.ContentId <- MimeUtils.GenerateMessageId ()
    // multipart.Add(imagePart)


    // Set the multipart as the message body
    message.Body <- multipart

    try
        use client = new SmtpClient ()

        client.Connect(smtpConfig.Host, smtpConfig.Port, SecureSocketOptions.StartTls)
        client.Authenticate(smtpConfig.UserName, smtpConfig.Password)
        client.Send(message) |> ignore
        client.Disconnect(true)

        Ok userAttributes.EMail
    with ex ->
        Error <| sprintf "%s (%s)" ex.Message userAttributes.EMail
