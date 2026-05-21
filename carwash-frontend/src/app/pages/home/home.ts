import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

// Home Page Component
// Yeh hamara landing page hai - jab koi pehli baar site pe aaye to yeh dikhega
// Path: http://localhost:4200/

@Component({
  selector: 'app-home',
  imports: [RouterLink],     // routerLink use karne ke liye import karna zaroori hai
  templateUrl: './home.html',
  styleUrl: './home.scss'
})
export class HomeComponent {
  // Features list jo home page pe dikhayenge
  // Yeh TypeScript array hai - hum isko HTML mein loop karenge
  features = [
    {
      icon: '🚗',
      title: 'Basic Wash',
      description: 'Exterior washing with soap and rinse'
    },
    {
      icon: '✨',
      title: 'Premium Wash',
      description: 'Full interior and exterior cleaning'
    },
    {
      icon: '🏠',
      title: 'Home Service',
      description: 'We come to your doorstep'
    },
    {
      icon: '⚡',
      title: 'Express Wash',
      description: 'Quick wash in under 30 minutes'
    }
  ];

  // Stats for home page display
  stats = [
    { number: '500+', label: 'Happy Customers' },
    { number: '1000+', label: 'Cars Washed' },
    { number: '50+', label: 'Washers Available' },
    { number: '4.8★', label: 'Average Rating' }
  ];
}
