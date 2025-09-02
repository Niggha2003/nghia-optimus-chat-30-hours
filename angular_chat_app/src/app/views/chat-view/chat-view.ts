import { AfterViewInit, ChangeDetectionStrategy, CUSTOM_ELEMENTS_SCHEMA , Component, ElementRef, OnInit, ViewChild, HostListener, ViewChildren, QueryList } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatDividerModule } from '@angular/material/divider';
import { MatListModule } from '@angular/material/list';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { UserService } from '../../services/user.service';
import { GroupService } from '../../services/group.service';
import { MatDialog } from '@angular/material/dialog';
import { MatSelectModule } from '@angular/material/select';
import { MessageService } from '../../services/message.service';
import { CommonModule } from '@angular/common';
import { SignalRService } from '../../services/signalr.service';
import { PickerComponent } from '@ctrl/ngx-emoji-mart';
import { AdditionFileService } from '../../services/addition-file.service';

export interface User {
  id: number;
  userName: string;
  userEmail: string;
  roleName: string;
}

export interface Group {
  id: number;
  groupName: string;
  users: User[];
}

export interface Message {
  id: string;
  fromId: number;
  toId: number;
  isGroup: boolean;
  content: string;
  createAt: Date;
  isViewed?: boolean;
  hasFile?: boolean;
}

export interface MessageView {
  id: string;
  fromId: number;
  toId: number;
  fromName: string;
  toName: string;
  isGroup: boolean;
  groupName: string | null;
  content: string;
  createAt: Date;
  isViewed?: boolean;
  hasFile?: boolean;
}

export interface Base64File {
  id: string;
  messageId: string;
  fileName: string;
  FileBase64Content: string;
}

@Component({
  selector: 'app-chat-view',
  imports: [
    MatListModule, 
    MatDividerModule, 
    FormsModule, 
    MatFormFieldModule, 
    MatInputModule,
    MatIconModule,
    MatButtonModule,
    MatSelectModule,
    CommonModule,
    PickerComponent
  ],
  standalone: true,
  changeDetection: ChangeDetectionStrategy.Default,
  templateUrl: './chat-view.html',
  styleUrl: './chat-view.scss',
  schemas: [CUSTOM_ELEMENTS_SCHEMA], // <-- thêm dòng này
})
export class ChatView implements OnInit, AfterViewInit {
  @ViewChild('dialogNewChatTemplate') dialogNewChatTemplate: any;
  @ViewChild('dialogNewGroupTemplate') dialogNewGroupTemplate: any;
  @ViewChild('chatContainer') private chatContainer!: ElementRef;
  @ViewChild('emojiWrapper') emojiWrapper!: ElementRef;
  @ViewChildren('messageFileContainer') messageFileContainers!: QueryList<ElementRef<HTMLDivElement>>;
  
  selectedChatName: string = '';
  newMessage: string = '';
  errorMessage: string = '';
  group: Group = { id: 0, groupName: '', users: [] };
  userList: User[] = [];
  userToSendInform: User | null = null;
  userToSendReal: User | null = null;
  groupToSend: Group = { id: 0, groupName: '', users: [] };
  isSendingToGroup: boolean = false;
  currentUser: User | null = null;
  messageList: MessageView[] = [];
  totalMessageList: MessageView[] = [];
  showEmojiPicker = false;

  files: File[] = [];
  filesBase64: Base64File[] = [];
  uploadProgress: number = 0;

  constructor(private additionFileService: AdditionFileService, private signalRService: SignalRService, private messageService: MessageService, private dialog: MatDialog, private userService: UserService, private groupService: GroupService) {
    this.getCurrentUser();
  }

  ngAfterViewInit() {
    // Khi View init xong, load file cho tất cả message có file

    
  }

