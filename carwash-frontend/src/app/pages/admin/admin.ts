import { Component, inject, signal, effect } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { forkJoin, of } from 'rxjs';
import { AdminService } from '../../core/services/admin.service';
import { AdminTabService } from '../../core/services/admin-tab.service';
import { AdminBooking, AdminUser, PromoCode, BookingReport, RevenueReport, ReportFilter } from '../../models/admin.models';
import { Payment } from '../../models/payment.models';
import { ServicePlan, AddOn } from '../../models/services.models';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './admin.html',
  styleUrl: './admin.scss'
})
export class AdminComponent {
  private adminService = inject(AdminService);
  tabService = inject(AdminTabService);

  // ---- Bookings ----
  allBookings = signal<AdminBooking[]>([]);
  isLoadingBookings = signal(false);
  pageError  = signal('');
  successMsg = signal('');
  statusFilter  = signal('All');
  bookingSearch = signal('');
  bookingSortOrder = signal('newest');
  assigningId   = signal<number | null>(null);
  assignWasherId = signal(0);
  isAssigning   = signal(false);
  assignError   = signal('');
  updatingId    = signal<number | null>(null);
  isUpdating    = signal(false);
  updateError   = signal('');
  statusFilters = ['All', 'Pending', 'Confirmed', 'InProgress', 'Completed', 'Cancelled'];
  bookingSortOptions = [
    { value: 'newest',      label: 'Newest First' },
    { value: 'oldest',      label: 'Oldest First' },
    { value: 'amount-high', label: 'Amount: High to Low' },
    { value: 'amount-low',  label: 'Amount: Low to High' }
  ];

  // ---- Users ----
  allUsers        = signal<AdminUser[]>([]);
  isLoadingUsers  = signal(false);
  userSearch      = signal('');
  userRoleFilter  = signal('All');
  userRoleFilters = ['All', 'Customer', 'Washer', 'Admin'];
  userSortOrder   = signal('newest');
  userSortOptions = [
    { value: 'newest', label: 'Newest First' },
    { value: 'oldest', label: 'Oldest First' },
    { value: 'az',     label: 'Name A–Z' }
  ];
  washerSearch   = signal('');
  customerSearch = signal('');

  // ---- Services ----
  allPlans  = signal<ServicePlan[]>([]);
  allAddOns = signal<AddOn[]>([]);
  isLoadingServices = signal(false);
  servicesError     = signal('');
  private servicesLoaded = false;

  showPlanForm  = signal(false);
  editingPlanId = signal<number | null>(null);
  planFormData  = { name: '', description: '', price: 0 };
  isPlanSaving  = signal(false);
  planFormError = signal('');

  showAddonForm  = signal(false);
  editingAddonId = signal<number | null>(null);
  addonFormData  = { name: '', price: 0 };
  isAddonSaving  = signal(false);
  addonFormError = signal('');

  // ---- Promo Codes ----
  allPromoCodes   = signal<PromoCode[]>([]);
  isLoadingPromos = signal(false);
  promosError     = signal('');
  private promosLoaded = false;

  showPromoForm  = signal(false);
  editingPromoId = signal<number | null>(null);
  promoFormData  = { code: '', discountType: 'Percentage', discountValue: 0, expiryDate: '' };
  isPromoSaving  = signal(false);
  promoFormError = signal('');
  discountTypes  = ['Percentage', 'Fixed'];
  promoSortOrder = signal('expiry-asc');
  promoSortOptions = [
    { value: 'expiry-asc',  label: 'Expiry: Soonest First' },
    { value: 'expiry-desc', label: 'Expiry: Latest First' },
    { value: 'az',          label: 'Code A–Z' }
  ];
  copiedPromoId = signal<number | null>(null);

  // ---- Payments ----
  allPayments         = signal<Payment[]>([]);
  isLoadingPayments   = signal(false);
  paymentsError       = signal('');
  paymentStatusFilter = signal('All');
  paymentSearch       = signal('');
  paymentSortOrder    = signal('newest');
  paymentSortOptions  = [
    { value: 'newest',      label: 'Newest First' },
    { value: 'oldest',      label: 'Oldest First' },
    { value: 'amount-high', label: 'Amount: High to Low' },
    { value: 'amount-low',  label: 'Amount: Low to High' }
  ];
  updatingPaymentId   = signal<number | null>(null);
  isUpdatingPayment   = signal(false);
  updatePaymentError  = signal('');
  paymentStatusFilters = ['All', 'Pending', 'Success', 'Failed'];
  private paymentsLoaded = false;

