module App.Notification

type Note = {
    Title: string;
    Message: string;
}

type MsgType =
| Success of Note
| Warning of Note
| Error of Note
| Info of Note