  ngOnInit() {
    this.fetchUsers();
    this.signalRService.startConnection();
    this.signalRService.viewedMessageListener((messageId: string) => {
      var messageIndex = this.messageList.findIndex(m => m.id === messageId);
      
      if(messageIndex !== -1) {
        this.messageList[messageIndex].isViewed = true;
      }
    });
    this.signalRService.addMessageListener((userId: number, message: MessageView) => {
      var messageSideIndex = this.totalMessageList.findIndex(m => (message.isGroup && message.toId == m.toId) || (!message.isGroup && (m.fromId === message.fromId && m.toId === message.toId || m.fromId === message.toId && m.toId === message.fromId)));
      var messageFromName = message.fromName;
      if(messageSideIndex === -1) {
        message.fromName = message.toName;
        this.totalMessageList.push({ ...message });
      }else{
        this.totalMessageList[messageSideIndex].id = message.id;
        this.totalMessageList[messageSideIndex].fromId = message.fromId;
        this.totalMessageList[messageSideIndex].toId = message.toId;
        this.totalMessageList[messageSideIndex].content = message.content;
        this.totalMessageList[messageSideIndex].createAt = message.createAt;
        this.totalMessageList[messageSideIndex].isViewed = false;
      }
      if( (message.fromId === (this.userToSendReal ? this.userToSendReal.id : 0) || message.fromId === this.currentUser?.id)) {
        message.fromName = messageFromName;
        console.log("nhận", { ...message })
        this.messageList.push({ ...message });
        this.messageList.forEach((message, index) => {
            if (message.hasFile) {
              this.loadFilesForMessage(message, index);
            }
          });
        if(message.fromId !== this.currentUser?.id && !message.isViewed) {
          this.totalMessageList = this.totalMessageList.map((msg, index) =>
            index === messageSideIndex ? { ...msg, isViewed: false } : msg
          );
          message.isViewed = true;
          this.messageService.update(message).subscribe({
            next: () => {
              this.signalRService.viewedMessage(message.id);
            },
            error: (err) => {
              console.error('Failed to fetch messages', err);
              }
          });
        }
      }
      // Delay để Angular render DOM xong
      setTimeout(() => {
        this.scrollToBottom();
      }, 100);
    });
  }

  sendMessage() {
    if (!this.newMessage.trim() && this.files.length == 0) return;
    var newMessageData: Message = {
      id: '',
      fromId: this.currentUser!.id,
      toId: this.isSendingToGroup ? this.groupToSend.id : this.userToSendReal!.id,
      isGroup: this.isSendingToGroup,
      content: this.newMessage.trim(),
      createAt: new Date(),
      hasFile: this.files.length > 0
    };

    this.messageService.create(newMessageData).subscribe({
        next: (data) => {
          this.uploadFiles(data.id)
          newMessageData.id = data.id;
          if(this.isSendingToGroup) {
            this.signalRService.sendMessageToGroup(this.groupToSend.id, newMessageData);
          }else{
            this.signalRService.sendMessage(this.userToSendReal!.id, newMessageData);
          }
        },
        error: (err) => {
          console.error('Failed to change role', err);
        }
      });
    this.newMessage = '';
  }

  openNewChatDialog() {
    const dialogRef = this.dialog.open(this.dialogNewChatTemplate);
  }

  openNewGroupDialog() {
    const dialogRef = this.dialog.open(this.dialogNewGroupTemplate);
  }

  closeDialog() {
    this.dialog.closeAll();
  }

  submitNewChatDialog() {
    this.userToSendReal = this.userToSendInform;
    this.isSendingToGroup = false;
    this.selectedChatName = this.userToSendReal ? this.userToSendReal.userName : '';
    this.fetchMessage(this.currentUser!.id, this.userToSendReal!.id);
    this.closeDialog();
  }