  // ---- Reports ----
  reportMonth   = signal(new Date().getMonth() + 1);
  reportYear    = signal(new Date().getFullYear());
  bookingReport = signal<BookingReport | null>(null);
  revenueReport = signal<RevenueReport | null>(null);
  isLoadingReport = signal(false);
  reportError     = signal('');

  months = [
    { value: 1,  label: 'January' },  { value: 2,  label: 'February' },
    { value: 3,  label: 'March' },    { value: 4,  label: 'April' },
    { value: 5,  label: 'May' },      { value: 6,  label: 'June' },
    { value: 7,  label: 'July' },     { value: 8,  label: 'August' },
    { value: 9,  label: 'September' },{ value: 10, label: 'October' },
    { value: 11, label: 'November' }, { value: 12, label: 'December' }
  ];

  // ---- Overview ----
  todayBookingReport = signal<BookingReport | null>(null);
  todayRevenueReport = signal<RevenueReport | null>(null);
  isLoadingOverview  = signal(false);
  private overviewLoaded = false;
  private bookingsLoaded = false;
  private usersLoaded    = false;

  constructor() {
    effect(() => {
      const tab = this.tabService.activeTab();
      if (tab === 'overview')                                               this.loadOverview();
      if (tab === 'bookings' && !this.bookingsLoaded)                       this.loadBookings();
      if ((tab === 'washers' || tab === 'customers') && !this.usersLoaded)  this.loadUsers();
      if (tab === 'payments' && !this.paymentsLoaded)                       this.loadPayments();
      if (tab === 'services' && !this.servicesLoaded)                       this.loadServices();
      if (tab === 'promos'   && !this.promosLoaded)                         this.loadPromoCodes();
      if (tab === 'reports')                                                this.loadReports();
    });
  }

  get reportYears(): number[] {
    const current = new Date().getFullYear();
    return [current - 1, current, current + 1];
  }

  // ========== Tab ==========
  setTab(tab: string): void {
    this.tabService.activeTab.set(tab);
    this.closeAssign();
    this.closeUpdate();
  }

  // ========== Overview ==========
  loadOverview(): void {
    if (this.overviewLoaded) return;
    this.overviewLoaded = true;
    this.isLoadingOverview.set(true);
    const today = new Date().toISOString().split('T')[0];
    const filter: ReportFilter = { dateFrom: today, dateTo: today };
    forkJoin({
      bookingReport: this.adminService.getBookingReport(filter),
      revenueReport: this.adminService.getRevenueReport(filter),
      bookings: this.bookingsLoaded ? of(this.allBookings()) : this.adminService.getAllBookings(),
      users:    this.usersLoaded   ? of(this.allUsers())    : this.adminService.getUsers(),
      payments: this.paymentsLoaded? of(this.allPayments()) : this.adminService.getAllPayments(),
    }).subscribe({
      next: ({ bookingReport, revenueReport, bookings, users, payments }) => {
        this.todayBookingReport.set(bookingReport);
        this.todayRevenueReport.set(revenueReport);
        if (!this.bookingsLoaded) { this.allBookings.set(bookings); this.bookingsLoaded = true; }
        if (!this.usersLoaded)    { this.allUsers.set(users);       this.usersLoaded = true; }
        if (!this.paymentsLoaded) { this.allPayments.set(payments); this.paymentsLoaded = true; }
        this.isLoadingOverview.set(false);
      },
      error: () => this.isLoadingOverview.set(false)
    });
  }

  get needsAssignment(): AdminBooking[] {
    return this.allBookings()
      .filter(b => b.status === 'Pending' && !b.assignedWasherId)
      .sort((a, b) => new Date(a.bookingDate).getTime() - new Date(b.bookingDate).getTime())
      .slice(0, 6);
  }

