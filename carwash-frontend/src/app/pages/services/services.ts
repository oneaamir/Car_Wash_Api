import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ServicesService } from '../../core/services/services.service';
import { ServicePlan, AddOn } from '../../models/services.models';

@Component({
  selector: 'app-services',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './services.html',
  styleUrl: './services.scss'
})
export class ServicesComponent implements OnInit {
  private servicesService = inject(ServicesService);

  plans = signal<ServicePlan[]>([]);
  addOns = signal<AddOn[]>([]);
  isLoadingPlans = signal(true);
  isLoadingAddOns = signal(true);
  errorPlans = signal('');
  errorAddOns = signal('');

  ngOnInit(): void {
    this.loadPlans();
    this.loadAddOns();
  }

  loadPlans(): void {
    this.servicesService.getServicePlans().subscribe({
      next: (data) => {
        this.plans.set(data.filter(p => p.isActive));
        this.isLoadingPlans.set(false);
      },
      error: () => {
        this.errorPlans.set('Unable to load service plans. Please try again later.');
        this.isLoadingPlans.set(false);
      }
    });
  }

  loadAddOns(): void {
    this.servicesService.getAddOns().subscribe({
      next: (data) => {
        this.addOns.set(data.filter(a => a.isActive));
        this.isLoadingAddOns.set(false);
      },
      error: () => {
        this.errorAddOns.set('Unable to load add-ons. Please try again later.');
        this.isLoadingAddOns.set(false);
      }
    });
  }

  getPlanIcon(name: string): string {
    const lower = name.toLowerCase();
    if (lower.includes('basic') || lower.includes('standard')) return '🚿';
    if (lower.includes('premium') || lower.includes('deluxe')) return '✨';
    if (lower.includes('ultra') || lower.includes('full')) return '💎';
    return '🚗';
  }
}