  submitNewGroupDialog() {
    this.groupToSend = this.group;
    this.isSendingToGroup = true;
    var groupUserIds = this.groupToSend.users.map(u => u.id);
    groupUserIds.push(this.currentUser!.id);
    var newGroup = {
      ownerId: this.currentUser?.id,
      groupName: this.groupToSend.groupName,
      userIds: groupUserIds
    }
    this.groupService.create(newGroup).subscribe({
        next: (data) => {
          this.groupToSend.id = data.id;
          this.isSendingToGroup = true;
          this.selectedChatName = this.groupToSend ? this.groupToSend.groupName : '';
          this.signalRService.joinGroup(data.id, newGroup.userIds);
          this.messageList = [];
          this.closeDialog();
        },
        error: (err) => {
          console.error('Failed to change role', err);
          this.errorMessage = err?.error?.message || 'Failed to change role';
        }
      });
    this.closeDialog();
  }

  fetchUsers() {
    this.userService.getAll().subscribe({
      next: (data) => {
        this.userList = data.filter((u: User) => u.id !== this.currentUser?.id);
      },
      error: (err) => {
        console.error('Failed to fetch users', err);
      }
    });
  }

  getCurrentUser() {
    this.userService.getCurrentUser().subscribe({
      next: (data) => {
        this.currentUser = data;
        this.fetchUsers();
        this.fetchChat();
      },
      error: (err) => {
        console.error('Failed to fetch current user', err);
      }
    });
  }

  fetchMessage(fromId: number, toId: number) {
    if(this.isSendingToGroup) {
      this.messageService.getAllGroup(toId).subscribe({
        next: (data) => {
          this.messageList = data.map((m: Message) => ({
            id: m.id,
            fromId: m.fromId,
            toId: m.toId,
            fromName: this.currentUser!.userName,
            toName: this.isSendingToGroup ? (this.groupToSend ? this.groupToSend.groupName : 'Unknown') : (this.userToSendReal ? this.userToSendReal.userName : 'Unknown'),
            isGroup: m.isGroup,
            content: m.content,
            createAt: m.createAt,
            isViewed: m.isViewed,
            hasFile: m.hasFile
          }));
          this.messageList.forEach((message, index) => {
            if (message.hasFile) {
              this.loadFilesForMessage(message, index);
            }
          });
          // Delay để Angular render DOM xong
          setTimeout(() => {
            this.scrollToBottom();
          }, 100);
        },
        error: (err) => {
          console.error('Failed to fetch messages', err);
        }
      });
    }else{
      this.messageService.getAll(fromId, toId).subscribe({
        next: (data) => {
          this.messageList = data.map((m: Message) => ({
            id: m.id,
            fromId: m.fromId,
            toId: m.toId,
            fromName: m.fromId === this.currentUser!.id ? this.currentUser!.userName : (this.userToSendReal ? this.userToSendReal.userName : 'Unknown'),
            toName: this.isSendingToGroup ? (this.groupToSend ? this.groupToSend.groupName : 'Unknown') : (this.userToSendReal ? this.userToSendReal.userName : 'Unknown'),
            isGroup: m.isGroup,
            content: m.content,
            createAt: m.createAt,
            isViewed: m.isViewed,
            hasFile: m.hasFile
          }));
          this.messageList.forEach((message, index) => {
            if (message.hasFile) {
              this.loadFilesForMessage(message, index);
            }
          });
          // Delay để Angular render DOM xong
          setTimeout(() => {
            this.scrollToBottom();
          }, 100);
        },
        error: (err) => {
          console.error('Failed to fetch messages', err);
        }
      });
    }
    
  }

  scrollToBottom(): void {
    try {
      this.chatContainer.nativeElement.scrollTop = this.chatContainer.nativeElement.scrollHeight;
    } catch (err) {
      console.error(err);
    }
  }

  fetchChat(): void {
    this.messageService.getById(this.currentUser!.id).subscribe({
      next: (data) => {
        this.totalMessageList = data;
      },
      error: (err) => {
        console.error('Failed to fetch messages', err);
        }
    });
  }