  get liveActivity(): Array<{ text: string; time: string; type: string }> {
    const bookings = this.allBookings().map(b => ({
      text: `${b.customerName} booked ${b.servicePlanName}`,
      time: b.createdAt,
      type: 'booking'
    }));
    const payments = this.allPayments()
      .filter(p => p.paymentStatus === 'Success')
      .map(p => ({
        text: `Payment Rs.${p.amount} received${p.customerName ? ' · ' + p.customerName : ''}`,
        time: p.createdAt,
        type: 'payment'
      }));
    return [...bookings, ...payments]
      .sort((a, b) => new Date(b.time).getTime() - new Date(a.time).getTime())
      .slice(0, 6);
  }

  get washerStatus(): Array<AdminUser & { currentStatus: string }> {
    return this.allUsers()
      .filter(u => u.role === 'Washer' && u.isActive)
      .map(w => ({
        ...w,
        currentStatus: this.allBookings().some(
          b => b.assignedWasherId === w.id && b.status === 'InProgress'
        ) ? 'On a job' : 'Available'
      }));
  }

  get activeWashersOnJob(): number {
    return this.washerStatus.filter(w => w.currentStatus === 'On a job').length;
  }

  get filteredWashers(): AdminUser[] {
    const q = this.washerSearch().toLowerCase();
    return this.allUsers()
      .filter(u => u.role === 'Washer')
      .filter(u => !q || u.fullName.toLowerCase().includes(q) || u.email.toLowerCase().includes(q));
  }

  get filteredCustomers(): AdminUser[] {
    const q = this.customerSearch().toLowerCase();
    return this.allUsers()
      .filter(u => u.role === 'Customer')
      .filter(u => !q || u.fullName.toLowerCase().includes(q) || u.email.toLowerCase().includes(q));
  }

  timeAgo(dateStr: string): string {
    if (!dateStr) return '';
    const diff = Date.now() - new Date(dateStr).getTime();
    const mins = Math.floor(diff / 60000);
    if (mins < 1)  return 'just now';
    if (mins < 60) return `${mins} min ago`;
    const hrs = Math.floor(mins / 60);
    if (hrs < 24)  return `${hrs} hr ago`;
    return `${Math.floor(hrs / 24)} days ago`;
  }

  getInitials(name: string): string {
    return (name || '').split(' ').filter(w => w).map(w => w[0].toUpperCase()).slice(0, 2).join('');
  }

  goAssignBooking(booking: AdminBooking): void {
    this.setTab('bookings');
    setTimeout(() => this.openAssign(booking.id, booking.assignedWasherId), 150);
  }

  // ========== Bookings ==========
  loadBookings(): void {
    this.bookingsLoaded = true;
    this.isLoadingBookings.set(true);
    this.adminService.getAllBookings().subscribe({
      next: (data) => { this.allBookings.set(data); this.isLoadingBookings.set(false); },
      error: () => { this.pageError.set('Unable to load bookings.'); this.isLoadingBookings.set(false); }
    });
  }

  loadUsers(): void {
    this.usersLoaded = true;
    this.isLoadingUsers.set(true);
    this.adminService.getUsers().subscribe({
      next: (data) => { this.allUsers.set(data); this.isLoadingUsers.set(false); },
      error: () => { this.isLoadingUsers.set(false); }
    });
  }

  get filteredBookings(): AdminBooking[] {
    const f = this.statusFilter();
    const q = this.bookingSearch().toLowerCase().trim();
    let list = f === 'All' ? this.allBookings() : this.allBookings().filter(b => b.status === f);
    if (q) {
      list = list.filter(b =>
        b.customerName.toLowerCase().includes(q) ||
        b.carNumber.toLowerCase().includes(q) ||
        String(b.id).includes(q)
      );
    }
    switch (this.bookingSortOrder()) {
      case 'oldest':      return [...list].sort((a, b) => new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime());
      case 'amount-high': return [...list].sort((a, b) => b.totalAmount - a.totalAmount);
      case 'amount-low':  return [...list].sort((a, b) => a.totalAmount - b.totalAmount);
      default:            return [...list].sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
    }
  }

