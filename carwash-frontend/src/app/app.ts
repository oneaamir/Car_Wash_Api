import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from './shared/components/header/header';
import { FooterComponent } from './shared/components/footer/footer';

// @Component = Angular decorator hai
// Decorator ek special tag hai jo class ko batata hai ki yeh ek Angular Component hai
// Component = Angular ki building block - ek reusable piece of UI

@Component({
  selector: 'app-root',          // HTML mein yeh tag use hoga: <app-root>
  imports: [
    RouterOutlet,                // Router ka outlet - yahan pe page components render honge
    HeaderComponent,             // Hamara header component
    FooterComponent              // Hamara footer component
  ],
  templateUrl: './app.html',     // HTML template file
  styleUrl: './app.scss'         // Styles file
})
export class App {
  // App class = root component ki logic
  // Abhi koi logic nahi hai, sirf layout hai
}
