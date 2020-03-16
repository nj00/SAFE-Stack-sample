module App.Notification

type Note = {
    title: string;
    message: string;
}

type MsgType =
| Success of Note
| Warning of Note
| Error of Note
| Info of Note
