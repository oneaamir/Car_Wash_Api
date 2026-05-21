export interface AdminBooking {
  id: number;
  userId: number;
  carId: number;
  servicePlanId: number;
  promoCodeId: number | null;
  assignedWasherId: number | null;
  customerName: string;
  customerEmail: string;
  carNumber: string;
  servicePlanName: string;
  promoCode: string;
  assignedWasherName: string;
  assignedWasherEmail: string;
  addOnNames: string[];
  bookingType: string;
  bookingDate: string;
  address: string;
  status: string;
  totalAmount: number;
  message: string;
}

export interface AdminUser {
  id: number;
  fullName: string;
  email: string;
  phone: string;
  role: string;
  isActive: boolean;
  message: string;
}

export interface AssignWasherRequest { washerId: number; }
export interface UpdateBookingStatusRequest { status: string; }
export interface UpdatePaymentStatusRequest { paymentStatus: string; }

// Service Plans
export interface CreateServicePlanRequest { name: string; description: string; price: number; }
export interface UpdateServicePlanRequest { name: string; description: string; price: number; }

// Add-Ons
export interface CreateAddOnRequest { name: string; price: number; }
export interface UpdateAddOnRequest { name: string; price: number; }

// Promo Codes
export interface PromoCode {
  id: number; code: string; discountType: string;
  discountValue: number; expiryDate: string; isActive: boolean; message: string;
}
export interface CreatePromoCodeRequest { code: string; discountType: string; discountValue: number; expiryDate: string; }
export interface UpdatePromoCodeRequest { code: string; discountType: string; discountValue: number; expiryDate: string; }

// Reports
export interface ReportFilter { dateFrom?: string; dateTo?: string; }
export interface BookingReport {
  totalBookings: number;
  pendingBookings: number;
  confirmedBookings: number;
  inProgressBookings: number;
  completedBookings: number;
  cancelledBookings: number;
  message: string;
}
export interface PaymentMethodSummary { method: string; count: number; amount: number; }
export interface RevenueReport {
  totalPaymentAttempts: number;
  pendingPayments: number;
  successfulPayments: number;
  failedPayments: number;
  totalRevenue: number;
  averagePaymentAmount: number;
  revenueByMethod: PaymentMethodSummary[];
  message: string;
}
