import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../environments/environment';
import { MessageView } from '../views/chat-view/chat-view';
import { Message } from '../views/chat-view/chat-view';


@Injectable({
    providedIn: 'root'
})
export class SignalRService {
    private apiUrl = environment.apiUrl + 'chathub';

    private hubConnection!: signalR.HubConnection;

    startConnection() {
        this.hubConnection = new signalR.HubConnectionBuilder()
        .withUrl(this.apiUrl, {
            accessTokenFactory: () => localStorage.getItem('auth-token') || '',
            transport: signalR.HttpTransportType.WebSockets
        })
        .withAutomaticReconnect()
        .build();

        this.hubConnection
        .start()
        .then(() => console.log('SignalR Connected'))
        .catch(err => console.log('Error while starting connection: ' + err));
    }

    addMessageListener(callback: (userId: number, message: MessageView) => void) {
        this.hubConnection.on('ReceiveMessage', callback);
    }

    sendMessage(userReceiveId: number, message: Message) {
        this.hubConnection.invoke('SendMessage', userReceiveId, message)
        .catch(err => console.error(err));
    }

    joinGroup(groupId: number, userIds: number[]) {
        this.hubConnection.invoke('JoinGroup', groupId, userIds)
            .catch(err => console.error(err));
    }

    leaveGroup(groupId: number) {
        this.hubConnection.invoke('LeaveGroup', groupId)
            .catch(err => console.error(err));
    }

    sendMessageToGroup(groupId: number, message: Message) {
        this.hubConnection.invoke('SendMessageToGroup', groupId, message)
            .catch(err => console.error(err));
    }

    viewedMessage(messageId: string) {
        this.hubConnection.invoke('ViewedMessage', messageId)
            .catch(err => console.error(err));
    }

    viewedMessageListener(callback: (messageId: string) => void) {
        this.hubConnection.on('ViewedMessage', callback);
    }
}
