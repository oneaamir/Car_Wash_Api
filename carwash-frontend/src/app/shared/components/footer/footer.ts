import { Component } from '@angular/core';

@Component({
  selector: 'app-footer',
  imports: [],
  templateUrl: './footer.html',
  styleUrl: './footer.scss'
})
export class FooterComponent {
  // Current year dynamically lena
  // new Date().getFullYear() = current year return karta hai (e.g., 2025)
  currentYear = new Date().getFullYear();
}