  get filteredUsers(): AdminUser[] {
    const q  = this.userSearch().toLowerCase().trim();
    const rf = this.userRoleFilter();
    let list = rf === 'All' ? this.allUsers() : this.allUsers().filter(u => u.role === rf);
    if (q) {
      list = list.filter(u =>
        u.fullName.toLowerCase().includes(q) ||
        u.email.toLowerCase().includes(q)
      );
    }
    switch (this.userSortOrder()) {
      case 'oldest': return [...list].sort((a, b) => a.id - b.id);
      case 'az':     return [...list].sort((a, b) => a.fullName.localeCompare(b.fullName));
      default:       return [...list].sort((a, b) => b.id - a.id);
    }
  }

  get washers(): AdminUser[] {
    return this.allUsers().filter(u => u.role === 'Washer');
  }

  getNextStatuses(current: string): string[] {
    switch (current.toLowerCase()) {
      case 'pending':    return ['Confirmed', 'Cancelled'];
      case 'confirmed':  return ['InProgress', 'Cancelled'];
      case 'inprogress': return ['Completed'];
      default:           return [];
    }
  }

  setFilter(status: string): void { this.statusFilter.set(status); this.closeAssign(); this.closeUpdate(); }

  openAssign(bookingId: number, currentWasherId: number | null): void {
    this.closeUpdate();
    this.assigningId.set(bookingId);
    this.assignWasherId.set(currentWasherId ?? 0);
    this.assignError.set('');
  }
  closeAssign(): void { this.assigningId.set(null); this.assignWasherId.set(0); this.assignError.set(''); }

  submitAssign(bookingId: number): void {
    if (!this.assignWasherId()) { this.assignError.set('Please select a washer.'); return; }
    this.isAssigning.set(true);
    this.adminService.assignWasher(bookingId, { washerId: this.assignWasherId() }).subscribe({
      next: (updated) => {
        this.allBookings.update(list => list.map(b => b.id === bookingId ? updated : b));
        this.showSuccess(`Washer assigned to Booking #${bookingId}.`);
        this.isAssigning.set(false);
        this.closeAssign();
      },
      error: (err) => { this.assignError.set(err.error?.message || err.error || 'Failed to assign washer.'); this.isAssigning.set(false); }
    });
  }

  openUpdate(bookingId: number): void { this.closeAssign(); this.updatingId.set(bookingId); this.updateError.set(''); }
  closeUpdate(): void { this.updatingId.set(null); this.updateError.set(''); }

  submitStatus(bookingId: number, newStatus: string): void {
    this.isUpdating.set(true);
    this.adminService.updateBookingStatus(bookingId, { status: newStatus }).subscribe({
      next: (updated) => {
        this.allBookings.update(list => list.map(b => b.id === bookingId ? updated : b));
        this.showSuccess(`Booking #${bookingId} status updated to ${newStatus}.`);
        this.isUpdating.set(false);
        this.closeUpdate();
      },
      error: (err) => { this.updateError.set(err.error?.message || err.error || 'Failed to update status.'); this.isUpdating.set(false); }
    });
  }

  // ========== Service Plans ==========
  loadServices(): void {
    this.isLoadingServices.set(true);
    this.servicesLoaded = true;
    this.adminService.getAllPlans().subscribe({
      next: (data) => { this.allPlans.set(data); },
      error: () => { this.servicesError.set('Failed to load service plans.'); }
    });
    this.adminService.getAllAddOns().subscribe({
      next: (data) => { this.allAddOns.set(data); this.isLoadingServices.set(false); },
      error: () => { this.servicesError.set('Failed to load add-ons.'); this.isLoadingServices.set(false); }
    });
  }

  openAddPlan(): void {
    this.editingPlanId.set(null);
    this.planFormData = { name: '', description: '', price: 0 };
    this.planFormError.set('');
    this.showPlanForm.set(true);
  }

  openEditPlan(plan: ServicePlan): void {
    this.editingPlanId.set(plan.id);
    this.planFormData = { name: plan.name, description: plan.description, price: plan.price };
    this.planFormError.set('');
    this.showPlanForm.set(true);
  }

