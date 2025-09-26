import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../../core/services/account-service';

@Component({
  selector: 'app-nav',
  imports: [FormsModule],
  templateUrl: './nav.html',
  styleUrl: './nav.css'
})
export class Nav {
  protected creds: any = {};

  protected isLoggedIn = signal(false);

  private accountService = inject(AccountService);

  login() {
    this.accountService.login(this.creds).subscribe({
      next: (result) => {
        console.log(result);
        this.isLoggedIn.set(true);
        this.creds = {};
      },
      error: (err) => alert(err.message)
    });
  }

  logout() {
    this.isLoggedIn.set(false);
  }
}
