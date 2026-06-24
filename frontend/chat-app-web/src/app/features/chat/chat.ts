import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
  selector: 'app-chat',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `<p>WIP Chat</p>`,
})
export class ChatComponent {}