  selectChat(toId: number, message: MessageView) {
    if(message.isGroup) {
      this.userToSendReal = null;
      this.groupToSend.id = toId;
      this.groupToSend.groupName = message.fromName;
      this.groupService.getGroupUsers(this.groupToSend.id).subscribe({
        next: (data: User[]) => {
          this.groupToSend.users = data;
        },
        error: (err) => {
          console.error('Failed to fetch messages', err);
          }
      });
    }else{
      this.groupToSend = { id: 0, groupName: '', users: [] };;
      this.userToSendReal = this.userList.find(u => u.id === toId) || null;
    }
    this.isSendingToGroup = message.isGroup;
    this.selectedChatName = message.fromName;
    this.fetchMessage(this.currentUser!.id, toId);
    if(message.fromId !== this.currentUser?.id && !message.isViewed) {
      message.isViewed = true;
      this.messageService.update(message).subscribe({
          next: () => {
            this.signalRService.viewedMessage(message.id);
          },
          error: (err) => {
            console.error('Failed to fetch messages', err);
            }
        });
    }
  }

  markAsViewed(message: Message) {
    message.isViewed = true;
    return ""; 
  }

  toggleEmojiPicker() {
    this.showEmojiPicker = !this.showEmojiPicker;
  }

  addEmoji(event: any) {
    // event.nativeEvent.unified chứa emoji
    this.newMessage += event.emoji.native; 
  }

  @HostListener('document:click', ['$event'])
  clickOutside(event: Event) {
    if (
      this.showEmojiPicker &&
      this.emojiWrapper &&
      !this.emojiWrapper.nativeElement.contains(event.target)
    ) {
      this.showEmojiPicker = false;
    }
  }

  onFilesSelected(event: any) {
    this.files = event.target.files;

    this.filesBase64 = []; // reset trước

    Array.from(this.files).forEach((file: File) => {
      const reader = new FileReader();
      reader.onload = () => {
        const base64 = (reader.result as string).split(',')[1];
        this.filesBase64.push({
          fileName: file.name,
          id: "",
          messageId: "",
          FileBase64Content: base64
        });
      };
      reader.readAsDataURL(file);
    });
  }

  uploadFiles(messageId: string) {
    if (this.uploadProgress > 0) return;
    if (this.filesBase64.length === 0) return;
    let uploadedCount = 0; // số file đã upload
    const totalFiles = this.filesBase64.length;

    this.filesBase64.forEach(file => {
      file.messageId = messageId;
      this.additionFileService.create({...file}).subscribe({
        next: () => {
          uploadedCount++;
          this.uploadProgress = Math.round((uploadedCount / totalFiles) * 100);
          if (uploadedCount === totalFiles) {
            alert('Tất cả file đã upload xong!');
            this.uploadProgress = 0;
            this.files = [];
            this.filesBase64 = [];
          }
        },
        error: () => {
          uploadedCount++;
          this.uploadProgress = Math.round((uploadedCount / totalFiles) * 100);
          console.error('Upload file lỗi:', file.fileName);
        }
      });
    });
  }

  downloadFile(fileId: string, fileName: string) {
    this.additionFileService.getById(fileId).subscribe(blob => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = fileName;
        a.click();
        window.URL.revokeObjectURL(url);
      });
  }

  loadFilesForMessage(message: any, index: number) {
    this.additionFileService.getAll(message.id).subscribe(files => {
      const container = this.messageFileContainers.toArray()[index].nativeElement;

      console.log(files)
      // Xóa nội dung cũ
      container.innerHTML = '';

      files.forEach((file: any) => {
        const fileDiv = document.createElement('div');
        fileDiv.classList.add('file-item');
        fileDiv.style.cursor = 'pointer';
        fileDiv.style.margin = '5px';
        fileDiv.innerText = file.fileName.split("_")[0];

        // Gắn sự kiện click
        fileDiv.addEventListener('click', () => {
          this.downloadFile(file.id, file.fileName); 
        });

        container.appendChild(fileDiv);
      });
    });
  }

  log(ji: any) {
    console.log(ji)
  }
}