  closePlanForm(): void { this.showPlanForm.set(false); this.editingPlanId.set(null); this.planFormError.set(''); }

  submitPlanForm(): void {
    if (!this.planFormData.name.trim() || !this.planFormData.description.trim() || this.planFormData.price <= 0) {
      this.planFormError.set('All fields are required and price must be greater than 0.');
      return;
    }
    this.isPlanSaving.set(true);
    const editId = this.editingPlanId();
    const obs = editId
      ? this.adminService.updatePlan(editId, this.planFormData)
      : this.adminService.createPlan(this.planFormData);

    obs.subscribe({
      next: (saved) => {
        if (editId) {
          this.allPlans.update(list => list.map(p => p.id === editId ? saved : p));
          this.showSuccess('Service plan updated.');
        } else {
          this.allPlans.update(list => [...list, saved]);
          this.showSuccess('Service plan created.');
        }
        this.isPlanSaving.set(false);
        this.closePlanForm();
      },
      error: (err) => { this.planFormError.set(err.error?.message || err.error || 'Failed to save.'); this.isPlanSaving.set(false); }
    });
  }

  deletePlan(plan: ServicePlan): void {
    if (!confirm(`Delete service plan "${plan.name}"?`)) return;
    this.adminService.deletePlan(plan.id).subscribe({
      next: () => { this.allPlans.update(list => list.filter(p => p.id !== plan.id)); this.showSuccess('Service plan deleted.'); },
      error: (err) => { this.servicesError.set(err.error?.message || err.error || 'Failed to delete.'); }
    });
  }

  // ========== Add-Ons ==========
  openAddAddon(): void {
    this.editingAddonId.set(null);
    this.addonFormData = { name: '', price: 0 };
    this.addonFormError.set('');
    this.showAddonForm.set(true);
  }

  openEditAddon(addon: AddOn): void {
    this.editingAddonId.set(addon.id);
    this.addonFormData = { name: addon.name, price: addon.price };
    this.addonFormError.set('');
    this.showAddonForm.set(true);
  }

  closeAddonForm(): void { this.showAddonForm.set(false); this.editingAddonId.set(null); this.addonFormError.set(''); }

  submitAddonForm(): void {
    if (!this.addonFormData.name.trim() || this.addonFormData.price < 0) {
      this.addonFormError.set('Name is required and price cannot be negative.');
      return;
    }
    this.isAddonSaving.set(true);
    const editId = this.editingAddonId();
    const obs = editId
      ? this.adminService.updateAddOn(editId, this.addonFormData)
      : this.adminService.createAddOn(this.addonFormData);

    obs.subscribe({
      next: (saved) => {
        if (editId) {
          this.allAddOns.update(list => list.map(a => a.id === editId ? saved : a));
          this.showSuccess('Add-on updated.');
        } else {
          this.allAddOns.update(list => [...list, saved]);
          this.showSuccess('Add-on created.');
        }
        this.isAddonSaving.set(false);
        this.closeAddonForm();
      },
      error: (err) => { this.addonFormError.set(err.error?.message || err.error || 'Failed to save.'); this.isAddonSaving.set(false); }
    });
  }

  deleteAddon(addon: AddOn): void {
    if (!confirm(`Delete add-on "${addon.name}"?`)) return;
    this.adminService.deleteAddOn(addon.id).subscribe({
      next: () => { this.allAddOns.update(list => list.filter(a => a.id !== addon.id)); this.showSuccess('Add-on deleted.'); },
      error: (err) => { this.servicesError.set(err.error?.message || err.error || 'Failed to delete.'); }
    });
  }

  // ========== Promo Codes ==========
  loadPromoCodes(): void {
    this.isLoadingPromos.set(true);
    this.promosLoaded = true;
    this.adminService.getAllPromoCodes().subscribe({
      next: (data) => { this.allPromoCodes.set(data); this.isLoadingPromos.set(false); },
      error: () => { this.promosError.set('Failed to load promo codes.'); this.isLoadingPromos.set(false); }
    });
  }

  openAddPromo(): void {
    this.editingPromoId.set(null);
    this.promoFormData = { code: '', discountType: 'Percentage', discountValue: 0, expiryDate: '' };
    this.promoFormError.set('');
    this.showPromoForm.set(true);
  }

