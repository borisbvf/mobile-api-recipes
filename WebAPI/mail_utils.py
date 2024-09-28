import ssl, smtplib
import os
from email.message import EmailMessage
from mailjet_rest import Client

def send_email(subject: str, text: str, receiver: str):
    port = 587
    smtp_server = os.getenv("EMAIL_SMTP_SERVER")
    sender_email = os.getenv("EMAIL_SENDER")
    password = os.getenv("EMAIL_PASSWORD")
    msg = EmailMessage()
    msg.set_content(text)
    msg['Subject'] = subject
    msg['From'] = sender_email
    msg['To'] = receiver
    context = ssl.create_default_context()
    with smtplib.SMTP(smtp_server, port) as server:
        server.starttls(context=context)
        server.ehlo()
        server.login(sender_email, password)
        server.ehlo()
        server.send_message(msg)

def send_email_mailjet(subject: str, text: str, receiver: str):
    api_key = os.getenv("MAILJET_API_KEY")
    api_secret = os.getenv("MAILJET_SECRET")
    mailjet = Client(auth=(api_key, api_secret), version='v3.1')
    data = {
        'Messages': [
            {
                "From": {
                    "Email": os.getenv("EMAIL_SENDER"),
                    "Name": "Recipes Notification Center"
                },
                "To": [
                    {
                        "Email": receiver,
                        "Name": "Recipes User"
                    }
                ],
                "Subject": subject,
                "TextPart": text
            }
        ]
    }
    mailjet.send.create(data=data)

def send_code(email_address: str, code: str):
    subject = "Welcome to Recipes App"
    text = f"Your code for application is {code}"
    send_email_mailjet(subject, text, email_address)