  openEditPromo(promo: PromoCode): void {
    this.editingPromoId.set(promo.id);
    this.promoFormData = {
      code: promo.code,
      discountType: promo.discountType,
      discountValue: promo.discountValue,
      expiryDate: promo.expiryDate ? promo.expiryDate.substring(0, 10) : ''
    };
    this.promoFormError.set('');
    this.showPromoForm.set(true);
  }

  closePromoForm(): void { this.showPromoForm.set(false); this.editingPromoId.set(null); this.promoFormError.set(''); }

  submitPromoForm(): void {
    if (!this.promoFormData.code.trim() || !this.promoFormData.expiryDate || this.promoFormData.discountValue <= 0) {
      this.promoFormError.set('All fields are required and discount value must be greater than 0.');
      return;
    }
    this.isPromoSaving.set(true);
    const editId = this.editingPromoId();
    const obs = editId
      ? this.adminService.updatePromoCode(editId, this.promoFormData)
      : this.adminService.createPromoCode(this.promoFormData);

    obs.subscribe({
      next: (saved) => {
        if (editId) {
          this.allPromoCodes.update(list => list.map(p => p.id === editId ? saved : p));
          this.showSuccess('Promo code updated.');
        } else {
          this.allPromoCodes.update(list => [...list, saved]);
          this.showSuccess('Promo code created.');
        }
        this.isPromoSaving.set(false);
        this.closePromoForm();
      },
      error: (err) => { this.promoFormError.set(err.error?.message || err.error || 'Failed to save.'); this.isPromoSaving.set(false); }
    });
  }

  get sortedPromoCodes(): PromoCode[] {
    switch (this.promoSortOrder()) {
      case 'expiry-desc': return [...this.allPromoCodes()].sort((a, b) => new Date(b.expiryDate).getTime() - new Date(a.expiryDate).getTime());
      case 'az':          return [...this.allPromoCodes()].sort((a, b) => a.code.localeCompare(b.code));
      default:            return [...this.allPromoCodes()].sort((a, b) => new Date(a.expiryDate).getTime() - new Date(b.expiryDate).getTime());
    }
  }

  copyPromoCode(promo: PromoCode): void {
    navigator.clipboard.writeText(promo.code).then(() => {
      this.copiedPromoId.set(promo.id);
      setTimeout(() => this.copiedPromoId.set(null), 2000);
    });
  }

  deletePromo(promo: PromoCode): void {
    if (!confirm(`Delete promo code "${promo.code}"?`)) return;
    this.adminService.deletePromoCode(promo.id).subscribe({
      next: () => { this.allPromoCodes.update(list => list.filter(p => p.id !== promo.id)); this.showSuccess('Promo code deleted.'); },
      error: (err) => { this.promosError.set(err.error?.message || err.error || 'Failed to delete.'); }
    });
  }

  // ========== Payments ==========
  loadPayments(): void {
    this.isLoadingPayments.set(true);
    this.paymentsLoaded = true;
    this.adminService.getAllPayments().subscribe({
      next: (data) => { this.allPayments.set(data); this.isLoadingPayments.set(false); },
      error: () => { this.paymentsError.set('Failed to load payments.'); this.isLoadingPayments.set(false); }
    });
  }

  get filteredPayments(): Payment[] {
    const f = this.paymentStatusFilter();
    const q = this.paymentSearch().toLowerCase().trim();
    let list = f === 'All' ? this.allPayments() : this.allPayments().filter(p => p.paymentStatus === f);
    if (q) {
      list = list.filter(p =>
        p.customerName.toLowerCase().includes(q) ||
        String(p.bookingId).includes(q)
      );
    }
    switch (this.paymentSortOrder()) {
      case 'oldest':      return [...list].sort((a, b) => new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime());
      case 'amount-high': return [...list].sort((a, b) => b.amount - a.amount);
      case 'amount-low':  return [...list].sort((a, b) => a.amount - b.amount);
      default:            return [...list].sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
    }
  }

  get paymentSummary(): { pending: number; pendingTotal: number; collected: number; collectedTotal: number } {
    const all = this.allPayments();
    const pending = all.filter(p => p.paymentStatus === 'Pending');
    const success = all.filter(p => p.paymentStatus === 'Success');
    return {
      pending:        pending.length,
      pendingTotal:   pending.reduce((s, p) => s + p.amount, 0),
      collected:      success.length,
      collectedTotal: success.reduce((s, p) => s + p.amount, 0)
    };
  }

  getNextPaymentStatuses(current: string): string[] {
    switch (current.toLowerCase()) {
      case 'pending': return ['Success', 'Failed'];
      case 'failed':  return ['Success'];
      default:        return [];
    }
  }

  openUpdatePayment(paymentId: number): void { this.updatingPaymentId.set(paymentId); this.updatePaymentError.set(''); }
  closeUpdatePayment(): void { this.updatingPaymentId.set(null); this.updatePaymentError.set(''); }

  submitPaymentStatus(paymentId: number, newStatus: string): void {
    this.isUpdatingPayment.set(true);
    this.adminService.updatePaymentStatus(paymentId, { paymentStatus: newStatus }).subscribe({
      next: (updated) => {
        this.allPayments.update(list => list.map(p => p.id === paymentId ? updated : p));
        this.showSuccess(`Payment #${paymentId} marked as ${newStatus}.`);
        this.isUpdatingPayment.set(false);
        this.closeUpdatePayment();
      },
      error: (err) => {
        this.updatePaymentError.set(err.error?.message || err.error || 'Failed to update payment.');
        this.isUpdatingPayment.set(false);
      }
    });
  }

  getPaymentStatusClass(status: string): string {
    switch (status?.toLowerCase()) {
      case 'success': return 'badge-completed';
      case 'failed':  return 'badge-cancelled';
      default:        return 'badge-pending';
    }
  }

  // ========== Reports ==========
  loadReports(): void {
    this.isLoadingReport.set(true);
    this.reportError.set('');

    const year  = this.reportYear();
    const month = this.reportMonth();
    const pad = (n: number) => String(n).padStart(2, '0');
    const lastDay = new Date(year, month, 0).getDate();
    const filter = {
      dateFrom: `${year}-${pad(month)}-01`,
      dateTo:   `${year}-${pad(month)}-${pad(lastDay)}`
    };

    let bookingDone = false;
    let revenueDone = false;

    this.adminService.getBookingReport(filter).subscribe({
      next: (data) => {
        this.bookingReport.set(data);
        bookingDone = true;
        if (revenueDone) this.isLoadingReport.set(false);
      },
      error: () => { this.reportError.set('Failed to load booking report.'); this.isLoadingReport.set(false); }
    });

    this.adminService.getRevenueReport(filter).subscribe({
      next: (data) => {
        this.revenueReport.set(data);
        revenueDone = true;
        if (bookingDone) this.isLoadingReport.set(false);
      },
      error: () => { this.reportError.set('Failed to load revenue report.'); this.isLoadingReport.set(false); }
    });
  }

  get selectedMonthLabel(): string {
    return this.months.find(m => m.value === this.reportMonth())?.label ?? '';
  }

  // ========== Helpers ==========
  private showSuccess(msg: string): void {
    this.successMsg.set(msg);
    setTimeout(() => this.successMsg.set(''), 3000);
  }

  formatDate(dateStr: string): string {
    if (!dateStr) return '';
    return new Date(dateStr).toLocaleDateString('en-IN', { day: '2-digit', month: 'short', year: 'numeric' });
  }

  getStatusClass(status: string): string {
    switch (status?.toLowerCase()) {
      case 'pending':    return 'badge-pending';
      case 'confirmed':  return 'badge-confirmed';
      case 'inprogress': return 'badge-inprogress';
      case 'completed':  return 'badge-completed';
      case 'cancelled':  return 'badge-cancelled';
      default:           return 'badge-pending';
    }
  }

  getRoleClass(role: string): string {
    switch (role?.toLowerCase()) {
      case 'admin':  return 'role-admin';
      case 'washer': return 'role-washer';
      default:       return 'role-customer';
    }
  }
}
